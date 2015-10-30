using Jagi.Interface;
using Jagi.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JagiWebSample.Models
{
    public class PatientListView : Entity, IMapFrom<Patient>
    {
        public string Name { get; set; }
        public string ChartId { get; set; }
        public string IdCard { get; set; }
        public string BirthDay { get; set; }
    }
}