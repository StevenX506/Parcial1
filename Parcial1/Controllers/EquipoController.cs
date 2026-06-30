using Microsoft.AspNetCore.Mvc;
using Parcial1.Models;
using Parcial1.Services;

namespace Parcial1.Controllers
{
    public class EquipoController : Controller
    {
        private readonly EquipoService _equipoService;

        public EquipoController(EquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        public async Task<IActionResult> Index(int torneoId)
        {
            var equipos = await _equipoService.ObtenerPorTorneoAsync(torneoId);
            ViewBag.TorneoId = torneoId;
            return View(equipos);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var equipo = await _equipoService.ObtenerPorIdAsync(id);

            if (equipo == null)
                return NotFound();

            return View(equipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Equipo equipo)
        {
            if (!ModelState.IsValid)
                return View(equipo);

            await _equipoService.ActualizarAsync(equipo);

            return RedirectToAction("Detalle", "Torneo", new { id = equipo.TorneoId });
        }

        public IActionResult Crear(int torneoId)
        {
            var equipo = new Equipo
            {
                TorneoId = torneoId
            };

            return View(equipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Equipo equipo)
        {
            if (!ModelState.IsValid)
                return View(equipo);

            await _equipoService.CrearAsync(equipo);

            return RedirectToAction("Detalle", "Torneo", new { id = equipo.TorneoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, int torneoId)
        {
            var eliminado = await _equipoService.EliminarAsync(id, torneoId);

            if (!eliminado)
                TempData["Error"] = "No se puede eliminar un equipo que ya participó en un partido.";

            return RedirectToAction("Detalle", "Torneo", new { id = torneoId });
        }
    }
}