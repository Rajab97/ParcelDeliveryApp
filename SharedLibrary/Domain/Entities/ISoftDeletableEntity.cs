using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Domain.Entities
{
    public interface ISoftDeletableEntity
    {
        bool? IsActive { get; set; }
    }
}
