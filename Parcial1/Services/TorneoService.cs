using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Parcial1.Models;

namespace Parcial1.Services
{
    public class TorneoService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public TorneoService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["Supabase:Url"]! + "/rest/v1/torneos";
        }

        public async Task<List<Torneo>> ObtenerTodosAsync()
        {
            var response = await _http.GetAsync($"{_baseUrl}?select=*");

            if (!response.IsSuccessStatusCode)
                return new List<Torneo>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Torneo>>(json, _jsonOptions)
                   ?? new List<Torneo>();
        }

        public async Task<List<Torneo>> ObtenerActivosAsync()
        {
            var response = await _http.GetAsync($"{_baseUrl}?select=*&activo=eq.true");

            if (!response.IsSuccessStatusCode)
                return new List<Torneo>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Torneo>>(json, _jsonOptions)
                   ?? new List<Torneo>();
        }

        public async Task<Torneo?> ObtenerPorIdAsync(int id)
        {
            var response = await _http.GetAsync($"{_baseUrl}?id=eq.{id}&select=*");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            var lista = JsonSerializer.Deserialize<List<Torneo>>(json, _jsonOptions);

            return lista?.FirstOrDefault();
        }

        public async Task CrearAsync(Torneo torneo)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);
            request.Headers.Add("Prefer", "return=minimal");

            request.Content = JsonContent.Create(new
            {
                nombre = torneo.Nombre,
                edicion = torneo.Edicion,
                activo = torneo.Activo
            });

            await _http.SendAsync(request);
        }

        public async Task ActualizarAsync(Torneo torneo)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}?id=eq.{torneo.Id}");
            request.Headers.Add("Prefer", "return=minimal");

            request.Content = JsonContent.Create(new
            {
                nombre = torneo.Nombre,
                edicion = torneo.Edicion,
                activo = torneo.Activo
            });

            await _http.SendAsync(request);
        }

        public async Task ActivarAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}?id=eq.{id}");
            request.Headers.Add("Prefer", "return=minimal");

            request.Content = JsonContent.Create(new { activo = true });

            await _http.SendAsync(request);
        }

        public async Task DesactivarAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}?id=eq.{id}");
            request.Headers.Add("Prefer", "return=minimal");

            request.Content = JsonContent.Create(new { activo = false });

            await _http.SendAsync(request);
        }
    }
}