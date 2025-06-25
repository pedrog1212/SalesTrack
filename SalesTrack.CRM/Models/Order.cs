using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesTrack.CRM.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [NotMapped]
        public string FullName => Customer?.FullName;

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Required]
        public int OrderStatusId { get; set; }

        [ForeignKey("OrderStatusId")]
        public OrderStatus OrderStatus { get; set; }
        
        [NotMapped]
        public string StatusName => OrderStatus?.StatusName;
        
        [Required]
        public int OrderTypeId { get; set; }

        [ForeignKey("OrderTypeId")]
        public OrderType OrderType { get; set; }

        [NotMapped]
        public string TypeName => OrderType?.TypeName;

        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
