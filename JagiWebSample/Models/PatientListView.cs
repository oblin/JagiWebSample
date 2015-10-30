using Jagi.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JagiWebSample.Models
{
    public class PatientListView : IMapFrom<Patient>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ChartId { get; set; }
        public string IdCard { get; set; }
        public string Birthday { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedUser { get; set; }
    }
}