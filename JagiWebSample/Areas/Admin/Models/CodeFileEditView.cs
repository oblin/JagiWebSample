using Jagi.Mapping;
using System.Collections.Generic;
using System.ComponentModel;

namespace JagiWebSample.Areas.Admin.Models
{
    public class CodeFileListView
    {
        public CodeFilesEditView Code { get; set; }
        public IEnumerable<CodeDetailEditView> CodeDetailView { get; set; }
    }

    public class CodeFilesEditView : IMapFromCustomized
    {
        public int Id { get; set; }
        [DisplayName("代號類別")]
        public string ItemType { get; set; }
        [DisplayName("類別名稱")]
        public string TypeName { get; set; }
        [DisplayName("代號說明1")]
        public string Desc { get; set; }
        [DisplayName("父項類別")]
        public string ParentType { get; set; }
        [DisplayName("父項代碼")]
        public string ParentCode { get; set; }
        [DisplayName("代碼位數")]
        public int CharNumber { get; set; }

        public void CreateMappings(AutoMapper.IConfiguration configuration)
        {
            configuration.CreateMap<CodeFile, CodeFilesEditView>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ID))
                .ForMember(d => d.ItemType, opt => opt.MapFrom(s => s.ITEM_TYPE))
                .ForMember(d => d.TypeName, opt => opt.MapFrom(s => s.TYPE_NAME))
                .ForMember(d => d.Desc, opt => opt.MapFrom(s => s.DESC_1))
                .ForMember(d => d.ParentType, opt => opt.MapFrom(s => s.PARENT_TYPE))
                .ForMember(d => d.ParentCode, opt => opt.MapFrom(s => s.PARENT_CODE))
                .ForMember(d => d.CharNumber, opt => opt.MapFrom(s => s.CHAR_NUM));
        }
    }


    public class CodeDetailEditView : IMapFromCustomized
    {
        public int Id { get; set; }
        public int CodeFileID { get; set; }
        public string ItemCode { get; set; }
        public string Desc { get; set; }

        public void CreateMappings(AutoMapper.IConfiguration configuration)
        {
            configuration.CreateMap<CodeDetail, CodeDetailEditView>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ID))
                .ForMember(d => d.ItemCode, opt => opt.MapFrom(s => s.ITEM_CODE))
                .ForMember(d => d.Desc, opt => opt.MapFrom(s => s.DESC));
        }
    }
}