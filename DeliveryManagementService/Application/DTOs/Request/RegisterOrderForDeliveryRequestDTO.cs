namespace DeliveryManagementService.Application.DTOs.Request
{
    public class RegisterOrderForDeliveryRequestDTO
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
    }
}
