using SalesTrack.Shared.DTOs;
using System.Net.Http.Json;

namespace SalesTrack.WebFrontend.Services
{
    public class InventoryApiClient
    {
        private readonly HttpClient _http;

        public InventoryApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<InventoryDto>> GetInventoryItemsAsync()
        {
            return await _http.GetFromJsonAsync<List<InventoryDto>>("api/inventory") ?? new();
        }

        public async Task<InventoryDto?> GetInventoryByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<InventoryDto>($"api/inventory/{id}");
        }

        public async Task CreateInventoryAsync(InventoryDto item)
        {
            var response = await _http.PostAsJsonAsync("api/inventory", item);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateInventoryAsync(int id, InventoryDto item)
        {
            var response = await _http.PutAsJsonAsync($"api/inventory/{id}", item);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteInventoryAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/inventory/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
