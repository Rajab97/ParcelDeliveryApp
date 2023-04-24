namespace OrderManagementService.Application.Models.ExternalServiceModels
{
    public class IsCancelationAllowedResponseDTO
    {
        public bool IsAllowed { get; set; }
        public string CurrentDeliveryStatus { get; set; }
    }
}
