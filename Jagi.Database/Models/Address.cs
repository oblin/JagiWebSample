using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Database.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string ZIP { get; set; }

        public string County { get; set; }

        public string REALM { get; set; }

        public string STREET { get; set; }
    }
}
