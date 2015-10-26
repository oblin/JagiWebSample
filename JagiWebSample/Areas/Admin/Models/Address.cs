using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagiWebSample.Areas.Admin.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string Zip { get; set; }

        public string County { get; set; }

        public string Realm { get; set; }

        public string Street { get; set; }
    }
}
