using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class Pagination<T> where T : class
    {
        public Pagination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }

        public int PageIndex { get; set; } // current page number
        public int PageSize { get; set; } // number of items per page
        public int Count { get; set; } // number of items after filter, select
        public IReadOnlyList<T> Data { get; set; }
    }
}