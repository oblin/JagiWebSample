using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UnitTestJagi
{
    public abstract class MockMvcBase
    {
        protected HttpContextBase context;
        protected HttpRequestBase request;
        protected HttpResponseBase response;
        protected ControllerBase baseController;

        /// <summary>
        /// Must invoke in [TestInitialize ]
        /// </summary>
        protected void SetupHttpContext()
        {
            context = Substitute.For<HttpContextBase>();
            request = Substitute.For<HttpRequestBase>();
            response = Substitute.For<HttpResponseBase>();
            baseController = Substitute.For<ControllerBase>();
            response.Output.Returns(new StringWriter());
            context.Request.Returns(request);
            context.Response.Returns(response);
        }

        protected TestController SetupController()
        {
            var routes = new RouteCollection();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            RouteData route = new RouteData();
            route.Values.Add("controller", "Test");
            route.Values.Add("action", "Link");
            route.Values.Add("id", "null");

            var controller = new TestController();
            controller.ControllerContext = new ControllerContext(context, route, controller);
            controller.Url = new UrlHelper(new RequestContext(context, route), routes);

            return controller;
        }
    }

    public class MockMvc : MockMvcBase
    {
        public string ViewContextWriteOut { get; set; }

        public MockMvc()
        {
            base.SetupHttpContext();
        }

        public HtmlHelper<T> CreateHtmlHelper<T>(bool needOut = false) where T : new()
        {
            var viewDataDictionary = new ViewDataDictionary(new T());
            var controllerContext = new ControllerContext(context, new RouteData(), baseController);

            IView view = Substitute.For<IView>();
            TextWriter writer = Substitute.For<TextWriter>();
            var viewContext = new ViewContext(controllerContext, view, viewDataDictionary, new TempDataDictionary(), writer);

            IViewDataContainer viewDataContainer = Substitute.For<IViewDataContainer>();
            viewDataContainer.ViewData.Returns(viewDataDictionary);

            if (needOut)
                viewContext.Writer.Write(Arg.Do<string>(x => ViewContextWriteOut = x));

            return new HtmlHelper<T>(viewContext, viewDataContainer);
        }

    }
}
