using Jagi.Database.Mvc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JagiWebSample.Utility
{
    public class EntityHtmlTag : ColumnsHtmlTag
    {
        protected override Dictionary<string, string> GetTableOptions(string valueforTable, string valueforKey, string valueforValue, string valueFor)
        {
            var tableValue = ServiceLocator.Current.GetInstance<TableValue>();
            if (tableValue == null)
                return null;
            return tableValue.GetCodeDetail(valueforTable, valueforKey, valueforValue, valueFor);
        }
    }
}