using Microsoft.AspNetCore.Mvc;
using Parcial1.Services;

namespace Parcial1.Controllers
{
    /// Controlador para gestionar los partidos.
    public class PartidoController : Controller
    {
        private readonly PartidoService _partidoService;
        private readonly EquipoService _equipoService;

        public PartidoController(PartidoService partidoService, EquipoService equipoService)
        {
            _partidoService = partidoService;
            _equipoService = equipoService;
        }

        [HttpGet]
        public async Task<IActionResult> Resultado(int id)
        {
            var partido = await _partidoService.ObtenerPorIdAsync(id);
            if (partido == null)
                return NotFound();

            if (partido.Jugado)
            {
                TempData["Error"] = "Este partido ya fue jugado y no se puede editar.";
                return RedirectToAction("Detalle", "Torneo", new { id = partido.TorneoId });
            }

            return View(partido);
        }

        ///Partido/RegistrarResultado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarResultado(int id, int golesLocal, int golesVisitante, int torneoId)
        {
            var resultado = await _partidoService.RegistrarResultadoAsync(id, golesLocal, golesVisitante);

            if (resultado == "empate")
                return RedirectToAction("Penales", new { id, torneoId });

            if (resultado == "error")
                TempData["Error"] = "El partido ya fue jugado. ";

            return RedirectToAction("Detalle", "Torneo", new { id = torneoId });
        }

        ///Partido/Penales/5
        [HttpGet]
        public async Task<IActionResult> Penales(int id, int torneoId)
        {
            var partido = await _partidoService.ObtenerPorIdAsync(id);
            if (partido == null)
                return NotFound();

            var equipos = await _equipoService.ObtenerPorTorneoAsync(torneoId);
            ViewBag.Local = equipos.FirstOrDefault(e => e.Id == partido.EquipoLocalId);
            ViewBag.Visitante = equipos.FirstOrDefault(e => e.Id == partido.EquipoVisitanteId);
            ViewBag.TorneoId = torneoId;

            return View(partido);
        }

        ///Partido/Penales
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Penales(int id, int ganadorId, int torneoId)
        {
            await _partidoService.DefinirGanadorPenalesAsync(id, ganadorId);
            return RedirectToAction("Detalle", "Torneo", new { id = torneoId });
        }

        ///Partido/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, int torneoId)
        {
            var eliminado = await _partidoService.EliminarAsync(id);

            if (!eliminado)
                TempData["Error"] = "No se puede eliminar un partido terminado.";

            return RedirectToAction("Detalle", "Torneo", new { id = torneoId });
        }
    }
}