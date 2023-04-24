namespace DeliveryManagementService.Application.DTOs.Response
{
    public class IsCancelationAllowedResponseDTO
    {
        public bool IsAllowed { get; set; }
        public string CurrentDeliveryStatus { get; set; }
    }
}
