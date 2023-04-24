namespace OrderManagementService.Application.DTOs.Request
{
    public class CancelOrderRequestDTO : IValidatableDTO
    {
        public int OrderId { get; set; }
    }
}
