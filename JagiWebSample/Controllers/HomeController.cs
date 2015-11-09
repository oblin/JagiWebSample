using Jagi.Helpers;
using JagiWebSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JagiWebSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            string testString = "Your contact page. {0} - {1}";

            ViewBag.Message = testString.FormatWith("test", "string");

            return View();
        }
    }
}