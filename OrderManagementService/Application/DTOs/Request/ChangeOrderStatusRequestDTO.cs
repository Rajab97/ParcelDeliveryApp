namespace OrderManagementService.Application.DTOs.Request
{
    public class ChangeOrderStatusRequestDTO
    {
        public string OrderNumber { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public string OrderStatus { get; set; }
    }
}
