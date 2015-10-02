using Jagi.Helpers;
using Jagi.Mvc.Angular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UnitTestJagi
{
    [TestClass]
    public class AngularModelHelperTest
    {
        private List<Sample> samples;
        private HtmlHelper<Sample> sampleHtmlHelper;

        [TestInitialize]
        public void Setup()
        {
            samples = new List<Sample>
            {
                new Sample { Number = 11, Text = "Abc" },
                new Sample { Number = 12, Text = "ABc" },
                new Sample { Number = 13, Text = "AbD" }
            };

            var mvc = new MockMvc();
            sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
        }

        [TestMethod]
        public void Test_Angula_ExpressionFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var expression = sampleModel.ExpressionFor(x => x.IsChinese);
            Assert.AreEqual("isChinese", expression.ToString());

            sampleModel = sampleHtmlHelper.Angular().ModelFor("sample");

            expression = sampleModel.ExpressionFor(x => x.Number);
            Assert.AreEqual("sample.number", expression.ToString());
        }

        [TestMethod]
        public void Test_Angula_BindingFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var expression = sampleModel.BindingFor(x => x.IsChinese);
            Assert.AreEqual("{{isChinese}}", expression.ToString());

            sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");

            expression = sampleModel.BindingFor(x => x.Text);
            Assert.AreEqual("{{vm.sample.text}}", expression.ToString());
        }

        [TestMethod]
        public void Test_Angula_FormGroupFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var htmlString = sampleModel.FormGroupFor(x => x.Text, true).ToString();
            Assert.IsTrue(htmlString.Contains("label for=\"Text\" class=\"control-label\">"));
            Assert.IsTrue(
                htmlString.Contains("<input ng-model=\"text\" name=\"Text\" type=\"text\" placeholder=\"Text...\" class=\"form-control\" />"));
        }
    }
}