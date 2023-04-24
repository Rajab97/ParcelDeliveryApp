using SharedLibrary.Domain.Entities;

namespace DeliveryManagementService.Domain.Entities
{
    public class DeliveryHostory:BaseEntity
    {
        public string Status { get; set; }
        public DateTime EventTime { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
