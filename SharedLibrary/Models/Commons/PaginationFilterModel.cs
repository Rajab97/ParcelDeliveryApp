using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models.Commons
{
    public class PaginationFilterModel
    {
        private int defaultPageSize = 10;

        private int pageIndex;
        private int pageSize;

        public int PageIndex
        {
            get
            {
                if (pageIndex <= 0) return 1;
                else return pageIndex;
            }
            set
            {
                if (value <= 0) pageIndex = 1;
                else pageIndex = value;
            }
        }
        public int PageSize
        {
            get
            {
                if (pageSize <= 0) return defaultPageSize;
                else return pageSize;
            }
            set
            {
                if (value <= 0) pageSize = defaultPageSize;
                else pageSize = value;
            }
        }

        public PaginationFilterModel()
        {
        }

        public PaginationFilterModel(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
