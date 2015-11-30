using System;
using System.Collections.Generic;

namespace Jagi.Interface
{
    public class PagedView
    {
        // Mapping entity name with display name
        public Dictionary<string, string> Headers { get; set; }
        public IEnumerable<object> Data { get; set; }

        public string SearchField { get; set; }
        public string SearchKeyword { get; set; }

        public string SortField { get; set; }
        public string Sort { get; set; }    // ASC & DESC

        public int PageSize { get; set; }
        public int? PageNumber { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages
        {
            get
            {
                if (PageSize == 0)
                    return 0;
                return (int)Math.Ceiling((decimal)TotalCount / PageSize);
            }
        }
    }
}
