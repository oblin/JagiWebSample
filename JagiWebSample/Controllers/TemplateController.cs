using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JagiWebSample.Controllers
{
    public class TemplateController : Controller
    {
        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/app/{0}/templates/{1}", feature, name));
        }
    }
}