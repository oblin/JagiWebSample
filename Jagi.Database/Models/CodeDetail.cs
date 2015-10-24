using System;
using System.ComponentModel;

namespace Jagi.Database.Models
{
    public class CodeDetail
    {
        public int ID { get; set; }

        public int CodeFileID { get; set; }

        public string ITEM_CODE { get; set; }

        public string DESC { get; set; }

        public bool IsBanned { get; set; }

        public string C_REMARK { get; set; }
    }
}