using Microsoft.AspNetCore.Mvc;
using Parcial1.Models;
using Parcial1.Services;

namespace Parcial1.Controllers
{
    /// Controlador principal para gestionar torneos.
    public class TorneoController : Controller
    {
        private readonly TorneoService _torneoService;
        private readonly EquipoService _equipoService;
        private readonly PartidoService _partidoService;

        public TorneoController(
            TorneoService torneoService,
            EquipoService equipoService,
            PartidoService partidoService)
        {
            _torneoService = torneoService;
            _equipoService = equipoService;
            _partidoService = partidoService;
        }

        ///Torneo
        public async Task<IActionResult> Index()
        {
            var torneos = await _torneoService.ObtenerTodosAsync();
            return View(torneos);
        }

        ///Torneo/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        ///Torneo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Torneo torneo)
        {
            if (!ModelState.IsValid)
                return View(torneo);

            await _torneoService.CrearAsync(torneo);
            return RedirectToAction(nameof(Index));
        }

        ///Torneo/Editar/5
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var torneo = await _torneoService.ObtenerPorIdAsync(id);
            if (torneo == null)
                return NotFound();

            return View(torneo);
        }

        ///Torneo/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Torneo torneo)
        {
            if (!ModelState.IsValid)
                return View(torneo);

            await _torneoService.ActualizarAsync(torneo);
            return RedirectToAction(nameof(Index));
        }

        ///Torneo/Activar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            await _torneoService.ActivarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        ///Torneo/Desactivar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var desactivado = await _torneoService.DesactivarAsync(id);

            if (!desactivado)
                TempData["Error"] = "No se puede finalizar un torneo con equipos inscritos.";

            return RedirectToAction(nameof(Index));
        }

        ///Torneo/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var torneo = await _torneoService.ObtenerPorIdAsync(id);
            if (torneo == null)
                return NotFound();

            ViewBag.Equipos = await _equipoService.ObtenerPorTorneoAsync(id);
            ViewBag.Partidos = await _partidoService.ObtenerPorTorneoAsync(id);

            return View(torneo);
        }

        ///Torneo/GenerarCuartos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarCuartos(int id)
        {
            var equipos = await _equipoService.ObtenerPorTorneoAsync(id);

            if (equipos.Count != 8)
            {
                TempData["Error"] = "Se necesitan 8 equipos para jugar.";
                return RedirectToAction("Detalle", new { id });
            }

            var partidosExistentes = await _partidoService.ObtenerPorRondaAsync(id, 1);
            if (partidosExistentes.Any())
            {
                TempData["Error"] = "Cuartos de final.";
                return RedirectToAction("Detalle", new { id });
            }

            await _partidoService.GenerarCuartosAsync(id, equipos);
            return RedirectToAction("Detalle", new { id });
        }

        ///Torneo/GenerarSiguienteRonda
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarSiguienteRonda(int id, int rondaActual)
        {
            var partidos = await _partidoService.ObtenerPorRondaAsync(id, rondaActual);

            if (partidos.Any(p => !p.Jugado))
            {
                TempData["Error"] = "Registra los resultados de la ronda actual.";
                return RedirectToAction("Detalle", new { id });
            }

            if (partidos.Count == 1)
            {
                TempData["Error"] = "El torneo tiene un ganador.";
                return RedirectToAction("Detalle", new { id });
            }

            await _partidoService.GenerarSiguienteRondaAsync(id, rondaActual);
            return RedirectToAction("Detalle", new { id });
        }
    }
}