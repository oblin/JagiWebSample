using Jagi.Helpers;
using Jagi.Mvc.Angular;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace UnitTestJagi
{
    [TestClass]
    public class AngularModelHelperTest
    {
        private List<Sample> samples;
        private HtmlHelper<Sample> sampleHtmlHelper;
        private MockMvc mvc;

        [TestInitialize]
        public void Setup()
        {
            samples = new List<Sample>
            {
                new Sample { Number = 11, Text = "Abc" },
                new Sample { Number = 12, Text = "ABc" },
                new Sample { Number = 13, Text = "AbD" }
            };

            mvc = new MockMvc();
            sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
        }

        [TestMethod]
        public void Test_Angular_ExpressionFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var expression = sampleModel.ExpressionFor(x => x.IsChinese);
            Assert.AreEqual("isChinese", expression.ToString());

            sampleModel = sampleHtmlHelper.Angular().ModelFor("sample");

            expression = sampleModel.ExpressionFor(x => x.Number);
            Assert.AreEqual("sample.number", expression.ToString());
        }

        [TestMethod]
        public void Test_Angular_ExpressionFor_ComplexType_Properties()
        {
            List<Sample> samples = new List<Sample>()
            {
                new Sample (), new Sample()
            };

            LotsSamples ls = new LotsSamples
            {
                Samples = samples
            };
            var lsHtmlHelper = mvc.CreateHtmlHelper<LotsSamples>();
            var lsModel = lsHtmlHelper.Angular().ModelFor(string.Empty);
            var expression1 = lsModel.ExpressionFor(x => x.Samples.Count);
            Assert.AreEqual("samples.count", expression1.ToString());

            var expression2 = lsModel.ExpressionFor(x => x.Samples[0].Text);
            Assert.AreEqual("samples[0].text", expression2.ToString());
        }

        [TestMethod]
        public void Test_Angular_NgRepeat()
        {
            List<Sample> samples = new List<Sample>()
            {
                new Sample (), new Sample()
            };

            LotsSamples ls = new LotsSamples
            {
                Samples = samples
            };
            var lsHtmlHelper = mvc.CreateHtmlHelper<LotsSamples>(needOut: true);
            var lsModel = lsHtmlHelper.Angular().ModelFor(string.Empty);
            lsModel.Repeat(x => x.Samples, "model");
            string writeOut = mvc.ViewContextWriteOut;
            Assert.AreEqual("<div ng-repeat=\"model in samples\">", writeOut);

            lsModel.Repeat(x => x.Samples, "sample");
            writeOut = mvc.ViewContextWriteOut;
            Assert.AreEqual("<div ng-repeat=\"sample in samples\">", writeOut);
        }

        [TestMethod]
        public void Test_Angular_BindingFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var expression = sampleModel.BindingFor(x => x.IsChinese);
            Assert.AreEqual("{{isChinese}}", expression.ToString());

            sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");

            expression = sampleModel.BindingFor(x => x.Text);
            Assert.AreEqual("{{vm.sample.text}}", expression.ToString());
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Default).ToString();

            /// Result:
            /// <div form-group-validation="Text" class="form-group has-feedback">
            ///     <label for="Text" class="control-label">Text</label>
            ///     <input placeholder="請輸入任意文字..." type="text" name="Text" ng-model="text" class="form-control" />
            /// </div>

            Assert.IsTrue(htmlString.Contains("label for=\"Text\""));
            Assert.IsTrue(htmlString.Contains("class=\"control-label\">"));
            Assert.IsTrue(htmlString.Contains("<input"));
            Assert.IsTrue(htmlString.Contains("ng-model=\"text\""));
            Assert.IsTrue(htmlString.Contains("name=\"Text\""));
            Assert.IsTrue(htmlString.Contains("type=\"text\""));
            Assert.IsTrue(htmlString.Contains("placeholder=\"請輸入任意文字...\""));
            Assert.IsTrue(htmlString.Contains("class=\"form-control\""));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_WithLabelInputGrid()
        {
            var grid = new FormGroupLayout(5, 7);
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty, grid);

            var htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Default).ToString();

            /// Result:
            /// <div form-group-validation="Text" class="form-group has-feedback">
            ///     <label for="Text" class="control-label col-sm-5">Text</label>
            ///     <div class="col-sm-7">
            ///     <input placeholder="請輸入任意文字..." type="text" name="Text" ng-model="text" class="form-control" />
            ///     </div>
            /// </div>

            Assert.IsTrue(htmlString.Contains("label for=\"Text\""));
            Assert.IsTrue(htmlString.Contains("class=\"control-label col-sm-5\">"));
            Assert.IsTrue(htmlString.Contains("<div class=\"col-sm-7\">"));
            Assert.IsTrue(htmlString.Contains("ng-model=\"text\""));
            Assert.IsTrue(htmlString.Contains("name=\"Text\""));
            Assert.IsTrue(htmlString.Contains("type=\"text\""));
            Assert.IsTrue(htmlString.Contains("placeholder=\"請輸入任意文字...\""));
            Assert.IsTrue(htmlString.Contains("class=\"form-control\""));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_Required()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var htmlString = sampleModel.FormGroupFor(x => x.Number).ToString();

            /// Result:
            /// <div form-group-validation="Text" class="form-group has-feedback required">
            ///     <label for="Text" class="control-label col-sm-5">Text</label>
            ///     <div class="col-sm-7">
            ///     <input placeholder="請輸入任意文字..." type="text" name="Text" ng-model="text" class="form-control" />
            ///     </div>
            /// </div>

            Assert.IsTrue(htmlString.Contains("<div form-group-validation=\"Number\" class=\"form-group has-feedback required\">"));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_WithLabelInput_AndFormGrid()
        {
            var grid = new FormGroupLayout(5, 7,4 );
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty, grid);

            var htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Default).ToString();

            /// Result:
            /// <div form-group-validation="Text" class="form-group has-feedback col-sm-4">
            ///     <label for="Text" class="control-label col-sm-5">Text</label>
            ///     <div class="col-sm-7">
            ///     <input placeholder="請輸入任意文字..." type="text" name="Text" ng-model="text" class="form-control" />
            ///     </div>
            /// </div>

            Assert.IsTrue(htmlString.Contains("<div form-group-validation=\"Text\" class=\"form-group has-feedback col-sm-4\">"));
            Assert.IsTrue(htmlString.Contains("label for=\"Text\" class=\"control-label col-sm-5\">"));
            Assert.IsTrue(htmlString.Contains("<div class=\"col-sm-7\">"));
            Assert.IsTrue(htmlString.Contains("ng-model=\"text\""));
            Assert.IsTrue(htmlString.Contains("name=\"Text\""));
            Assert.IsTrue(htmlString.Contains("type=\"text\""));
            Assert.IsTrue(htmlString.Contains("placeholder=\"請輸入任意文字...\""));
            Assert.IsTrue(htmlString.Contains("class=\"form-control\""));

            htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Default, formGroupGrid: 6).ToString();
            Assert.IsTrue(htmlString.Contains("<div form-group-validation=\"Text\""));
            Assert.IsTrue(htmlString.Contains("class=\"form-group has-feedback col-sm-6\">"));

            grid = new FormGroupLayout(5, 7);
            sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty, grid);
            htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Default, formGroupGrid: 5).ToString();
            Assert.IsTrue(htmlString.Contains("<div form-group-validation=\"Text\""));
            Assert.IsTrue(htmlString.Contains("class=\"form-group has-feedback col-sm-5\">"));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_Textarea()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var htmlString = sampleModel.FormGroupFor(x => x.MultiLine).ToString();
            Assert.IsTrue(htmlString.Contains("<textarea"));

            htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Textarea).ToString();
            Assert.IsTrue(htmlString.Contains("<textarea"));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_Number()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var htmlString = sampleModel.FormGroupFor(x => x.Number).ToString();
            Assert.IsTrue(htmlString.Contains("text-right"));

            htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Number).ToString();
            Assert.IsTrue(htmlString.Contains("text-right"));

            htmlString = sampleModel.FormGroupFor(x => x.FloatingPoint).ToString();
            Assert.IsTrue(htmlString.Contains("text-right"));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_Dropdown()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
            var options = SetOptions();
            var htmlString = sampleModel.FormGroupFor(x => x.Text,
                FormGroupType.Dropdown,
                options: options).ToString();

            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.sample.text\""));
            Assert.IsTrue(htmlString.Contains("<option value=\"\">請選擇...</option>"));
            Assert.IsTrue(htmlString.Contains("<option value=\"1\">Key 1</option>"));
            Assert.IsTrue(htmlString.Contains("</select>"));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_Dropdown_CustomAttr()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
            var attrs = SetAttrs();
            var htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Dropdown,
                attrs: attrs).ToString();

            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.sample.text\""));
            Assert.IsTrue(htmlString.Contains("ng-options=\"color.name for color in colors\""));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_Dropdown_CustomAttr_String()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
            var attrs = SetAttrs();
            var htmlString = sampleModel.FormGroupFor(x => x.Text,
                FormGroupType.Dropdown,
                attr: "ng-options='color.name for color in colors'").ToString();

            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.sample.text\""));
            Assert.IsTrue(htmlString.Contains("ng-options=\"color.name for color in colors\""));
        }

        [TestMethod]
        public void Test_Angular_FormGroupFor_Checkbox()
        {
            // Checkbox 跟  Input 一樣
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
            var htmlString = sampleModel.FormGroupFor(x => x.IsChinese, value: "CheckboxValue").ToString();
            Assert.IsTrue(htmlString.Contains("label for=\"IsChinese\""));
            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.sample.isChinese\""));
            Assert.IsTrue(htmlString.Contains("type=\"checkbox\""));
        }

        //[TestMethod]
        //public void Test_Angular_FormGroupFor_Checkbox_MultiValues()
        //{
        //    var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
        //    var values = SetValues();
        //    var htmlString = sampleModel.FormGroupFor(x => x.IsChinese, values: values).ToString();

        //    /// <div form-group-validation="IsChinese" class="form-group has-feedback">
        //    ///    <div class="checkbox">
        //    ///      <label>
        //    ///        <input value="True" type="checkbox" name="IsChinese" ng-model="vm.sample.isChinese" class="form-control" />
        //    ///      </label>
        //    ///    </div>
        //    ///    <div class="checkbox">
        //    ///      <label>
        //    ///        <input value="False" type="checkbox" name="IsChinese" ng-model="vm.sample.isChinese" class="form-control" />
        //    ///      </label>
        //    ///    </div>
        //    /// </div>
        //    Assert.IsTrue(htmlString.Contains("<div class=\"checkbox\"><label><input value=\"" + values[0] + "\" type=\"checkbox\""));
        //    Assert.IsTrue(htmlString.Contains("<div class=\"checkbox\"><label><input value=\"" + values[1] + "\" type=\"checkbox\""));
        //}

        //[TestMethod]
        //public void Test_Angular_FormGroupFor_RadioButton()
        //{
        //    // 尚未決定好如何測試，基本上如果有此需求，會建議直接寫 html 語法
        //    var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
        //    var htmlString = sampleModel.FormGroupFor(x => x.IsChinese, 
        //        type: FormGroupType.RadioButton).ToString();
        //    /// <div class="radio">
        //    ///     <label><input type="radio" value="perkCar" 
        //    ///               ng-model="editableEmployee.perkCar"/>Company Car</label>
        //    /// </div>
        //    Assert.IsTrue(htmlString.Contains("<div class=\"radio\""));
        //    Assert.IsTrue(htmlString.Contains("<label><input value=\"true\" type=\"radio\""));

        //    htmlString = sampleModel.FormGroupFor(x => x.IsChinese, 
        //        type: FormGroupType.RadioButton, 
        //        value: "CheckboxValue").ToString();
        //    Assert.IsTrue(htmlString.Contains("<label><input value=\"CheckboxValue\" type=\"radio\""));
        //}

        //[TestMethod]
        //public void Test_Angular_FormGroupFor_RadioButton_Multi()
        //{
        //    var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
        //    var values = SetValues();
        //    var htmlString = sampleModel.FormGroupFor(x => x.IsChinese,
        //        type: FormGroupType.RadioButton, values: values).ToString();

        //    Assert.IsTrue(htmlString.Contains("<div class=\"radio\"><label><input value=\"" + values[0] + "\" type=\"radio\""));
        //    Assert.IsTrue(htmlString.Contains("<div class=\"radio\"><label><input value=\"" + values[1] + "\" type=\"radio\""));
        //}

        [TestMethod]
        public void Test_Angular_LabelFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularLabelFor(x => x.IsChinese).ToString();

            Assert.AreEqual("<label for=\"IsChinese\" class=\"control-label\">Is Chinese</label>", htmlString);
        }

        [TestMethod]
        public void Test_Angular_EditorFor()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.Text).ToString();
            // result: <input placeholder="請輸入任意文字..." type="text" name="Text" ng-model="vm.text" class="form-control" />
            Assert.IsTrue(htmlString.Contains("type=\"text\""));
            Assert.IsTrue(htmlString.Contains("name=\"Text\""));
            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.text\""));
            Assert.IsTrue(htmlString.Contains("class=\"form-control\""));
        }

        [TestMethod]
        public void Test_Angular_EditorFor_Number()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.Number).ToString();            
            Assert.IsTrue(htmlString.Contains("type=\"number\" name=\"Number\""));

            htmlString = sampleModel.AngularEditorFor(x => x.NullableInt).ToString();
            Assert.IsTrue(htmlString.Contains("type=\"number\" name=\"NullableInt\""));
        }

        [TestMethod]
        public void Test_Angular_EditorFor_Validation()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<SampleValidation>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.Text).ToString();
            // result: <input placeholder="請輸入任意文字..." type="text" name="Text" ng-model="vm.text" class="form-control" />
            Assert.IsTrue(htmlString.Contains("ng-maxlength=\"5\""));
            Assert.IsTrue(htmlString.Contains("required"));

            htmlString = sampleModel.AngularEditorFor(x => x.LimitTest).ToString();
            // result: <input placeholder="請輸入任意文字..." type="text" name="Text" ng-model="vm.text" class="form-control" />
            Assert.IsTrue(htmlString.Contains("ng-maxlength=\"10\""));
            Assert.IsTrue(htmlString.Contains("ng-minlength=\"4\""));
            Assert.IsFalse(htmlString.Contains("required"));
        }

        [TestMethod]
        public void Test_Angular_EditorFor_Date()
        {
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.StartDate).ToString();
            /// <div class="input-group">
            /// 	<input type = "text" class="form-control" datepicker-popup="yyyy/MM/dd" ng-model="editableEmployee.dateHired" is-open="status.opened" current-text="今天" clear-text="清除" close-text="關閉" />
            /// 	<span class="input-group-btn">
            /// 		<button type = "button" class="btn btn-default" ng-click="openCalendar()"><i class="glyphicon glyphicon-calendar"></i></button>
            /// 	</span>                       
            /// </div>
            Assert.IsTrue(htmlString.Contains("<div class=\"input-group\">"));
            Assert.IsTrue(htmlString.Contains("datepicker-popup=\"yyyy/MM/dd\""));
            Assert.IsTrue(htmlString.Contains("is-open=\"dateStatus.opened\""));
            Assert.IsTrue(htmlString.Contains("<span class=\"input-group-btn\">"));
            Assert.IsTrue(htmlString.Contains("ng-click=\"dateStatus.opened = true\""));
        }

        private RouteValueDictionary SetAttrs()
        {
            Dictionary<string, string> attrs = new Dictionary<string, string>();
            attrs.Add("ng-options", "color.name for color in colors");
            return attrs.ToRouteValueDictionary();
        }

        private string[] SetValues()
        {
            string[] values = new string[]{ "True", "False", "Null" };
            return values;
        }
        
        private Dictionary<string, string> SetOptions()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("", "請選擇...");
            options.Add("1", "Key 1");
            options.Add("2", "Key 2");

            return options;
        }
    }

    internal class LotsSamples
    {
        public IList<Sample> Samples { get; set; }
    }
}