using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.DTOs.Response
{
    public class CreateOrderResponseDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string OrderStatus { get; set; }
        public string AssignedBy { get; set; }
        public IEnumerable<CreateOrderItemResponseDTO> OrderItems { get; set; }
    }

    public class CreateOrderItemResponseDTO
    {
        public string ProductNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
