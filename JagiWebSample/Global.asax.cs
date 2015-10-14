using Jagi.Mapping;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace JagiWebSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SetupAutoMapper();
        }

        private void SetupAutoMapper()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var config = new MappingBaseConfig(assembly);

            config.Execute();
        }
    }
}
