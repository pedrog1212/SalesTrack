using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesTrack.Shared.DTOs;
using SalesTrack.WebFrontend.Services;

namespace SalesTrack.WebFrontend.Pages.Inventory
{
    public class IndexModel : PageModel
    {
        private readonly InventoryApiClient _inventoryApiClient;

        public IndexModel(InventoryApiClient inventoryApiClient)
        {
            _inventoryApiClient = inventoryApiClient;
        }

        public List<InventoryDto> InventoryItems { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ProductNameFilter { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? SKUFilter { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? SortColumn { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? SortDirection { get; set; } = "asc";


        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0;

        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageIndex > 0;
        public bool HasNextPage => PageIndex + 1 < TotalPages;
        public string CurrentSort => SortDirection ?? "";

        public async Task OnGetAsync()
        {
            var items = await _inventoryApiClient.GetInventoryItemsAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                items = items.Where(i =>
                    i.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (i.ProductName != null && i.ProductName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(ProductNameFilter))
            {
                items = items.Where(i =>
                    i.ProductName != null && i.ProductName.Contains(ProductNameFilter, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(SKUFilter))
            {
                items = items.Where(i =>
                   i.SKU != null && i.SKU.Contains(SKUFilter, StringComparison.OrdinalIgnoreCase)
               ).ToList();
            }

            items = (SortColumn, SortDirection?.ToLower()) switch
            {

                ("Id", "asc") => items.OrderBy(i => i.Id).ToList(),
                ("Id", "desc") => items.OrderByDescending(i => i.Id).ToList(),

                ("ProductName", "asc") => items.OrderBy(i => i.ProductName).ToList(),
                ("ProductName", "desc") => items.OrderByDescending(i => i.ProductName).ToList(),

                ("SKU", "asc") => items.OrderBy(i => i.SKU).ToList(),
                ("SKU", "desc") => items.OrderByDescending(i => i.SKU).ToList(),

                ("QuantityAvailable", "asc") => items.OrderBy(i => i.QuantityAvailable).ToList(),
                ("QuantityAvailable", "desc") => items.OrderByDescending(i => i.QuantityAvailable).ToList(),


                ("UnitPrice", "asc") => items.OrderBy(i => i.UnitPrice).ToList(),
                ("UnitPrice", "desc") => items.OrderByDescending(i => i.UnitPrice).ToList(),

                _ => items.OrderBy(i => i.Id).ToList(),

            };

            //InventoryItems = items;
            int totalItems = items.Count;
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            InventoryItems = items.Skip(PageIndex * PageSize).Take(PageSize).ToList();

        }

        public string GetSortDirection(string column)
        {
            return SortColumn == column && SortDirection == "asc" ? "desc" : "asc";
        }
    }
}
