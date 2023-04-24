namespace OrderManagementService.Application.DTOs.Response
{
    public class GetOrdersResponseDTO
    {
        public string OrderNumber { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<GetOrdersItemResponseDTO> OrderItems { get; set; }
    }

    public class GetOrdersItemResponseDTO
    {
        public string ProductNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
