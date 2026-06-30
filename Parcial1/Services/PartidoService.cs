using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Parcial1.Models;

namespace Parcial1.Services
{
    public class PartidoService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public PartidoService(HttpClient http, IConfiguration config)
        {
            _http = http;
            // Construimos la URL absoluta directa y recuperamos la ApiKey aquí
            _baseUrl = config["Supabase:Url"]!.TrimEnd('/') + "/rest/v1/partidos";
            _apiKey = config["Supabase:ApiKey"]!;
        }

        private HttpRequestMessage CrearMensajeConCabeceras(HttpMethod metodo, string url)
        {
            var request = new HttpRequestMessage(metodo, url);
            request.Headers.Add("apikey", _apiKey);
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            return request;
        }

        public async Task<List<Partido>> ObtenerPorTorneoAsync(int torneoId)
        {
            var url = $"{_baseUrl}?select=*&torneo_id=eq.{torneoId}";
            var request = CrearMensajeConCabeceras(HttpMethod.Get, url);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<Partido>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Partido>>(json, _jsonOptions) ?? new List<Partido>();
        }

        public async Task<List<Partido>> ObtenerPorRondaAsync(int torneoId, int ronda)
        {
            var url = $"{_baseUrl}?select=*&torneo_id=eq.{torneoId}&ronda=eq.{ronda}";
            var request = CrearMensajeConCabeceras(HttpMethod.Get, url);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<Partido>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Partido>>(json, _jsonOptions) ?? new List<Partido>();
        }

        public async Task<Partido?> ObtenerPorIdAsync(int id)
        {
            var url = $"{_baseUrl}?id=eq.{id}&select=*";
            var request = CrearMensajeConCabeceras(HttpMethod.Get, url);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonSerializer.Deserialize<List<Partido>>(json, _jsonOptions);
            return lista?.FirstOrDefault();
        }

        public async Task CrearAsync(Partido partido)
        {
            var request = CrearMensajeConCabeceras(HttpMethod.Post, _baseUrl);
            request.Headers.Add("Prefer", "return=minimal");
            request.Content = JsonContent.Create(new
            {
                torneo_id = partido.TorneoId,
                equipo_local_id = partido.EquipoLocalId,
                equipo_visitante_id = partido.EquipoVisitanteId,
                ronda = partido.Ronda,
                fecha_partido = partido.FechaPartido,
                jugado = false,
                goles_local = 0,
                goles_visitante = 0
            });
            await _http.SendAsync(request);
        }

        public async Task GenerarCuartosAsync(int torneoId, List<Equipo> equipos)
        {
            var random = new Random();
            var lista = equipos.OrderBy(_ => random.Next()).ToList();

            for (int i = 0; i < lista.Count; i += 2)
            {
                await CrearAsync(new Partido
                {
                    TorneoId = torneoId,
                    EquipoLocalId = lista[i].Id,
                    EquipoVisitanteId = lista[i + 1].Id,
                    FechaPartido = DateTime.Today,
                    Ronda = 1
                });
            }
        }

        public async Task GenerarSiguienteRondaAsync(int torneoId, int rondaActual)
        {
            var partidos = await ObtenerPorRondaAsync(torneoId, rondaActual);

            if (partidos.Any(p => !p.Jugado))
                return;

            var ganadores = partidos
                .Select(p => p.GanadorId ?? (p.GolesLocal > p.GolesVisitante ? p.EquipoLocalId : p.EquipoVisitanteId))
                .ToList();

            var random = new Random();
            var lista = ganadores.OrderBy(_ => random.Next()).ToList();

            for (int i = 0; i < lista.Count; i += 2)
            {
                await CrearAsync(new Partido
                {
                    TorneoId = torneoId,
                    EquipoLocalId = lista[i],
                    EquipoVisitanteId = lista[i + 1],
                    FechaPartido = DateTime.Today,
                    Ronda = rondaActual + 1
                });
            }
        }

        public async Task<string> RegistrarResultadoAsync(int id, int golesLocal, int golesVisitante)
        {
            var partido = await ObtenerPorIdAsync(id);
            if (partido == null || partido.Jugado)
                return "error";

            if (golesLocal == golesVisitante)
            {
                var request = CrearMensajeConCabeceras(HttpMethod.Patch, $"{_baseUrl}?id=eq.{id}");
                request.Headers.Add("Prefer", "return=minimal");
                request.Content = JsonContent.Create(new
                {
                    goles_local = golesLocal,
                    goles_visitante = golesVisitante
                });
                await _http.SendAsync(request);
                return "empate";
            }

            var request2 = CrearMensajeConCabeceras(HttpMethod.Patch, $"{_baseUrl}?id=eq.{id}");
            request2.Headers.Add("Prefer", "return=minimal");
            request2.Content = JsonContent.Create(new
            {
                goles_local = golesLocal,
                goles_visitante = golesVisitante,
                jugado = true
            });
            await _http.SendAsync(request2);
            return "ok";
        }

        public async Task<bool> DefinirGanadorPenalesAsync(int id, int ganadorId)
        {
            var partido = await ObtenerPorIdAsync(id);
            if (partido == null || partido.Jugado)
                return false;

            var request = CrearMensajeConCabeceras(HttpMethod.Patch, $"{_baseUrl}?id=eq.{id}");
            request.Headers.Add("Prefer", "return=minimal");
            request.Content = JsonContent.Create(new
            {
                ganador_id = ganadorId,
                jugado = true
            });
            await _http.SendAsync(request);
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var partido = await ObtenerPorIdAsync(id);
            if (partido == null || partido.Jugado)
                return false;

            var request = CrearMensajeConCabeceras(HttpMethod.Delete, $"{_baseUrl}?id=eq.{id}");
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}