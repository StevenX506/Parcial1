using Microsoft.AspNetCore.Mvc;
using Parcial1.Models;
using Parcial1.Services;

namespace Parcial1.Controllers
{
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

        public async Task<IActionResult> Index()
        {
            var torneos = await _torneoService.ObtenerTodosAsync();
            return View(torneos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            await _torneoService.ActivarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Torneo torneo)
        {
            await _torneoService.ActualizarAsync(torneo);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Torneo torneo)
        {
            Console.WriteLine($"NOMBRE: {torneo.Nombre}, EDICION: {torneo.Edicion}");
            Console.WriteLine($"MODELSTATE VALID: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ERROR: {error.ErrorMessage}");
                }
                return View(torneo);
            }

            await _torneoService.CrearAsync(torneo);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var torneo = await _torneoService.ObtenerPorIdAsync(id);

            if (torneo == null)
                return NotFound();

            return View(torneo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _torneoService.DesactivarAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var torneo = await _torneoService.ObtenerPorIdAsync(id);

            if (torneo == null)
                return NotFound();

            ViewBag.Equipos = await _equipoService.ObtenerPorTorneoAsync(id);
            ViewBag.Partidos = await _partidoService.ObtenerPorTorneoAsync(id);

            return View(torneo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarCuartos(int id)
        {
            var equipos = await _equipoService.ObtenerPorTorneoAsync(id);

            if (equipos.Count != 8)
            {
                TempData["Error"] = "Se necesitan exactamente 8 equipos para generar los cuartos de final.";
                return RedirectToAction("Detalle", new { id });
            }

            var partidosExistentes = await _partidoService.ObtenerPorRondaAsync(id, 1);
            if (partidosExistentes.Any())
            {
                TempData["Error"] = "Los cuartos de final ya fueron generados.";
                return RedirectToAction("Detalle", new { id });
            }

            await _partidoService.GenerarCuartosAsync(id, equipos);
            return RedirectToAction("Detalle", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarSiguienteRonda(int id, int rondaActual)
        {
            var partidos = await _partidoService.ObtenerPorRondaAsync(id, rondaActual);

            if (partidos.Any(p => !p.Jugado))
            {
                TempData["Error"] = "Debes registrar todos los resultados de la ronda actual antes de continuar.";
                return RedirectToAction("Detalle", new { id });
            }

            if (partidos.Count == 1)
            {
                TempData["Error"] = "El torneo ya tiene un ganador.";
                return RedirectToAction("Detalle", new { id });
            }

            await _partidoService.GenerarSiguienteRondaAsync(id, rondaActual);
            return RedirectToAction("Detalle", new { id });
        }
    }
}