namespace DeliveryManagementService.Application.DTOs.Request
{
    public class ChangeOrderStatusRequestDTO : IValidatableDTO
    {
        public int Id { get; set; }
        public string OrderStatus { get; set; }
    }
}
