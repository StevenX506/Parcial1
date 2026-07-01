using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Parcial1.Models
{
    /// <summary>
    /// Representa un torneo de fútbol administrado en el sistema.
    /// </summary>
    public class Torneo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("edicion")]
        [Required(ErrorMessage = "La edición es obligatoria.")]
        [Range(1, 9999, ErrorMessage = "La edición es obligatoria.")]
        public int Edicion { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        [JsonPropertyName("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}