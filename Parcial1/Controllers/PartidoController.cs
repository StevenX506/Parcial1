using Microsoft.AspNetCore.Mvc;
using Parcial1.Models;
using Parcial1.Services;

namespace Parcial1.Controllers
{
    public class PartidoController : Controller
    {
        private readonly PartidoService _partidoService;
        private readonly EquipoService _equipoService;

        public PartidoController(PartidoService partidoService, EquipoService equipoService)
        {
            _partidoService = partidoService;
            _equipoService = equipoService;
        }

        public async Task<IActionResult> Resultado(int id)
        {
            var partido = await _partidoService.ObtenerPorIdAsync(id);
            if (partido == null)
                return NotFound();

            if (partido.Jugado)
            {
                TempData["Error"] = "Este partido ya fue jugado y no puede editarse.";
                return RedirectToAction("Detalle", "Torneo", new { id = partido.TorneoId });
            }

            return View(partido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarResultado(int id, int golesLocal, int golesVisitante, int torneoId)
        {
            var resultado = await _partidoService.RegistrarResultadoAsync(id, golesLocal, golesVisitante);

            if (resultado == "empate")
                return RedirectToAction("Penales", new { id, torneoId });

            if (resultado == "error")
                TempData["Error"] = "No se pudo registrar el resultado. El partido ya fue jugado.";

            return RedirectToAction("Detalle", "Torneo", new { id = torneoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Penales(int id, int ganadorId, int torneoId)
        {
            await _partidoService.DefinirGanadorPenalesAsync(id, ganadorId);
            return RedirectToAction("Detalle", "Torneo", new { id = torneoId });
        }

        public async Task<IActionResult> Penales(int id, int torneoId)
        {
            var partido = await _partidoService.ObtenerPorIdAsync(id);
            if (partido == null)
                return NotFound();

            var equipos = await _equipoService.ObtenerPorTorneoAsync(torneoId);
            var local = equipos.FirstOrDefault(e => e.Id == partido.EquipoLocalId);
            var visitante = equipos.FirstOrDefault(e => e.Id == partido.EquipoVisitanteId);

            ViewBag.Local = local;
            ViewBag.Visitante = visitante;
            ViewBag.TorneoId = torneoId;

            return View(partido);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, int torneoId)
        {
            var eliminado = await _partidoService.EliminarAsync(id);

            if (!eliminado)
                TempData["Error"] = "No se puede eliminar un partido que ya fue jugado.";

            return RedirectToAction("Detalle", "Torneo", new { id = torneoId });
        }
    }
}