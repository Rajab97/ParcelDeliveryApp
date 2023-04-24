namespace DeliveryManagementService.Application.DTOs.Request
{
    public class IsCancelationAllowedRequestDTO : IValidatableDTO
    {
        public string OrderNumber { get; set; }
    }
}
