namespace SalesTrack.WebFrontend.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int QuantityAvailable { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
