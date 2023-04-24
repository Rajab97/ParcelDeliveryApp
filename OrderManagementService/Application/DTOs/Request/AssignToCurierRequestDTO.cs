namespace OrderManagementService.Application.DTOs.Request
{
    public class AssignToCurierRequestDTO : IValidatableDTO
    {
        public int OrderId { get; set; }
        public int CourierId { get; set; }
    }
}
