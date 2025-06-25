using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesTrack.Shared.DTOs;
using SalesTrack.WebFrontend.Models;
using SalesTrack.WebFrontend.Services;

namespace SalesTrack.WebFrontend.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly CustomerApiClient _api;
        public EditModel(CustomerApiClient api) => _api = api;

        [BindProperty]
        public CustomerDto Customer { get; set; }
        public SelectList Customers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                Customer = new CustomerDto();
            }

            else
            {
                //Customer = await _api.GetCustomerAsync(id.Value) ?? new Customer();
                /*
                var customerList = await _api.GetCustomersAsync();

                if (!customerList.Any())
                {
                    customerList = new List<CustomerDto>
                    {
                        new CustomerDto { Id = 0, FullName = "[No Customers]" }
                    };
                }

                Customers = new SelectList(customerList, "Id", "FullName");
                */
                var existing = await _api.GetCustomerAsync(id.Value);

                if (existing == null)
                    return NotFound();

                Customer = existing;
                ViewData["Title"] = $"Edit: {Customer.FullName}";

            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"Posting customer with ID: {Customer.Id}");

            if (!ModelState.IsValid) return Page();

            if (Customer.Id == 0)
                await _api.CreateCustomerAsync(Customer);
            else
                await _api.UpdateCustomerAsync(Customer.Id, Customer);

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (Customer.Id > 0)
                await _api.DeleteCustomerAsync(Customer.Id);

            return RedirectToPage("Index");
        }
    }
}
