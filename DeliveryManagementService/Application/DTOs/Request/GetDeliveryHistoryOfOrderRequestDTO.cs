namespace DeliveryManagementService.Application.DTOs.Request
{
    public class GetDeliveryHistoryOfOrderRequestDTO : IValidatableDTO
    {
        public string OrderNumber { get; set; }
    }
}
