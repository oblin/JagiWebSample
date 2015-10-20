using System.Web.Optimization;

namespace JagiWebSample
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/underscore.js",
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-route.js",
                        "~/Scripts/ui-grid.js",
                        "~/Scripts/angular-ui/ui-bootstrap-tpls.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                    "~/app/utility/ArrayExtensions.js",
                    "~/app/utility/common.js",
                    "~/app/app.js",
                    "~/app/utility/alerts.js",
                    "~/app/directives/inputDirective.js",
                    "~/app/directives/showErrorsDirective.js",
                    "~/app/directives/preventDefault.js",
                    "~/app/directives/iconDirective.js",
                    "~/app/directives/clickConfirm.js",
                    "~/app/directives/InputValidationIconsDirective.js",
                    "~/app/directives/FormGroupValidationDirective.js",
                    "~/app/directives/waitModalDirective.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/MvcMembership.css",
                      "~/fonts/font-awesome/css/font-awesome.css",
                      "~/Content/site.css"));
        }
    }
}
