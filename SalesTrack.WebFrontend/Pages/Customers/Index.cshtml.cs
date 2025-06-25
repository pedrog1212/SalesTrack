using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesTrack.Shared.DTOs;
using SalesTrack.WebFrontend.Services;

namespace SalesTrack.WebFrontend.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly CustomerApiClient _api;
        public IndexModel(CustomerApiClient api) => _api = api;

        public List<CustomerDto> PagedCustomers { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0;

        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageIndex > 0;
        public bool HasNextPage => PageIndex + 1 < TotalPages;
        public string CurrentSort => SortOrder ?? "";

        public async Task OnGetAsync()
        {
            var customers = await _api.GetCustomersAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                customers = customers
                    .Where(c =>
                        c.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.Id.ToString() == SearchTerm)
                    .ToList();
            }

            customers = SortOrder switch
            {
                "FullName_desc" => customers.OrderByDescending(c => c.FullName).ToList(),
                "Email" => customers.OrderBy(c => c.Email).ToList(),
                "Email_desc" => customers.OrderByDescending(c => c.Email).ToList(),
                "PhoneNumber" => customers.OrderBy(c => c.PhoneNumber).ToList(),
                "PhoneNumber_desc" => customers.OrderByDescending(c => c.PhoneNumber).ToList(),
                "Address" => customers.OrderBy(c => c.Address).ToList(),
                "Address_desc" => customers.OrderByDescending(c => c.Address).ToList(),
                "Id_desc" => customers.OrderByDescending(c => c.Id).ToList(),
                _ => customers.OrderBy(c => c.Id).ToList(),
            };

            TotalPages = (int)Math.Ceiling(customers.Count / (double)PageSize);
            PagedCustomers = customers
                .Skip(PageIndex * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
