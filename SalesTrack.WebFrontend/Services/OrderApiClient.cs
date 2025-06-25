using SalesTrack.Shared.DTOs;

namespace SalesTrack.WebFrontend.Services
{
    public class OrderApiClient
    {
        private readonly HttpClient _http;

        public OrderApiClient(HttpClient http)
        {
            _http = http;
        }
        public OrderApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("API");
        }

        public async Task<List<OrderDto>> GetOrdersAsync()
        {
            return await _http.GetFromJsonAsync<List<OrderDto>>("api/orders") ?? new();
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int? customerId, string? searchTerm, string? sortOrder)
        {
            string query = "";

            if (customerId.HasValue)
                query += $"customerId={customerId.Value}&";
            if (!string.IsNullOrEmpty(searchTerm))
                query += $"searchTerm={Uri.EscapeDataString(searchTerm)}&";
            if (!string.IsNullOrEmpty(sortOrder))
                query += $"sortOrder={Uri.EscapeDataString(sortOrder)}";

            string url = "api/orders";
            if (!string.IsNullOrEmpty(query))
                url += "?" + query.TrimEnd('&');

            return await _http.GetFromJsonAsync<List<OrderDto>>(url) ?? new List<OrderDto>();
        }
        
        public async Task<OrderDto?> GetOrderAsync(int id)
        {
            return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
        }

        public async Task CreateOrderAsync(OrderDto order)
        {
            var response = await _http.PostAsJsonAsync("api/orders", order);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateOrderAsync(int id, OrderDto order)
        {
            Console.WriteLine($"Trying to update api/orders/{id}"); // or set a breakpoint

            var response = await _http.PutAsJsonAsync($"api/orders/{id}", order);

            Console.WriteLine($"Passed PutAsJsonAsync ?"); // or set a breakpoint

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/orders/{id}");
            response.EnsureSuccessStatusCode();
        }

    }
}
