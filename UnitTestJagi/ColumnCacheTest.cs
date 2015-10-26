using Jagi.Database;
using Jagi.Database.Cache;
using Jagi.Database.Models;
using Jagi.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;

namespace UnitTestJagi
{
    [TestClass]
    public class ColumnCacheTest
    {
        private IQueryable<TableSchema> tableSchema;

        [TestInitialize]
        public void Setup()
        {
            tableSchema = new List<TableSchema>
            {
                new TableSchema { TableName = ConstantString.SCHEMA_DEFAULT_TABLE_NAME, ColumnName = "Id", DisplayName = "Display Id", MinValue = 0, DataType = Jagi.Interface.FieldType.Int32 },
                new TableSchema { TableName = "TableSchema", ColumnName = "TableName" },
                new TableSchema { TableName = "TableSchema", ColumnName = "ColumnName" },
                new TableSchema { TableName = "TableSchema", ColumnName = "DataType", DisplayName = "Display Data Type", StringMaxLength = 5 },
                new TableSchema { TableName = "TableSchema", ColumnName = "Nullable", DisplayName = "Display Nullable", Nullable = true, DataType = Jagi.Interface.FieldType.Boolean },
            }.AsQueryable();
        }

        [TestMethod]
        public void Test_Get_Column_DataType()
        {
            var mockTableSchema = tableSchema.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.TableSchema.Returns(mockTableSchema);

            var codeInitializer = new InitColumnCache(mockContext);
            codeInitializer.Execute();

            var columnCache = new ColumnsCache();

            var column = columnCache.Get("TableSchema", "DataType");

            Assert.IsFalse(column.Nullable);
            Assert.AreEqual("Display Data Type", column.DisplayName);

            column = columnCache.Get("TableSchema", "Nullable");
            Assert.IsTrue(column.Nullable);
        }

        [TestMethod]
        public void Test_Get_Column_Nullable()
        {
            var mockTableSchema = tableSchema.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.TableSchema.Returns(mockTableSchema);

            var codeInitializer = new InitColumnCache(mockContext);
            codeInitializer.Execute();

            var columnCache = new ColumnsCache();

            var column = columnCache.Get("TableSchema", "Nullable");

            Assert.AreEqual("Display Nullable", column.DisplayName);
            Assert.IsTrue(column.Nullable);
            Assert.AreEqual(Jagi.Interface.FieldType.Boolean, column.DataType);
        }

        [TestMethod]
        public void Test_Get_Column_Id()
        {
            var mockTableSchema = tableSchema.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.TableSchema.Returns(mockTableSchema);

            var codeInitializer = new InitColumnCache(mockContext);
            codeInitializer.Execute();

            var columnCache = new ColumnsCache();

            var column = columnCache.Get("TableSchema", "Id");
            Assert.IsNotNull(column);
            Assert.AreEqual("Display Id", column.DisplayName);
            Assert.AreEqual(Jagi.Interface.FieldType.Int32, column.DataType);
            Assert.AreEqual(0, column.MinValue);

            column = columnCache.Get("TableSchema", "NotImplemented");

            Assert.IsNull(column);
        }
    }
}