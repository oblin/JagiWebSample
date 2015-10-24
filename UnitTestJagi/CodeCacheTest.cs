using Jagi.Database;
using Jagi.Database.Cache;
using Jagi.Database.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;

namespace UnitTestJagi
{
    [TestClass]
    public class CodeCacheTest
    {
        private IQueryable<CodeFile> codefiles;
        private IQueryable<CodeDetail> codedetails;

        [TestInitialize]
        public void Setup()
        {
            codedetails = new List<CodeDetail>
            {
                new CodeDetail { ITEM_CODE = "A1", CodeFileID = 1, DESC = "A1 Detail" },
                new CodeDetail { ITEM_CODE = "A2", CodeFileID = 1, DESC = "A2 Detail" },
                new CodeDetail { ITEM_CODE = "B1", CodeFileID = 2, DESC = "B1 Detail" },
                new CodeDetail { ITEM_CODE = "C1", CodeFileID = 3, DESC = "C1 Detail" },
                new CodeDetail { ITEM_CODE = "C2", CodeFileID = 3, DESC = "C2 Detail" },
                new CodeDetail { ITEM_CODE = "C3", CodeFileID = 3, DESC = "C3 Detail" }
            }.AsQueryable();

            codefiles = new List<CodeFile>
            {
                new CodeFile { ID = 1, ITEM_TYPE = "A", TYPE_NAME = "A Name", CodeDetails = codedetails.Where(p => p.CodeFileID == 1).ToList() },
                new CodeFile { ID = 2, ITEM_TYPE = "B", TYPE_NAME = "B Name", CodeDetails = codedetails.Where(p => p.CodeFileID == 2).ToList() },
                new CodeFile { ID = 3, ITEM_TYPE = "C", TYPE_NAME = "C Name", CodeDetails = codedetails.Where(p => p.CodeFileID == 3).ToList() },
            }.AsQueryable();
        }

        [TestMethod]
        public void Test_Get_Codes_A()
        {
            var mockCodesSet = codefiles.MockDbSet();
            var mockDetailsSet = codedetails.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.CodeFiles.Returns(mockCodesSet);
            mockContext.CodeDetails.Returns(mockDetailsSet);

            var codeInitializer = new InitCodeCache(mockContext);
            codeInitializer.Execute();

            var codeCache = new CodeCache();

            var details = codeCache.GetCodeDetails("A");

            Assert.AreEqual(2, details.Count());
            Assert.AreEqual("A", details.First().ItemType);

            var desc = codeCache.GetCodeDesc("A", "A1");
            Assert.AreEqual("A1 Detail", desc);
        }

        [TestMethod]
        public void Test_Get_Codes_B()
        {
            var mockCodesSet = codefiles.MockDbSet();
            var mockDetailsSet = codedetails.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.CodeFiles.Returns(mockCodesSet);
            mockContext.CodeDetails.Returns(mockDetailsSet);

            var codeInitializer = new InitCodeCache(mockContext);
            codeInitializer.Execute();

            var codeCache = new CodeCache();

            var details = codeCache.GetCodeDetails("B");

            Assert.AreEqual(1, details.Count());

            var desc = codeCache.GetCodeDesc("B", "B1");
            Assert.AreEqual("B1 Detail", desc);
        }
    }
}