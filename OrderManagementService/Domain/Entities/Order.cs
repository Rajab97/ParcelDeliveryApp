using SharedLibrary.Domain.Entities;

namespace OrderManagementService.Domain.Entities
{
    public class Order : AuditEntity
    {
        public string OrderNumber { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string OrderStatus { get; set; }
        public string AssignedBy { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
