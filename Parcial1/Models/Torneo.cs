using System.ComponentModel.DataAnnotations;

namespace Parcial1.Models
{
    public class Torneo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public int Edicion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreadoEn { get; set; } = DateTime.Now;
    }
}