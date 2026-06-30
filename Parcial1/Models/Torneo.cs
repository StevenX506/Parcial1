using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Parcial1.Models
{
    public class Torneo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edición es obligatoria.")]
        [Range(1, 9999, ErrorMessage = "La edición debe ser un número válido.")]
        public int Edicion { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}