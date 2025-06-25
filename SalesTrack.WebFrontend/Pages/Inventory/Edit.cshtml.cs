using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesTrack.Shared.DTOs;
using SalesTrack.WebFrontend.Services;

namespace SalesTrack.WebFrontend.Pages.Inventory
{
    public class EditModel : PageModel
    {
        private readonly InventoryApiClient _inventoryApiClient;

        public EditModel(InventoryApiClient inventoryApiClient)
        {
            _inventoryApiClient = inventoryApiClient;
        }

        [BindProperty]
        public InventoryDto InventoryItem { get; set; } = new InventoryDto();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var item = await _inventoryApiClient.GetInventoryByIdAsync(id.Value);
                if (item == null)
                {
                    return NotFound();
                }
                InventoryItem = item;
                ViewData["Title"] = $"Edit: {InventoryItem.ProductName}";
            }
            else
            {
                InventoryItem = new InventoryDto();
                ViewData["Title"] = "Add Inventory Item";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (InventoryItem.Id == 0)     // Only allow insert when Id not provided
            {
                await _inventoryApiClient.CreateInventoryAsync(InventoryItem);
            }
            else
            {
                await _inventoryApiClient.UpdateInventoryAsync(InventoryItem.Id, InventoryItem);
            }

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (InventoryItem.Id > 0)
            {
                await _inventoryApiClient.DeleteInventoryAsync(InventoryItem.Id);
            }

            return RedirectToPage("Index");
        }

    }
}
