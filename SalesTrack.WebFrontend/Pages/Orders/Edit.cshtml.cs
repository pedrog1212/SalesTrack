using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
using SalesTrack.Shared.DTOs;
using SalesTrack.WebFrontend.Services;

namespace SalesTrack.WebFrontend.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly OrderApiClient _orderApi;
        private readonly CustomerApiClient _customerApi;
        private readonly OrderStatusApiClient _statusApi;
        private readonly OrderTypeApiClient _typeApi;
        private readonly OrderItemsApiClient _itemApi;
        private readonly InventoryApiClient _inventoryApi;

        public EditModel(OrderApiClient orderApi, CustomerApiClient customerApi, OrderStatusApiClient statusApi,
                         OrderTypeApiClient typeApi, OrderItemsApiClient itemApi, InventoryApiClient inventoryApi)
        {
            _orderApi = orderApi;
            _customerApi = customerApi;
            _statusApi = statusApi;
            _typeApi = typeApi;
            _itemApi = itemApi;
            _inventoryApi = inventoryApi;

        }

        [BindProperty]
        public OrderDto Order { get; set; }

        [BindProperty]
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public List<InventoryDto> InventoryItems { get; set; } = new();

        public Dictionary<int, string> InventoryLookup { get; set; } = new();
        public Dictionary<int, string> InventorySkuLookup { get; set; } = new();

        public SelectList Customers { get; set; }
        public SelectList Statuses { get; set; }
        public SelectList OrderTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadDropdowns();

            if (id == null)
            {
                // Adding a New order
                Order = new OrderDto
                {
                    CustomerId = 0,
                    //FullName = string.Empty, // Clear customer name
                    OrderDate = DateTime.Today,
                    DeliveryDate = null,
                    OrderStatusId = 0,
                    OrderTypeId = 0
                };
                ViewData["Title"] = "Add Customer";


                return Page();
            }

            // Editing an existing order
            var existing = await _orderApi.GetOrderAsync(id.Value);
            if (existing == null) return NotFound();

            Order = new OrderDto
            {
                Id = existing.Id,
                CustomerId = existing.CustomerId,
                FullName = existing.FullName ?? "[Unknown]",
                OrderDate = existing.OrderDate,
                DeliveryDate = existing.DeliveryDate,
                OrderStatusId = existing.OrderStatusId,
                OrderTypeId = existing.OrderTypeId
            };
            OrderItems = await _itemApi.GetItemsForOrderAsync(id.Value);

            InventoryItems = await _inventoryApi.GetInventoryItemsAsync(); //  service call to Expose Inventory columns into Razor Model

            InventoryLookup = InventoryItems.ToDictionary(i => i.Id, i => i.ProductName);

            InventorySkuLookup = InventoryItems.ToDictionary(i => i.Id, i => i.SKU);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"Posting Order with ID: {Order.Id}"); // or set a breakpoint
            
            await LoadDropdowns();

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Model error for {kvp.Key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            Console.WriteLine($"ModelState is Valid"); // or set a breakpoint

            var orderToSend = new OrderDto
            {
                Id = Order.Id,
                CustomerId = Order.CustomerId,
                OrderDate = Order.OrderDate,
                DeliveryDate = Order.DeliveryDate,
                OrderStatusId = Order.OrderStatusId,
                OrderTypeId = Order.OrderTypeId 
            };

            Console.WriteLine($"Data transfered"); // or set a breakpoint

            if (Order.Id == 0)
            {
                await _orderApi.CreateOrderAsync( orderToSend);
            }
            else
            {
                await _orderApi.UpdateOrderAsync(Order.Id, orderToSend);
            }

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (Order.Id == 0) return NotFound();

            await _orderApi.DeleteOrderAsync(Order.Id);
            return RedirectToPage("Index");
        }

        private async Task LoadDropdowns()
        {
            // OrderStatuses drop-down List
            var statusList = await _statusApi.GetOrderStatusesAsync();

            if (statusList == null || !statusList.Any())
            {
                statusList = new List<OrderStatusDto>
                {
                    new OrderStatusDto { Id = 0, StatusName = "[None Available]" }
                };
            }
            Statuses = new SelectList(statusList, "Id", "StatusName");

            // OrderTypes drop-down List
            var typeList = await _typeApi.GetOrderTypesAsync();

            if (typeList == null || !typeList.Any())
            {
                typeList = new List<OrderTypeDto>
                {
                    new OrderTypeDto { Id = 0, TypeName = "[None Available]" }
                };
            }
            OrderTypes = new SelectList(typeList, "Id", "TypeName");

            // Customers drop-down List
            var customerList = await _customerApi.GetCustomersAsync();

            if (customerList == null || !customerList.Any())
            {
                customerList = new List<CustomerDto>
                {
                    new CustomerDto { Id = 0, FullName = "[No Customers]" }
                };
            }
            Customers = new SelectList(customerList, "Id", "FullName");
        }
    }
}

