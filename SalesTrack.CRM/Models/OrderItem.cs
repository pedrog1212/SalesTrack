using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesTrack.CRM.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        //---------

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        //---------

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Inventory Inventory { get; set; }
        //---------
        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public string ProductName { get; set; } = string.Empty; // only for display

        [NotMapped]
        public string SKU { get; set; } = string.Empty;

        [NotMapped]
        public decimal Total => Quantity * UnitPrice;

    }

}
