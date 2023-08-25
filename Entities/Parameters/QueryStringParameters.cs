using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Parameters
{
    public abstract class QueryStringParameters
    {
        const int maxPageSize = 50;
        private int? _pageSize = null;
        public int? PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value <= maxPageSize) ? value : maxPageSize; }
        }
        public int PageNumber { get; set; } = 1;
        public string OrderBy { get; set; }
    }
}
