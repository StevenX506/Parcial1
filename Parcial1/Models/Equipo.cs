using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Parcial1.Models
{
    public class Equipo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("torneo_id")]
        public int TorneoId { get; set; }

        [JsonPropertyName("nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("ciudad")]
        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(100)]
        public string Ciudad { get; set; } = string.Empty;
    }
}