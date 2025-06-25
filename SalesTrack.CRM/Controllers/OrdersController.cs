using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesTrack.CRM.Data;
using SalesTrack.CRM.Models;
using SalesTrack.Shared.DTOs;

namespace SalesTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public OrdersController(CrmDbContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders() 
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)    // SQL JOIN this enables access to Customer FullName
                .Include(o => o.OrderStatus) // SQL JOIN this enables access to StatusName
                .Include(o => o.OrderType)   // SQL JOIN this enables access to OrderType 

                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    FullName = o.Customer.FullName,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    OrderStatusId = o.OrderStatusId,
                    StatusName = o.OrderStatus.StatusName,
                    OrderTypeId = o.OrderTypeId
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)    // SQL JOIN this enables access to Customer FullName
                .Include(o => o.OrderStatus) // SQL JOIN this enables access to StatusName
                .Include(o => o.OrderType)   // SQL JOIN this enables access to OrderType 

                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var dto = new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                FullName = order.Customer?.FullName,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                OrderStatusId = order.OrderStatusId,
                OrderTypeId = order.OrderTypeId
            };

            return Ok(dto);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> PostOrder(OrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = new Order
            {
                CustomerId = dto.CustomerId ?? 0,

                // Convert C# DateTime to UTC in controller or before saving
                OrderDate = DateTime.SpecifyKind(dto.OrderDate, DateTimeKind.Utc),
                DeliveryDate = dto.DeliveryDate.HasValue
                    ? DateTime.SpecifyKind(dto.DeliveryDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null,

                OrderStatusId = dto.OrderStatusId,
                // optional: check required fields
                OrderTypeId = dto.OrderTypeId,

            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            dto.Id = order.Id;
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, dto);
        }

        // PUT: api/orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDto dto)
        {
            try
            { 
                if (id != dto.Id)
                return BadRequest("Order ID mismatch");

                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return NotFound();

                // Update fields
                order.CustomerId = dto.CustomerId ?? 0;

                // Convert C# DateTime to UTC in controller or before saving
                order.OrderDate = DateTime.SpecifyKind(dto.OrderDate, DateTimeKind.Utc);
                order.DeliveryDate = dto.DeliveryDate.HasValue
                    ? DateTime.SpecifyKind(dto.DeliveryDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;

                order.OrderStatusId = dto.OrderStatusId;
                // optional: check required fields
                order.OrderTypeId = dto.OrderTypeId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                // log and return database-specific errors
                Console.WriteLine("DB ERROR in PUT Order: " + dbEx.ToString());
                return StatusCode(500, "A database error occurred while saving the order.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("GENERIC ERROR in PUT Order: " + ex.ToString());
                return StatusCode(500, "An error occurred while saving the order.");
            }
        }
        

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}


