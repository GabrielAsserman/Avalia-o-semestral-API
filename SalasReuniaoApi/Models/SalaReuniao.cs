using System.ComponentModel.DataAnnotations;

namespace SalasReuniaoApi.Models
{
    public class SalaReuniao
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        public int Capacidade { get; set; }

        public bool PossuiProjetor { get; set; }
    }
}
