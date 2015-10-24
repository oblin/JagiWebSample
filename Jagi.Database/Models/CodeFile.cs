using System;
using System.Collections.Generic;

namespace Jagi.Database.Models
{
    public class CodeFile
    {
        public int ID { get; set; }
        public string ITEM_TYPE { get; set; }
        public string TYPE_NAME { get; set; }
        public string DESC_1 { get; set; }
        public string PARENT_TYPE { get; set; }
        public string PARENT_CODE { get; set; }
        public string C_REMARK { get; set; }
        public int ModifyFlag { get; set; }
        public int CHAR_NUM { get; set; }
        public virtual IList<CodeDetail> CodeDetails { get; set; }
    }
}