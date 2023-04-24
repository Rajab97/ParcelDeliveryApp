namespace OrderManagementService.Application.DTOs.Request
{
    public class ChangeOrderAddressRequestDTO : IValidatableDTO
    {
        public int OrderId { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
    }
}
