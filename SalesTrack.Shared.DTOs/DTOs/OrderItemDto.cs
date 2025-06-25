using System.ComponentModel.DataAnnotations;

namespace SalesTrack.Shared.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;   // Product Name also display only
        public string SKU { get; set; } = string.Empty;  // SKU only for display (optional, not required)

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

}


