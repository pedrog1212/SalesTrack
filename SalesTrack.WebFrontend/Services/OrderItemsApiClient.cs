using System.Net.Http.Json;
using SalesTrack.Shared.DTOs;

namespace SalesTrack.WebFrontend.Services
{
    public class OrderItemsApiClient
    {
        private readonly HttpClient _http;

        public OrderItemsApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<OrderItemDto>> GetItemsForOrderAsync(int orderId)
        {
            return await _http.GetFromJsonAsync<List<OrderItemDto>>($"api/orderitems/order/{orderId}") ?? new();
        }

        public async Task AddItemAsync(OrderItemDto item)
        {
            var response = await _http.PostAsJsonAsync("api/orderitems", item);
            response.EnsureSuccessStatusCode();
        }
        public async Task UpdateItemAsync(int id, OrderItemDto item)
        {
            var response = await _http.PutAsJsonAsync($"api/orderitems/{id}", item);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteItemAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/orderitems/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
