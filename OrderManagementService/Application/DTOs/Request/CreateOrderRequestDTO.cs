namespace OrderManagementService.Application.DTOs.Request
{
    public class CreateOrderRequestDTO : IValidatableDTO
    {
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public List<CreateOrderItemRequestDTO> OrderItems { get; set; }
    }

    public class CreateOrderItemRequestDTO : IValidatableDTO
    {
        public string ProductNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
