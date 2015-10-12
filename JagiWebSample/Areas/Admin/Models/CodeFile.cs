using AutoMapper;
using Jagi.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JagiWebSample.Areas.Admin.Models
{
    public class CodeFile : IMapFromCustomized
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        [DisplayName("代號類別")]
        public string ITEM_TYPE { get; set; }
        [Required]
        [StringLength(30)]
        [DisplayName("類別名稱")]
        public string TYPE_NAME { get; set; }
        [StringLength(30)]
        [DisplayName("代號說明1")]
        public string DESC_1 { get; set; }
        [StringLength(30)]
        [DisplayName("父項類別")]
        public string PARENT_TYPE { get; set; }
        [DisplayName("父項代碼")]
        public string PARENT_CODE { get; set; }
        [StringLength(30)]
        [DisplayName("備註說明(表頭)")]
        public string C_REMARK { get; set; }

        [Required]
        public ModifyFlag ModifyFlag { get; set; }

        [Required]
        [DisplayName("代碼位數")]
        public int CHAR_NUM { get; set; }

        public virtual IList<CodeDetail> CodeDetails { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? CreateDateTime { get; set; }
        [ScaffoldColumn(false)]
        [StringLength(20)]
        public string CreateLoginName { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? ModifiedDateTime { get; set; }
        [ScaffoldColumn(false)]
        [StringLength(20)]
        public string ModifiedLoginName { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<CodeFilesEditView, CodeFile>()
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.ITEM_TYPE, opt => opt.MapFrom(s => s.ItemType))
                .ForMember(d => d.TYPE_NAME, opt => opt.MapFrom(s => s.TypeName))
                .ForMember(d => d.DESC_1, opt => opt.MapFrom(s => s.Desc))
                .ForMember(d => d.PARENT_TYPE, opt => opt.MapFrom(s => s.ParentType))
                .ForMember(d => d.PARENT_CODE, opt => opt.MapFrom(s => s.ParentCode))
                .ForMember(d => d.CHAR_NUM, opt => opt.MapFrom(s => s.CharNumber));
        }
    }

    public enum ModifyFlag : int
    {
        [Display(Name = "可修改")]
        Yes,
        [Display(Name = "不可修改")]
        No,
        [Display(Name = "隱藏")]
        Hidden,
        [Display(Name = "僅能新增")]
        InsertOnly
    }
}