using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput.Messages;
using SalesTrack.CRM.Data;
using SalesTrack.CRM.Models;
using SalesTrack.CRM.Services;
using SalesTrack.Shared.DTOs;

namespace SalesTrack.CRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemsController : ControllerBase
    {
        private readonly CrmDbContext _context;
        private readonly ILogger<OrderItemsController> _logger;
        private readonly OrderInventoryService _inventoryService;

        public OrderItemsController(CrmDbContext context, ILogger<OrderItemsController> logger, OrderInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _inventoryService = inventoryService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetItemsForOrder(int orderId)
        {
            var items = await _context.OrderItems
                .Include(i => i.Inventory)  // join with Inventory via FK ProductId
                .Where(i => i.OrderId == orderId)
                .Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    OrderId = i.OrderId,
                    ProductId = i.ProductId,
                    ProductName = i.Inventory.ProductName, // fill ProductName from Inventory
                    SKU = i.Inventory.SKU,                 // fill SKU from Inventory table
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToListAsync();

            return Ok(items);
        }


        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] OrderItemDto dto)
        {
            _logger.LogInformation("POST /api/orderitems - Payload: {@dto}", dto);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var transaction = await _context.Database.BeginTransactionAsync();   // Begin Transaction

            try
            {
                var item = new OrderItem
                {
                    OrderId = dto.OrderId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = dto.UnitPrice
                };


                _context.OrderItems.Add(item);
                // Deduct inventory according to quantity in order
                await _inventoryService.DeductInventoryAsync(item.ProductId, item.Quantity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();   // commit changes
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();    // Rollback changes

                _logger.LogError(ex, "Error saving item");
                return StatusCode(500, ex.Message);
            }
        }      

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] OrderItemDto dto)
        {
            _logger.LogInformation("PUT /api/orderitems/{id} - Payload: {@dto}", id, dto);

            if (!ModelState.IsValid)  return BadRequest(ModelState);

            using var transaction = await _context.Database.BeginTransactionAsync();  // Begin Transaction

            try
            {
                var existing = await _context.OrderItems.FindAsync(id);
                if (existing == null) return NotFound();

                if (existing.ProductId != dto.ProductId)
                    return BadRequest("Changing product is not allowed.");

                // Adjust inventory according to quantity difference
                await _inventoryService.AdjustInventoryAsync(existing.ProductId, existing.Quantity, dto.Quantity);

                //existing.ProductId = dto.ProductId;
                existing.Quantity = dto.Quantity;
                existing.UnitPrice = dto.UnitPrice;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();   // commit changes
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();    // Rollback changes

                _logger.LogError(ex, "Error updating item");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            _logger.LogInformation("DELETE /api/orderitems/{id}", id);

            using var transaction = await _context.Database.BeginTransactionAsync();  // Begin Transaction

            try
            {
                var item = await _context.OrderItems.FindAsync(id);
                if (item == null) return NotFound();

                // Restock inventory according to deleted order quantity 
                await _inventoryService.ReturnInventoryAsync(item.ProductId, item.Quantity);
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();   // commit changes
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();    // Rollback changes

                _logger.LogError(ex, "Error deleting item");
                return StatusCode(500, ex.Message);
            }
        }

    }
}
