using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagiWebSample.Areas.Admin.Models
{
    public class Address
    {
        public int Id { get; set; }
        [Required, StringLength(5)]
        public string Zip { get; set; }
        [Required, StringLength(6)]
        public string County { get; set; }
        [Required, StringLength(8)]
        public string Realm { get; set; }
        [StringLength(16)]
        public string Street { get; set; }
    }
}
