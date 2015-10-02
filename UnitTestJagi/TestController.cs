using Jagi.Helpers;
using Jagi.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTestJagi
{
    public class TestController : JagiControllerBase
    {
        public JsonResult GetStandardJson()
        {
            var sample = SetupSample();
            return Json(sample);
        }

        public JsonResult GetStandardJsonArray()
        {
            var samples = SetupSamples();
            return Json(samples);
        }

        private static List<Sample> SetupSamples()
        {
            var samples = new List<Sample>
            {
                new Sample { Number = 11, Text = "Abc", StartDate = "2015/2/2".ConvertToDateTime() },
                new Sample { Number = 12, Text = "ABc", NullableInt = 1 },
                new Sample { Number = 13, Text = "AbD", IsChinese = true },
            };
            return samples;
        }

        public JsonResult GetBetterJson()
        {
            return BetterJson(SetupSample());
        }

        public JsonResult GetBetterJsonArray()
        {
            var samples = SetupSamples();
            return BetterJson(samples);
        }

        private Sample SetupSample()
        {
            string text = Request.QueryString["x"];
            string input = Request.Form["y"];
            var sample = new Sample { Number = 1, Text = text, StartDate = "2015/2/1".ConvertToDateTime(), EndDate = input };
            return sample;
        }
    }
}
