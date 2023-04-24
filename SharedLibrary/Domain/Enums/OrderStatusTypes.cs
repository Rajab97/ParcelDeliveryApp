using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Domain.Enums
{
    public enum OrderStatusTypes
    {
        //When order created
        Pending,
        //When admin assign it to couries
        InProgress,
        //When courier start to shipment
        Shipped,
        //When order delivered to customer
        Delivered,
        //When user cancel the order
        Canceled
    }

    public static class OrderStatusFlow
    {
        //Dictionary key kimi gosterilen statusa hansi statuslardan kecid oluna bildiyini gosterir
        private static Dictionary<OrderStatusTypes, List<OrderStatusTypes>> StatusFlow;
        static OrderStatusFlow()
        {
            StatusFlow = new Dictionary<OrderStatusTypes, List<OrderStatusTypes>>() {
                { OrderStatusTypes.Pending, new List<OrderStatusTypes>() { } },
                { OrderStatusTypes.InProgress, new List<OrderStatusTypes>() { OrderStatusTypes.Pending } },
                { OrderStatusTypes.Shipped, new List<OrderStatusTypes>() { OrderStatusTypes.InProgress } },
                { OrderStatusTypes.Delivered, new List<OrderStatusTypes>() { OrderStatusTypes.Shipped } },
                { OrderStatusTypes.Canceled, new List<OrderStatusTypes>() { OrderStatusTypes.Pending, OrderStatusTypes.InProgress, OrderStatusTypes.Shipped} },
            };
        }
        public static bool IsStatusExist(string status)
        {
            if (Enum.TryParse(status,false,out OrderStatusTypes statusEnum))
                return true;
            return false;
        }
        public static bool IsTransitionAllowed(string from , string to)
        {
            if (Enum.TryParse(from,true,out OrderStatusTypes fromEnum) && Enum.TryParse(to, true, out OrderStatusTypes toEnum))
            {
                if (StatusFlow.ContainsKey(toEnum))
                {
                    if (StatusFlow[toEnum].Any(m => m == fromEnum))
                        return true;
                    else
                        return false;
                }
                else
                    throw new ApplicationException("Transition for order not Exist");
            }
            else
                throw new ApplicationException("Cannot parse status to enum type");
        }
    }
}
