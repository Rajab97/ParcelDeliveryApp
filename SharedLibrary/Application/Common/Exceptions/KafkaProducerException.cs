using SharedLibrary.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Application.Common.Exceptions
{
    public class KafkaProducerException : ApplicationException
    {
        public KafkaProducerException() : base(ExceptionMessages.MessageNotProduced)
        {

        }
    }
}
