using Microsoft.EntityFrameworkCore;
using SalesTrack.CRM.Data;
using SalesTrack.CRM.Models;

namespace SalesTrack.CRM.Services
{
    public class OrderInventoryService
    {
        private readonly CrmDbContext _context;
        private readonly ILogger<OrderInventoryService> _logger;

        public OrderInventoryService(CrmDbContext context, ILogger<OrderInventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task DeductInventoryAsync(int productId, int quantity)
        {
            var inventory = await LockInventoryAsync(productId);
            inventory.QuantityAvailable -= quantity;

            if (inventory.QuantityAvailable < 0)
            {
                _logger.LogWarning("Inventory for ProductId {ProductId} is now negative: {QuantityAvailable}", productId, inventory.QuantityAvailable);
            }
        }

        public async Task ReturnInventoryAsync(int productId, int quantity)
        {
            var inventory = await LockInventoryAsync(productId);
            inventory.QuantityAvailable += quantity;
        }

        public async Task AdjustInventoryAsync(int productId, int oldQuantity, int newQuantity)
        {
            int delta = newQuantity - oldQuantity;

            if (delta > 0)
                await DeductInventoryAsync(productId, delta);
            else if (delta < 0)
                await ReturnInventoryAsync(productId, -delta);
        }

        private async Task<Inventory> LockInventoryAsync(int productId)
        {
            var inventory = await _context.Inventory
                .FromSqlRaw("SELECT * FROM \"Inventory\" WHERE \"Id\" = {0} FOR UPDATE", productId)
                .FirstOrDefaultAsync();

            if (inventory == null)
                throw new Exception($"ProductId {productId} not found in Inventory.");

            return inventory;
        }
    }
}
