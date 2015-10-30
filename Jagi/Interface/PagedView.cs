using Jagi.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Interface
{
    public class PagedView
    {
        // Mapping entity name with display name
        public Dictionary<string, string> Headers { get; set; }
        public IEnumerable<object> Data { get; set; }
        public string JsonHeaders
        {
            get
            {
                return this.Headers.ToJson();
            }
        }
        public string JsonData
        {
            get
            {
                return this.Data.ToJson();
            }
        }

        public int PageCount { get; set; }
        public int? CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages
        {
            get
            {
                if (PageCount == 0)
                    return 0;
                return (int)Math.Ceiling((decimal)TotalCount / PageCount);
            }
        }
    }
}
