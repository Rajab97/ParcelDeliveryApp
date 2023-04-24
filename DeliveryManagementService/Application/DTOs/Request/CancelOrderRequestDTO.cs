namespace DeliveryManagementService.Application.DTOs.Request
{
    public class CancelOrderRequestDTO : IValidatableDTO
    {
        public string OrderNumber { get; set; }
    }
}
