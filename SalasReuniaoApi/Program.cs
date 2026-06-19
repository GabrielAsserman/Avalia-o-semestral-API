using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalasReuniaoApi.Data;
using SalasReuniaoApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------- BANCO DE DADOS (MySQL + EF Core) ----------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    )
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------- CORS (liberado para o frontend) ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------------------------- JWT ----------------------------
const string chaveJwt = "MINHA_CHAVE_SUPER_SECRETA_SALAS_REUNIAO_2026";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ---------------------------- APLICAR MIGRATIONS AUTOMATICAMENTE ----------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------- LOGIN ----------------------------
// Usuário fixo para simplificar (sem tabela de usuários), conforme dica do enunciado.
app.MapPost("/login", (LoginRequest login) =>
{
    if (login.Email == "teste@teste.com" && login.Senha == "123")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(chaveJwt);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, login.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Results.Ok(new { token = tokenString });
    }

    return Results.Unauthorized();
});

// ---------------------------- CRUD: SALAS DE REUNIÃO ----------------------------
// Todas as rotas abaixo exigem JWT válido (RequireAuthorization).

app.MapGet("/salas", async (AppDbContext db) =>
    await db.SalasReuniao.ToListAsync()
).RequireAuthorization();

app.MapGet("/salas/{id:int}", async (int id, AppDbContext db) =>
    await db.SalasReuniao.FindAsync(id)
        is SalaReuniao sala ? Results.Ok(sala) : Results.NotFound()
).RequireAuthorization();

app.MapPost("/salas", async (SalaReuniao novaSala, AppDbContext db) =>
{
    db.SalasReuniao.Add(novaSala);
    await db.SaveChangesAsync();
    return Results.Created($"/salas/{novaSala.Id}", novaSala);
}).RequireAuthorization();

app.MapPut("/salas/{id:int}", async (int id, SalaReuniao salaAtualizada, AppDbContext db) =>
{
    var sala = await db.SalasReuniao.FindAsync(id);
    if (sala is null) return Results.NotFound();

    sala.Nome = salaAtualizada.Nome;
    sala.Capacidade = salaAtualizada.Capacidade;
    sala.PossuiProjetor = salaAtualizada.PossuiProjetor;

    await db.SaveChangesAsync();
    return Results.Ok(sala);
}).RequireAuthorization();

app.MapDelete("/salas/{id:int}", async (int id, AppDbContext db) =>
{
    var sala = await db.SalasReuniao.FindAsync(id);
    if (sala is null) return Results.NotFound();

    db.SalasReuniao.Remove(sala);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();
