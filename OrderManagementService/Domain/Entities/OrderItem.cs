using SharedLibrary.Domain.Entities;

namespace OrderManagementService.Domain.Entities
{
    public class OrderItem:BaseEntity
    {
        public string ProductNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
    }
}
