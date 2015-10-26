using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Interface
{
    public class PagedView
    {
        public IEnumerable<string> Headers { get; set; }
        public IEnumerable<IDictionary<string, object>> List { get; set; }

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
