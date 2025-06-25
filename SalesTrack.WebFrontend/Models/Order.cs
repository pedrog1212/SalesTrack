using System;

namespace SalesTrack.WebFrontend.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public string FullName { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public int OrderStatusId { get; set; }
        public string StatusName { get; set; }

        public int OrderTypeId { get; set; }
        public string TypeName { get; set; }
       // public int TotalItems { get; set; }  // Optional: calculated in frontend or mapped from API
    }
}
