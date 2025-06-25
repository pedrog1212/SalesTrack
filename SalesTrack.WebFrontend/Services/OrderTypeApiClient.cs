using SalesTrack.Shared.DTOs;

namespace SalesTrack.WebFrontend.Services
{
    public class OrderTypeApiClient
    {
        private readonly HttpClient _http;

        public OrderTypeApiClient(HttpClient http)
        {
            _http = http;
        }
        public OrderTypeApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("API");
        }

        // GET all order types
        public async Task<List<OrderTypeDto>> GetOrderTypesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<OrderTypeDto>>("api/ordertypes");
            return result ?? new List<OrderTypeDto>();
        }

        // GET a single order type by ID
        public async Task<OrderTypeDto?> GetOrderTypeAsync(int id)
        {
            return await _http.GetFromJsonAsync<OrderTypeDto>($"api/ordertypes/{id}");
        }

        // POST a new order type
        public async Task CreateOrderTypeAsync(OrderTypeDto type)
        {
            var response = await _http.PostAsJsonAsync("api/ordertypes", type);
            response.EnsureSuccessStatusCode();
        }

        // PUT update an existing order type
        public async Task UpdateOrderTypeAsync(int id, OrderTypeDto type)
        {
            var response = await _http.PutAsJsonAsync($"api/ordertypes/{id}", type);
            response.EnsureSuccessStatusCode();
        }

        // DELETE an order type by ID
        public async Task DeleteOrderTypeAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/ordertypes/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
