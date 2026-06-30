using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Parcial1.Models
{
    public class Partido
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("torneo_id")]
        public int TorneoId { get; set; }

        [JsonPropertyName("equipo_local_id")]
        public int EquipoLocalId { get; set; }

        [JsonPropertyName("equipo_visitante_id")]
        public int EquipoVisitanteId { get; set; }

        [JsonPropertyName("ronda")]
        public int Ronda { get; set; } = 1;

        [JsonPropertyName("fecha_partido")]
        public DateTime FechaPartido { get; set; } = DateTime.Today;

        [JsonPropertyName("goles_local")]
        [Range(0, 99, ErrorMessage = "Los goles deben estar entre 0 y 99.")]
        public int GolesLocal { get; set; }

        [JsonPropertyName("goles_visitante")]
        [Range(0, 99, ErrorMessage = "Los goles deben estar entre 0 y 99.")]
        public int GolesVisitante { get; set; }

        [JsonPropertyName("jugado")]
        public bool Jugado { get; set; } = false;

        [JsonPropertyName("ganador_id")]
        public int? GanadorId { get; set; }
    }
}