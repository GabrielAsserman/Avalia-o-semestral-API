# Avaliação Prática – CRUD Salas de Reunião (Minimal API + JWT)

Projeto pronto para a atividade. Backend em **Minimal API (C#)** com **JWT** e **Entity Framework Core (MySQL)**, e frontend em `index.html` (React via Babel) já com todos os `// COMPLETAR` resolvidos.

## Estrutura

```
SalasReuniaoApi/   -> Backend (.NET, Minimal API)
frontend/index.html -> Frontend (abrir direto no navegador)
```

## 1. Configurar o banco de dados

1. Crie um banco MySQL chamado `salasreuniao` (ou deixe a tabela ser criada automaticamente — veja o passo 3).
2. Abra `SalasReuniaoApi/appsettings.json` e troque `SUA_SENHA_AQUI` pela senha do seu usuário MySQL (e o nome do banco/usuário, se for diferente):

```json
"DefaultConnection": "server=localhost;database=salasreuniao;user=root;password=SUA_SENHA_AQUI"
```

## 2. Rodar o backend

```bash
cd SalasReuniaoApi
dotnet restore
dotnet run
```

A API vai subir em **http://localhost:5198** (porta já configurada em `Properties/launchSettings.json`, e já é a mesma usada no `index.html`).

## 3. Banco de dados (migration)

O `Program.cs` já está configurado para aplicar a migration automaticamente ao iniciar (`db.Database.Migrate()`), ou seja, ao rodar `dotnet run` a tabela `SalasReuniao` é criada sozinha no banco — não precisa rodar `dotnet ef database update` manualmente.

Se preferir gerar a migration você mesmo (ou se der algum erro de compatibilidade), apague a pasta `Migrations/` e rode:

```bash
dotnet tool install --global dotnet-ef   # se ainda não tiver o dotnet-ef
dotnet ef migrations add Inicial
dotnet ef database update
```

## 4. Testar a API (opcional, mas recomendado antes do frontend)

Use o arquivo `SalasReuniaoApi/SalasReuniaoApi.http` (abre direto no VS Code com a extensão REST Client, ou copie as requisições pro Postman/Insomnia).

- Login fixo: `email: teste@teste.com` / `senha: 123`

## 5. Rodar o frontend

Basta abrir `frontend/index.html` direto no navegador (duplo clique) com o backend rodando. O CORS já está liberado para qualquer origem.

## Resumo do que foi entregue

- **Backend** (`SalasReuniaoApi/`):
  - `POST /login` — autenticação fixa, retorna JWT
  - `GET /salas`, `POST /salas`, `PUT /salas/{id}`, `DELETE /salas/{id}` — CRUD completo, todos protegidos por JWT
  - Tabela `SalasReuniao` (Id, Nome, Capacidade, PossuiProjetor) via EF Core + MySQL
- **Frontend** (`frontend/index.html`): todos os `// COMPLETAR` do enunciado resolvidos (login, listagem, criação, edição e exclusão, com o token sempre enviado no header `Authorization: Bearer`)

## Pendências para você revisar

- A senha do MySQL no `appsettings.json` (placeholder).
- Não testei `dotnet build`/`dotnet run` localmente (ambiente onde gerei isso não tem o SDK do .NET instalado) — rode `dotnet build` primeiro pra confirmar que compila limpo na sua máquina antes da entrega. Se aparecer algum erro de compilação, me cole o erro aqui que eu ajusto.
