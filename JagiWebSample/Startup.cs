using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JagiWebSample.Startup))]
namespace JagiWebSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
