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
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;

namespace UnitTestJagi
{
    [TestClass]
    public class ColumnValidationTest
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
                new TableSchema { TableName = "Sample", ColumnName = "FloatingPoint", DataType = Jagi.Interface.FieldType.Decimal, Nullable = false, MinValue = 13 },
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
        public void Test_Sample_EndDate_Required()
        {
            var sample = new Sample { Text = "10", Number = 10 };
            var result = ColumnsValidations.ColumnsValidate(sample);

            Assert.IsTrue(result.Count > 0);

            var endDate = result.FirstOrDefault(p => p.PropertyName == "EndDate");
            var errors = endDate.Rules;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Keys.Contains(ConstantString.VALIDATION_REQUIRED_FIELD));

            sample = new Sample { EndDate = "123456789011" };
            result = ColumnsValidations.ColumnsValidate(sample);
            endDate = result.FirstOrDefault(p => p.PropertyName == "EndDate");
            errors = endDate.Rules;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Keys.Contains(ConstantString.VALIDATION_MAXLENGTH_FIELD));
        }

        [TestMethod]
        public void Test_Sample_NullableInt_Min_MaxValue()
        {
            var sample = new Sample { Text = "10" };
            var result = ColumnsValidations.ColumnsValidate(sample);

            Assert.IsTrue(result.Count > 0);

            var endDate = result.FirstOrDefault(p => p.PropertyName == "NullableInt");
            var errors = endDate.Rules;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Keys.Contains(ConstantString.VALIDATION_MIN_VALUE));

            sample = new Sample { NullableInt = 123 };
            result = ColumnsValidations.ColumnsValidate(sample);
            endDate = result.FirstOrDefault(p => p.PropertyName == "NullableInt");
            errors = endDate.Rules;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Keys.Contains(ConstantString.VALIDATION_MAX_VALUE));
        }

        [TestMethod]
        public void Test_Sample_FloatingPoint_MiValue()
        {
            var sample = new Sample { Text = "10" };
            var result = sample.ColumnValidation(s => s.FloatingPoint);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rules.ContainsKey(ConstantString.VALIDATION_MIN_VALUE));
        }

        [TestMethod]
        public void Test_Sample_IsChiness_No_Error()
        {
            var sample = new Sample { Text = "10" };
            var result = sample.ColumnValidation(s => s.IsChinese);

            Assert.IsNull(result);
        }
    }
}