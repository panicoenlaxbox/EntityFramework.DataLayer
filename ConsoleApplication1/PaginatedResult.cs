using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(int pageIndex, int pageSize, IEnumerable<T> result, int totalCount)
        {
            Result = result;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public IEnumerable<T> Result { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public int TotalPageCount
        {
            get
            {
                return (int)Math.Ceiling(TotalCount / (double)PageSize);
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return PageIndex < TotalPageCount;
            }
        }
    }
}