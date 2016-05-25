using System;
using System.Web.Mvc;

namespace Jagi.Mvc
{
    public class DropdownAttribute : Attribute, IMetadataAware
    {
        public string CodeMap { get; set; }
        public bool EmptyField { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP] = this.CodeMap;
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP_EMPTY_SELECTION] = this.EmptyField;
        }
    }

    public class DropdownForAttribute : Attribute, IMetadataAware
    {
        public string CodeMap { get; set; }
        public string CodeMapFor { get; set; }
        public bool EmptyField { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP] = this.CodeMap;
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP_FOR] = this.CodeMapFor;
            metadata.AdditionalValues[ConstantString.ADDITIONAL_VALUES_CODEMAP_EMPTY_SELECTION] = this.EmptyField;
        }
    }
}
