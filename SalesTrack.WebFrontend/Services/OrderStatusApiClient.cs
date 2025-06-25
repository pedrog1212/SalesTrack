using SalesTrack.Shared.DTOs;

namespace SalesTrack.WebFrontend.Services
{
    public class OrderStatusApiClient
    {
        private readonly HttpClient _http;

        public OrderStatusApiClient(HttpClient http)
        {
            _http = http;
        }
        public OrderStatusApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("API");
        }

        // GET all order statuses
        public async Task<List<OrderStatusDto>> GetOrderStatusesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<OrderStatusDto>>("api/orderstatuses");
            return result ?? new List<OrderStatusDto>();
        }

        // GET a single order status by ID
        public async Task<OrderStatusDto?> GetOrderStatusAsync(int id)
        {
            return await _http.GetFromJsonAsync<OrderStatusDto>($"api/orderstatuses/{id}");
        }

        // POST a new order status
        public async Task CreateOrderStatusAsync(OrderStatusDto status)
        {
            var response = await _http.PostAsJsonAsync("api/orderstatuses", status);
            response.EnsureSuccessStatusCode();
        }

        // PUT update an existing order status
        public async Task UpdateOrderStatusAsync(int id, OrderStatusDto status)
        {
            var response = await _http.PutAsJsonAsync($"api/orderstatuses/{id}", status);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> DeleteOrderStatusAsync(int id)
        {
            return await _http.DeleteAsync($"api/orderstatuses/{id}");
        }
    }

}

