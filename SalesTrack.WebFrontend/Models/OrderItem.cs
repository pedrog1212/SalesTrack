using System.ComponentModel.DataAnnotations.Schema;

namespace SalesTrack.WebFrontend.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        //public string ProductName { get; set; } = string.Empty; // only for display
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty; // only for display
        public string? SKU { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
