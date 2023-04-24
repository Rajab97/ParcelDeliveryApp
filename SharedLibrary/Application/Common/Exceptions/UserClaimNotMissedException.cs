using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Application.Common.Exceptions
{
    public class UserClaimNotMissedException : ApplicationException
    {
        public UserClaimNotMissedException() : base("Generate new JWT token. Your claims missed")
        {

        }
    }
}
