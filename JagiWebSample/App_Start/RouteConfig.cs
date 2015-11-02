using System.Web.Mvc;
using System.Web.Routing;

namespace JagiWebSample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Templates",
                url: "{feature}/Template/{name}",
                defaults: new { controller = "Template", action = "Render" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // AppendTrailingSlash = true 會將 routing 變成：
            // /update/1/ 最後面都多加 "/" 
            // 這個會造成 JHtmlHelper.BuildUrlFromExpression() 處理上錯誤
            //routes.AppendTrailingSlash = true;
        }
    }
}
