using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models.KafkaSchemaRegistry
{
    public static class KafkaTopics
    {
        public const string ASSIGN_TO_COURIER = "assign-to-courier";
        public const string ORDER_STATUS_CHANGED = "order-status-changed";
        public const string CANCEL_ORDER = "cancel-order";

        public static IEnumerable<string> GetAvailableTopics()
        {
            yield return ASSIGN_TO_COURIER;
            yield return CANCEL_ORDER;
            yield return ORDER_STATUS_CHANGED;
        }
    }
}
