using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesTrack.CRM.Data;
using SalesTrack.CRM.Models;
using SalesTrack.Shared.DTOs;

namespace SalesTrack.CRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public InventoryController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetInventoryAll()
        {
            var items = await _context.Inventory
                .Select(i => new InventoryDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    SKU = i.SKU,
                    QuantityAvailable = i.QuantityAvailable,
                    UnitPrice = i.UnitPrice
                }).ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryDto>> GetInventoryById(int id)
        {
            var i = await _context.Inventory.FindAsync(id);
            if (i == null) return NotFound();

            return new InventoryDto
            {
                Id = i.Id,
                ProductName = i.ProductName,
                SKU = i.SKU,
                QuantityAvailable = i.QuantityAvailable,
                UnitPrice = i.UnitPrice
            };
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory(InventoryDto dto)
        {
            var item = new Inventory
            {
                ProductName = dto.ProductName,
                SKU = dto.SKU,
                QuantityAvailable = dto.QuantityAvailable,
                UnitPrice = dto.UnitPrice
            };

            _context.Inventory.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventoryById), new { id = item.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, InventoryDto dto)
        {
            var item = await _context.Inventory.FindAsync(id);
            if (item == null) return NotFound();

            item.ProductName = dto.ProductName;
            item.SKU = dto.SKU;
            item.QuantityAvailable = dto.QuantityAvailable;
            item.UnitPrice = dto.UnitPrice;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var item = await _context.Inventory.FindAsync(id);
            if (item == null) return NotFound();

            _context.Inventory.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
