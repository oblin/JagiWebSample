using Jagi.Helpers;
using Jagi.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UnitTestJagi
{
    [TestClass]
    public class BetterJsonResultTest
    {
        protected HttpContextBase context;
        protected HttpRequestBase request;
        protected HttpResponseBase response;

        /// <summary>
        /// 標準設定 Context, Request & Response 提供後續測試
        /// 這裡的作法提供可以自定義 Controller，不需要 Web 專案
        /// </summary>
        [TestInitialize ]
        public void Setup()
        {
            context = Substitute.For<HttpContextBase>();
            request = Substitute.For<HttpRequestBase>();
            response = Substitute.For<HttpResponseBase>();
            response.Output.Returns(new StringWriter());
            context.Request.Returns(request);
            context.Response.Returns(response);
        }

        protected TestController SetupController()
        {
            var routes = new RouteCollection();
            var controller = new TestController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);
            controller.Url = new UrlHelper(new RequestContext(context, new RouteData()), routes);

            return controller;
        }

        protected virtual void SetupRequestCollection()
        {
            request.QueryString.Returns(new NameValueCollection { { "x", "testQString" } });
            request.Form.Returns(new NameValueCollection { { "y", "testDate" } });
        }

        /// <summary>
        /// 錯誤的測試方式，這裡 result.Data 永遠等於 Sample 因為並沒有執行 ExecuteResult()
        /// </summary>
        [TestMethod]
        public void Test_Standard_JsonResult_Result_Object_Sample()
        {
            SetupRequestCollection();

            var controller = SetupController();
            var result = controller.GetStandardJson();
            result.ExecuteResult(controller.ControllerContext);
            var data = result.Data;

            var propText = data.GetType().GetProperty("Text");
            var propEndDate = data.GetType().GetProperty("EndDate");
            Assert.IsNotNull(propText);
            Assert.AreEqual("testQString", propText.GetValue(data, null));
            Assert.AreEqual("testDate", propEndDate.GetValue(data, null));

            Assert.IsNull(data.GetType().GetProperty("text"));
        }

        [TestMethod]
        public void Test_Standard_JsonResult_Response_String()
        {
            SetupRequestCollection();

            var controller = SetupController();
            var result = controller.GetStandardJson();

            var response = controller.ControllerContext.HttpContext.Response;
            string responseWrite = string.Empty;
            response.Write(Arg.Do<string>(x => responseWrite = x));

            result.ExecuteResult(controller.ControllerContext);

            Assert.IsFalse(string.IsNullOrEmpty(responseWrite));
            Assert.IsTrue(responseWrite.IndexOf("Text", StringComparison.Ordinal) > 0);
            Assert.IsFalse(responseWrite.IndexOf("text", StringComparison.Ordinal) > 0);
        }


        /// <summary>
        /// 重點：使用 HttpResponseBase.Write() 取出實際的結果值
        /// </summary>
        [TestMethod]
        public void Test_BetterJsonResult_Result()
        {
            SetupRequestCollection();

            var controller = SetupController();
            var result = controller.GetBetterJson();

            var response = controller.ControllerContext.HttpContext.Response;
            string responseWrite = string.Empty;
            response.Write(Arg.Do<string>(x => responseWrite = x));

            result.ExecuteResult(controller.ControllerContext);

            Assert.IsFalse(string.IsNullOrEmpty(responseWrite));
            Assert.IsFalse(responseWrite.IndexOf("Text", StringComparison.Ordinal) > 0);
            Assert.IsTrue(responseWrite.IndexOf("text", StringComparison.Ordinal) > 0);

            var resultObject = JsonConvert.DeserializeObject<dynamic>(responseWrite);
            Assert.AreEqual("testQString", (string)resultObject.text);
            Assert.AreEqual(1, (int)resultObject.number);
        }

        [TestMethod]
        public void Test_Standard_JsonResult_Array_Result()
        {
            SetupRequestCollection();

            var controller = SetupController();
            var result = controller.GetStandardJsonArray();

            var response = controller.ControllerContext.HttpContext.Response;
            string responseWrite = string.Empty;
            response.Write(Arg.Do<string>(x => responseWrite = x));

            result.ExecuteResult(controller.ControllerContext);

            var resultObjects = JsonConvert.DeserializeObject<List<dynamic>>(responseWrite);
            Assert.AreEqual("Abc", (string)resultObjects[0].Text);
            Assert.AreEqual(11, (int)resultObjects[0].Number);
        }

        [TestMethod]
        public void Test_Better_JsonResult_Array_Result()
        {
            SetupRequestCollection();

            var controller = SetupController();
            var result = controller.GetBetterJsonArray();

            var response = controller.ControllerContext.HttpContext.Response;
            string responseWrite = string.Empty;
            response.Write(Arg.Do<string>(x => responseWrite = x));

            result.ExecuteResult(controller.ControllerContext);

            var resultObjects = JsonConvert.DeserializeObject<List<dynamic>>(responseWrite);
            Assert.AreEqual("Abc", (string)resultObjects[0].text);
            Assert.AreEqual(11, (int)resultObjects[0].number);
        }
    }

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
