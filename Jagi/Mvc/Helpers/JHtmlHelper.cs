using Jagi.Mvc;
using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jagi.Mvc.Helpers
{
    public static class JHtmlHelper
    {
        public static string BuildUrlFromExpression<TController>(this HtmlHelper helper, Expression<Action<TController>> action) where TController : Controller
        {
            var url = LinkExtensions.BuildUrlFromExpression<TController>(helper, action);
            if (url.EndsWith("/0"))
                url = url.Substring(0, url.Length - 1);
            else if (!url.EndsWith("/"))
                url += "/";

            return url;
        }
    }
}
