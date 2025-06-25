using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesTrack.CRM.Data;
using SalesTrack.Shared.DTOs;
using SalesTrack.WebFrontend.Services;

namespace SalesTrack.WebFrontend.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly OrderApiClient _orderApi;
        private readonly CustomerApiClient _customerApi;
        private readonly OrderStatusApiClient _orderStatusApi;
        private readonly OrderTypeApiClient _orderTypeApi;
        private readonly CrmDbContext _context;

        public IndexModel(
            OrderApiClient orderApi,
            CustomerApiClient customerApi,
            OrderStatusApiClient orderStatusApi,
            OrderTypeApiClient orderTypeApi,
            CrmDbContext context)
        {
            _orderApi = orderApi;
            _customerApi = customerApi;
            _orderStatusApi = orderStatusApi;
            _orderTypeApi = orderTypeApi;
            _context = context;
        }

        public List<OrderDto> SalesOrders { get; set; } = new();
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> OrderStatuses { get; set; } = new();
        public List<SelectListItem> OrderTypes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? SelectedCustomerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedOrderId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedFullName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedOrderStatusName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedOrderTypeName { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedOrderDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDeliveryDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedStatusId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedTypeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "OrderDate";

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "asc";

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 0;
        public bool HasNextPage => PageIndex + 1 < TotalPages;
        public async Task OnGetAsync()
        {
            // populate dropdowns
            Customers = (await _customerApi.GetCustomersAsync())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName })
                .ToList();

            OrderStatuses = (await _orderStatusApi.GetOrderStatusesAsync())
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StatusName })
                .ToList();

            OrderTypes = (await _orderTypeApi.GetOrderTypesAsync())
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.TypeName })
                .ToList();

            // query from DB
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderType)
                .AsQueryable();

            // Filtering first using entity properties
            if (SelectedCustomerId.HasValue)
                query = query.Where(o => o.CustomerId == SelectedCustomerId.Value);

            if (SelectedStatusId.HasValue)
                query = query.Where(o => o.OrderStatusId == SelectedStatusId.Value);

            if (SelectedTypeId.HasValue)
                query = query.Where(o => o.OrderTypeId == SelectedTypeId.Value);

            // Then project to DTO
            var projectedQuery = query.Select(o => new OrderDto
            {
                Id = o.Id,
                FullName = o.Customer.FullName,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                OrderStatusId = o.OrderStatusId,
                StatusName = o.OrderStatus.StatusName,
                OrderTypeId = o.OrderTypeId,
                TypeName = o.OrderType.TypeName
            });
            /*
            // Filter after projecting to DTO
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string lowerTerm = SearchTerm.ToLower();

                projectedQuery = projectedQuery.Where(o =>
                    o.Id.ToString().Contains(lowerTerm) ||
                    o.FullName.ToLower().Contains(lowerTerm) ||
                    o.StatusName.ToLower().Contains(lowerTerm) ||
                    o.TypeName.ToLower().Contains(lowerTerm) ||
                    o.OrderDate.ToString("MM/dd/yyyy").Contains(lowerTerm) ||
                    (o.DeliveryDate.HasValue && o.DeliveryDate.Value.ToString("MM/dd/yyyy").Contains(lowerTerm))
                );
            }   
            */
            // apply sorting on the DTO query
            projectedQuery = SortColumn switch
            {
                "FullName" => SortDirection == "asc"
                    ? projectedQuery.OrderBy(o => o.FullName)
                    : projectedQuery.OrderByDescending(o => o.FullName),
                "OrderDate" => SortDirection == "asc"
                    ? projectedQuery.OrderBy(o => o.OrderDate)
                    : projectedQuery.OrderByDescending(o => o.OrderDate),
                "DeliveryDate" => SortDirection == "asc"
                    ? projectedQuery.OrderBy(o => o.DeliveryDate)
                    : projectedQuery.OrderByDescending(o => o.DeliveryDate),
                "StatusName" => SortDirection == "asc"
                    ? projectedQuery.OrderBy(o => o.StatusName)
                    : projectedQuery.OrderByDescending(o => o.StatusName),
                "TypeName" => SortDirection == "asc"
                    ? projectedQuery.OrderBy(o => o.TypeName)
                    : projectedQuery.OrderByDescending(o => o.TypeName),
                _ => SortDirection == "asc"
                    ? projectedQuery.OrderBy(o => o.Id)
                    : projectedQuery.OrderByDescending(o => o.Id)
            };

            // Execute the query (materialize)
            var list = await projectedQuery.ToListAsync();

            // Apply filtering in memory
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string lowerTerm = SearchTerm.ToLower();

                list = list.Where(o =>
                    o.Id.ToString().Contains(lowerTerm) ||
                    (o.FullName?.ToLower().Contains(lowerTerm) ?? false) ||
                    (o.StatusName?.ToLower().Contains(lowerTerm) ?? false) ||
                    (o.TypeName?.ToLower().Contains(lowerTerm) ?? false) ||
                    o.OrderDate.ToString().Contains(lowerTerm) ||
                    (o.DeliveryDate.HasValue && o.DeliveryDate.Value.ToString().Contains(lowerTerm))
                ).ToList();
            }

            // pagination
            SalesOrders = list;

            int pageSize = 12;
            int totalCount = list.Count;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // clamp PageIndex
            PageIndex = Math.Max(1, PageIndex);
            PageIndex = Math.Min(PageIndex, TotalPages > 0 ? TotalPages : 1);

            // apply pagination on filtered, sorted list
            SalesOrders = list
                .Skip((PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
