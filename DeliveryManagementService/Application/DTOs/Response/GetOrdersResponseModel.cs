namespace DeliveryManagementService.Application.DTOs.Response
{
    public class GetOrdersResponseModel
    {
        public string OrderNumber { get; set; }
        public int NumberOfItems { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string CustomerFullName { get; set; }
        public string AssignedBy { get; set; }
        public string CourierUserName { get; set; }
        public string CourierFullName { get; set; }
        public string CurrentStatus { get; set; }
        public IEnumerable<GetOrdersHistoryItemResponseModel> OrderStatusHistories { get; set; }
    }

    public class GetOrdersHistoryItemResponseModel
    {
        public string Status { get; set; }
        public DateTime EventTime { get; set; }
    }
}
