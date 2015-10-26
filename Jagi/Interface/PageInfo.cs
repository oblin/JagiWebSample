using System;

namespace Jagi.Interface
{
    public class PageInfo
    {
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public string SearchKeyword { get; set; }
        public string SortField { get; set; }
        public bool SortDesc { get; set; }
    }
}
