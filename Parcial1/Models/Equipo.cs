using System.ComponentModel.DataAnnotations;

namespace Parcial1.Models
{
    public class Equipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El torneo es obligatorio.")]
        public int TorneoId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(100)]
        public string Ciudad { get; set; } = string.Empty;
    }
}