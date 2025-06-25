using System.ComponentModel.DataAnnotations;

namespace SalesTrack.Shared.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int? CustomerId { get; set; } // now nullable, allows the model binding to match the placeholder when CustomerId is null
        public string? FullName { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public int OrderStatusId { get; set; }
        public string? StatusName { get; set; }

        public int OrderTypeId { get; set; }
        public string? TypeName { get; set; }

       // public int TotalItems { get; set; }  // Optional: calculated in frontend or mapped from API

    }
}
