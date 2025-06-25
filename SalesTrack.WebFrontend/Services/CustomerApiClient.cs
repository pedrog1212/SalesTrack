using System.Net.Http;
using SalesTrack.Shared.DTOs;

namespace SalesTrack.WebFrontend.Services
{
    public class CustomerApiClient
    {
        private readonly HttpClient _http;

        public CustomerApiClient(HttpClient http)
        {
            _http = http;
        }
        public CustomerApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("API");
        }
        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            var response = await _http.GetAsync("api/customers");

            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
                return customers ?? new List<CustomerDto>();
            }

            // Log error or handle specific cases here
            return new List<CustomerDto>();
        }

        public async Task<CustomerDto?> GetCustomerAsync(int id)
        {
            var response = await _http.GetAsync($"api/customers/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CustomerDto>();
            }

            return null;
        }
        
        public async Task CreateCustomerAsync(CustomerDto customer)
        {
            Console.WriteLine($"Creating customer: {System.Text.Json.JsonSerializer.Serialize(customer)}");

            var response = await _http.PostAsJsonAsync("api/customers", customer);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {response.StatusCode} - {error}");
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateCustomerAsync_v1(CustomerDto customer)
        {
            return await _http.PutAsJsonAsync($"api/customers/{customer.Id}", customer);
        }
        public async Task UpdateCustomerAsync_v2(int id, CustomerDto customer)
        {
            var response = await _http.PutAsJsonAsync($"api/customers/{id}", customer);
            response.EnsureSuccessStatusCode();
        }
        public async Task UpdateCustomerAsync(int id, CustomerDto customer)
        {
            var response = await _http.PutAsJsonAsync($"api/customers/{id}", customer);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error updating customer: {response.StatusCode}, {response.Content}");
            }
            response.EnsureSuccessStatusCode();

        }

        public async Task<HttpResponseMessage> DeleteCustomerAsync(int id)
        {
            return await _http.DeleteAsync($"api/customers/{id}");
        }
    }
}

/*
public async Task DeleteCustomerAsync(int id)
{
    var response = await _http.DeleteAsync($"api/customers/{id}");
    response.EnsureSuccessStatusCode();
}
*/