using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models.KafkaSchemaRegistry
{
    public class OrderStatusChangedMessage
    {
        public string OrderNumber { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public string OrderStatus { get; set; }
    }
}
