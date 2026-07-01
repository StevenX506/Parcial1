using System.Text.Json;
using Parcial1.Models;

namespace Parcial1.Services
{
    //Servicio para gestionar operaciones sobre la tabla de equipos.
    public class EquipoService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _partidosUrl;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public EquipoService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["Supabase:Url"]! + "/rest/v1/equipos";
            _partidosUrl = config["Supabase:Url"]! + "/rest/v1/partidos";
        }

        //Obtiene todos los equipos inscritos.
        public async Task<List<Equipo>> ObtenerPorTorneoAsync(int torneoId)
        {
            var response = await _http.GetAsync($"{_baseUrl}?select=*&torneo_id=eq.{torneoId}");
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<List<Equipo>>(json, _jsonOptions) ?? new List<Equipo>();
            }
            catch
            {
                return new List<Equipo>();
            }
        }

        //Obtiene un equipo por su ID.
        public async Task<Equipo?> ObtenerPorIdAsync(int id)
        {
            var response = await _http.GetAsync($"{_baseUrl}?id=eq.{id}&select=*");
            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonSerializer.Deserialize<List<Equipo>>(json, _jsonOptions);
            return lista?.FirstOrDefault();
        }

        //Crea un nuevo equipo en un torneo.
        public async Task CrearAsync(Equipo equipo)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
            request.Headers.Add("Prefer", "return=minimal");
            request.Content = JsonContent.Create(new
            {
                torneo_id = equipo.TorneoId,
                nombre = equipo.Nombre,
                ciudad = equipo.Ciudad
            });
            await _http.SendAsync(request);
        }

        //ctualiza el nombre y ciudad de un equipo.
        public async Task ActualizarAsync(Equipo equipo)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}?id=eq.{equipo.Id}");
            request.Headers.Add("Prefer", "return=minimal");
            request.Content = JsonContent.Create(new
            {
                nombre = equipo.Nombre,
                ciudad = equipo.Ciudad
            });
            await _http.SendAsync(request);
        }

        /// Elimina un equipo si no ha jugado.
        public async Task<bool> EliminarAsync(int id, int torneoId)
        {
            var response = await _http.GetAsync(
                $"{_partidosUrl}?select=id&torneo_id=eq.{torneoId}&or=(equipo_local_id.eq.{id},equipo_visitante_id.eq.{id})");
            var json = await response.Content.ReadAsStringAsync();
            var partidos = JsonSerializer.Deserialize<List<Partido>>(json, _jsonOptions);

            if (partidos != null && partidos.Any())
                return false;

            await _http.DeleteAsync($"{_baseUrl}?id=eq.{id}");
            return true;
        }
    }
}