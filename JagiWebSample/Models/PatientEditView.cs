using Jagi.Interface;
using Jagi.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JagiWebSample.Models
{
    public class PatientEditView : Entity, IMapFrom<Patient>
    {
        public string Name { get; set; }
        public string ChartId { get; set; }
        public string IdCard { get; set; }
        public string BirthDate { get; set; }
        public string Sex { get; set; }
        public string Marry { get; set; }
        public string CureDoc { get; set; }

        // Address
        public string County0 { get; set; }
        public string County { get; set; }
        public string Realm { get; set; }
        public string Mailno { get; set; }
        public string Village { get; set; }
        public string Street { get; set; }

        public string Idiopa { get; set; }
        public string Idiopathy { get; set; }
        public string Status { get; set; }


        // 健保、自費
        public string Type { get; set; }
        public string Educate { get; set; }
        // 職業
        public string Vocation { get; set; }
        // 聯絡人
        public string Father { get; set; }
        // 關係
        public string Relation { get; set; }
        // 血型
        public string Ab { get; set; }
    }
}