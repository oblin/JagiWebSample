using Jagi.Database;
using Jagi.Database.Cache;
using Jagi.Database.Models;
using Jagi.Database.Mvc;
using Jagi.Mvc;
using Jagi.Mvc.Angular;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestJagi
{
    [TestClass]
    public class ColumnHtmlTagTest
    {
        private MockMvc mvc;

        private IQueryable<TableSchema> tableSchema;

        /// <summary>
        /// 測試前置設定：
        ///     1. Columns Cache
        ///     2. 將 Cache 放入 Memory.Default
        ///     3. 設定 ServiceLocator
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Setup Code Definition
            var codedetails = new List<CodeDetail>
            {
                new CodeDetail { ITEM_CODE = "A1", CodeFileID = 1, DESC = "A1 Detail" },
                new CodeDetail { ITEM_CODE = "A2", CodeFileID = 1, DESC = "A2 Detail" },
                new CodeDetail { ITEM_CODE = "B1", CodeFileID = 2, DESC = "B1 Detail" },
                new CodeDetail { ITEM_CODE = "C1", CodeFileID = 3, DESC = "C1 Detail" },
                new CodeDetail { ITEM_CODE = "C2", CodeFileID = 3, DESC = "C2 Detail" },
                new CodeDetail { ITEM_CODE = "C3", CodeFileID = 3, DESC = "C3 Detail" }
            }.AsQueryable();

            var codefiles = new List<CodeFile>
            {
                new CodeFile { ID = 1, ITEM_TYPE = "A", TYPE_NAME = "A Name", CodeDetails = codedetails.Where(p => p.CodeFileID == 1).ToList() },
                new CodeFile { ID = 2, ITEM_TYPE = "B", TYPE_NAME = "B Name", PARENT_CODE = "A",
                    CodeDetails = codedetails.Where(p => p.CodeFileID == 2).ToList() },
                new CodeFile { ID = 3, ITEM_TYPE = "C", TYPE_NAME = "C Name", CodeDetails = codedetails.Where(p => p.CodeFileID == 3).ToList() },
            }.AsQueryable();

            // Setup Column Definition
            tableSchema = new List<TableSchema>
            {
                new TableSchema { TableName = ConstantString.SCHEMA_DEFAULT_TABLE_NAME, ColumnName = "Number", DisplayName = "Display Id", MinValue = 0, DataType = Jagi.Interface.FieldType.Int32 },
                new TableSchema { TableName = "Sample", ColumnName = "Text", DataType = Jagi.Interface.FieldType.Int32 },
                new TableSchema { TableName = "Sample", ColumnName = "IsChinese" },
                new TableSchema { TableName = "Sample", ColumnName = "StartDate", DisplayName = "開始日期", DataType = Jagi.Interface.FieldType.DateTime },
                new TableSchema { TableName = "Sample", ColumnName = "EndDate", DisplayName = "結束日期", StringMaxLength = 10, Nullable = false },
                new TableSchema { TableName = "Sample", ColumnName = "NullableInt", DataType = Jagi.Interface.FieldType.Int32, Nullable = true, MinValue = 10, MaxValue = 30 },
                new TableSchema { TableName = "Sample", ColumnName = "FloatingPoint", DataType = Jagi.Interface.FieldType.Decimal, Nullable = false },
                new TableSchema { TableName = "Sample", ColumnName = "Dropdown1", DropdwonKey = "A" },
                new TableSchema { TableName = "Sample", ColumnName = "Dropdown2", DropdwonKey = "C" },
                new TableSchema { TableName = "Sample", ColumnName = "Dropdown3", DropdwonKey = "B", 
                    DropdwonCascade = "Dropdown1" },
            }.AsQueryable();

            // Initial Column Cache
            var mockTableSchema = tableSchema.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.TableSchema.Returns(mockTableSchema);

            var columnsInitializer = new InitColumnCache(mockContext);
            columnsInitializer.Execute();

            // Initial Code Cache
            var mockCodesSet = codefiles.MockDbSet();
            var mockDetailsSet = codedetails.MockDbSet();
            mockContext.CodeFiles.Returns(mockCodesSet);
            mockContext.CodeDetails.Returns(mockDetailsSet);

            var codeInitializer = new CodeCacheManager(mockContext);
            codeInitializer.Execute();

            // Setup ServiceLocator
            var container = new UnityContainer();

            var angularHtmlTag = new ColumnsHtmlTag();
            container.RegisterInstance(typeof(AngularHtmlTag), angularHtmlTag);

            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            // Setup HtmlHelper
            mvc = new MockMvc();
        }

        [TestMethod]
        public void Test_Get_ColumnNumber_DisplayName()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var model = sampleHtmlHelper.Angular().ModelFor(string.Empty);
            var expression = model.AngularLabelFor(x => x.Number).ToString();
            // Test: had defined in column cache
            Assert.IsTrue(expression.Contains("Display Id"));

            // 測試類別名稱開頭符合 Sample 時候，一樣按照同一邏輯
            var sample4HtmlHelper = mvc.CreateHtmlHelper<SampleCopy4>();
            var model4 = sample4HtmlHelper.Angular().ModelFor(string.Empty);
            expression = model4.AngularLabelFor(x => x.Number).ToString();
            Assert.IsTrue(expression.Contains("Display Id"));

            // 測試如果已經定義 DisplayName attribute時候，不使用 cache 的定義:
            var sampleVHtmlHelper = mvc.CreateHtmlHelper<SampleValidation>();
            var modelV = sampleVHtmlHelper.Angular().ModelFor(string.Empty);
            expression = modelV.AngularLabelFor(x => x.Number).ToString();
            Assert.IsTrue(expression.Contains("數字"));
        }

        [TestMethod]
        public void Test_Angular_EditorFor_String_EndDate_WithDateTime()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.EndDate).ToString();

            Assert.IsTrue(htmlString.Contains("type=\"text\""));
            Assert.IsTrue(htmlString.Contains("name=\"EndDate\""));
            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.endDate\""));
            Assert.IsTrue(htmlString.Contains("is-open=\"dateStatus.opened\""));
            Assert.IsTrue(htmlString.Contains("<span class=\"input-group-btn\">"));
            Assert.IsTrue(htmlString.Contains("ng-click=\"dateStatus.opened = true\""));
        }

        [TestMethod]
        public void Test_Angular_EditorFor_DateTime_StartDate_Is_Still_Text_Type()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.StartDate).ToString();

            Assert.IsTrue(htmlString.Contains("type=\"text\""));
            Assert.IsTrue(htmlString.Contains("name=\"StartDate\""));
            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.startDate\""));
            Assert.IsTrue(htmlString.Contains("is-open=\"dateStatus.opened\""));
            Assert.IsTrue(htmlString.Contains("<span class=\"input-group-btn\">"));
            Assert.IsTrue(htmlString.Contains("ng-click=\"dateStatus.opened = true\""));
        }

        [TestMethod]
        public void Test_ColumnCache_Editor_Number()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.Text).ToString();
            Assert.IsTrue(htmlString.Contains("type=\"number\""));
            Assert.IsTrue(htmlString.Contains("name=\"Text\""));
        }

        [TestMethod]
        public void Test_ColumnCache_Editor_Nullable_Number_WithValidation()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm");
            var htmlString = sampleModel.AngularEditorFor(x => x.NullableInt).ToString();
            Assert.IsTrue(htmlString.Contains("type=\"number\""));
            Assert.IsTrue(htmlString.Contains("name=\"NullableInt\""));
            Assert.IsFalse(htmlString.Contains("required"));
            Assert.IsTrue(htmlString.Contains("min=\"10\""));
            Assert.IsTrue(htmlString.Contains("max=\"30\""));

            htmlString = sampleModel.AngularEditorFor(x => x.Number).ToString();
            Assert.IsTrue(htmlString.Contains("required"));
        }

        [TestMethod]
        public void Test_ColumnCache_FormGroupFor_Textarea()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor(string.Empty);

            var htmlString = sampleModel.FormGroupFor(x => x.MultiLine).ToString();
            Assert.IsTrue(htmlString.Contains("<textarea"));

            htmlString = sampleModel.FormGroupFor(x => x.Text, FormGroupType.Textarea).ToString();
            Assert.IsTrue(htmlString.Contains("<textarea"));
        }

        [TestMethod]
        public void Test_ColumnCache_FormGroupFor_Dropdown()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
            var htmlString = sampleModel.FormGroupFor(x => x.Dropdown1).ToString();

            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.sample.dropdown1\""));
            Assert.IsTrue(htmlString.Contains("<option value=\"A1\">A1 Detail</option>"));
            Assert.IsTrue(htmlString.Contains("<option value=\"A2\">A2 Detail</option>"));
            Assert.IsTrue(htmlString.Contains("</select>"));

            Assert.IsFalse(htmlString.Contains("<option value=\"B1\">B1 Detail</option>"));

            htmlString = sampleModel.FormGroupFor(x => x.Dropdown2).ToString();

            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.sample.dropdown2\""));
            Assert.IsTrue(htmlString.Contains("<option value=\"C1\">C1 Detail</option>"));
            Assert.IsTrue(htmlString.Contains("<option value=\"C2\">C2 Detail</option>"));
            Assert.IsTrue(htmlString.Contains("</select>"));

            Assert.IsFalse(htmlString.Contains("<option value=\"B1\">B1 Detail</option>"));
        }

        [TestMethod]
        public void Test_ColumnCache_FormGroupFor_DropdownCascade()
        {
            var sampleHtmlHelper = mvc.CreateHtmlHelper<Sample>();
            var sampleModel = sampleHtmlHelper.Angular().ModelFor("vm.sample");
            var htmlString = sampleModel.FormGroupFor(x => x.Dropdown3).ToString();

            Assert.IsTrue(htmlString.Contains("ng-model=\"vm.sample.dropdown3\""));
            Assert.IsTrue(htmlString.Contains("dropdown-cascade=\"vm.sample.dropdown1\""));
            Assert.IsTrue(htmlString.Contains("</select>"));

            Assert.IsFalse(htmlString.Contains("<option value=\"\"></option>"));
            Assert.IsFalse(htmlString.Contains("<option value=\"B1\">B1 Detail</option>"));

            Assert.IsTrue(htmlString.Contains("ng-options=\"key as value for (key, value) in codedetailDropdown3\""));
            Assert.IsTrue(htmlString.Contains("cascade-options=\"codedetailDropdown3\""));
        }
    }
}