using SharedLibrary.Domain.Entities;
using SharedLibrary.Domain.Enums;

namespace OrderManagementService.Domain.Entities
{
    public class OutBoxMessage : BaseEntity
    {
        public string Topic { get; set; }
        public string Message { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Status { get; set; }
        public OutBoxMessage()
        {
            Status = OutBoxStatusTypes.Pending.ToString();
            ReceivedDate = DateTime.Now;
        }
    }
}
