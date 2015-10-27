using System;

namespace Jagi.Interface
{
    public class PageInfo
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SearchKeyword { get; set; }
        public string SortField { get; set; }
        public string Sort { get; set; }    // ASC & DESC
    }
}
