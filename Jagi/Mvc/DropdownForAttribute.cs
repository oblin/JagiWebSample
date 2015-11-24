using System;
using System.Web.Mvc;

namespace Jagi.Mvc
{
    public class DropdownAttribute : Attribute, IMetadataAware
    {
        public string CodeMap { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP] = this.CodeMap;
        }
    }

    public class DropdownForAttribute : Attribute, IMetadataAware
    {
        public string CodeMap { get; set; }
        public string CodeMapFor { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP] = this.CodeMap;
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP_FOR] = this.CodeMapFor;
        }
    }
}
