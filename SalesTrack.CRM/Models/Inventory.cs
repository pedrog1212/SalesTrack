using System.ComponentModel.DataAnnotations;

namespace SalesTrack.CRM.Models
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; }

        [Required]
        public int QuantityAvailable { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
    }
}
