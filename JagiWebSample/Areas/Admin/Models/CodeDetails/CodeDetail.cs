using Jagi.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JagiWebSample.Areas.Admin.Models
{
    public class CodeDetail : IMapFromCustomized
    {
        [Key]
        public int ID { get; set; }

        public int CodeFileID { get; set; }
        [Required]
        [StringLength(8)]
        [DisplayName("代號")]
        public string ITEM_CODE { get; set; }
        [StringLength(100)]
        [DisplayName("代號說明")]
        public string DESC { get; set; }

        [DisplayName("已被停用")]
        public bool IsBanned { get; set; }
        [StringLength(30)]
        [DisplayName("備註說明")]
        public string C_REMARK { get; set; }

        public DateTime? CreateDateTime { get; set; }
        [StringLength(20)]
        public string CreateLoginName { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        [StringLength(20)]
        public string ModifiedLoginName { get; set; }

        public void CreateMappings(AutoMapper.IConfiguration configuration)
        {
            configuration.CreateMap<CodeDetailEditView, CodeDetail>()
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.ITEM_CODE, opt => opt.MapFrom(s => s.ItemCode))
                .ForMember(d => d.DESC, opt => opt.MapFrom(s => s.Desc));
        }
    }
}