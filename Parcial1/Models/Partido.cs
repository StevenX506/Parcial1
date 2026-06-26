using System.ComponentModel.DataAnnotations;

namespace Parcial1.Models
{
    public class Partido
    {
        public int Id { get; set; }

        public int TorneoId { get; set; }

        public int EquipoLocalId { get; set; }

        public int EquipoVisitanteId { get; set; }

        [Range(0, 99)]
        public int GolesLocal { get; set; }

        [Range(0, 99)]
        public int GolesVisitante { get; set; }

        public DateTime FechaPartido { get; set; }

        public bool Jugado { get; set; }
    }
}