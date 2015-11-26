using Microsoft.Web.Mvc;
using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Jagi.Mvc.Helpers
{
    public static class JHtmlHelper
    {
        /// <summary>
        /// 如果使用 int 且多個參數時候（例如：c.GetPaged(null, 0)），會出現以下這樣的連結：
        /// /HdPlans/GetPaged?patientId=0
        /// 修改為： /HdPlans/GetPaged?patientId=
        /// 使用方式為：
        ///     var url = model.getPagedUrls + patientId
        ///     dataService.get(url, pararms, function(){});
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="helper"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string BuildUrlFromExpression<TController>
            (this HtmlHelper helper, Expression<Action<TController>> action)
            where TController : Controller
        {
            var url = LinkExtensions.BuildUrlFromExpression<TController>(helper, action);
            if (url.EndsWith("/0"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            else if (url.EndsWith("=0"))
            {
                url = url.TrimEnd('0');
            }
            else if (!url.EndsWith("/"))
                url += "/";

            return url;
        }
    }
}