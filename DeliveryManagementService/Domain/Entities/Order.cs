using SharedLibrary.Domain.Entities;

namespace DeliveryManagementService.Domain.Entities
{
    public class Order : AuditEntity
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int NumberOfItems { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string AssignedBy { get; set; }

        public int CourierId { get; set; }
        public string CourierUserName { get; set; }
        public string CourierFullName { get; set; }
        public string OrderStatus { get; set; }
        public IEnumerable<DeliveryHostory> DeliveryHostories { get; set; }
    }
}
