using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models.KafkaSchemaRegistry
{
    public class CancelOrderMessage
    {
        public string OrderNumber { get; set; }
    }
}
