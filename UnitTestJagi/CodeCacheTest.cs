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
                new CodeDetail { ID = 1, ITEM_CODE = "A1", CodeFileID = 1, DESC = "A1 Detail" },
                new CodeDetail { ID = 2, ITEM_CODE = "A2", CodeFileID = 1, DESC = "A2 Detail" },
                new CodeDetail { ID = 3, ITEM_CODE = "B1", CodeFileID = 2, DESC = "B1 Detail" },
                new CodeDetail { ID = 4, ITEM_CODE = "C1", CodeFileID = 3, DESC = "C1 Detail" },
                new CodeDetail { ID = 5, ITEM_CODE = "C2", CodeFileID = 3, DESC = "C2 Detail" },
                new CodeDetail { ID = 6, ITEM_CODE = "C3", CodeFileID = 3, DESC = "C3 Detail" }
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

            var codeInitializer = new CodeCacheManager(mockContext);
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

            var codeInitializer = new CodeCacheManager(mockContext);
            codeInitializer.Execute();

            var codeCache = new CodeCache();

            var details = codeCache.GetCodeDetails("B");

            Assert.AreEqual(1, details.Count());

            var desc = codeCache.GetCodeDesc("B", "B1");
            Assert.AreEqual("B1 Detail", desc);
        }

        [TestMethod]
        public void Test_Remove_Code_C()
        {
            var mockCodesSet = codefiles.MockDbSet();
            var mockDetailsSet = codedetails.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.CodeFiles.Returns(mockCodesSet);
            mockContext.CodeDetails.Returns(mockDetailsSet);

            var codeInitializer = new CodeCacheManager(mockContext);
            codeInitializer.Execute();

            var codeCache = new CodeCache();

            var codes = codeCache.GetCodeDetails("C");

            Assert.AreEqual(3, codes.Count());

            codeInitializer.RemoveCodeFile(3);

            codes = codeCache.GetCodeDetails("C");

            Assert.AreEqual(0, codes.Count());
        }

        [TestMethod]
        public void Test_Remove_Detail_C1()
        {
            var mockCodesSet = codefiles.MockDbSet();
            var mockDetailsSet = codedetails.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.CodeFiles.Returns(mockCodesSet);
            mockContext.CodeDetails.Returns(mockDetailsSet);

            var codeInitializer = new CodeCacheManager(mockContext);
            codeInitializer.Execute();

            var codeCache = new CodeCache();

            var codes = codeCache.GetCodeDetails("C");
            // 確認 C 原來有三個
            Assert.AreEqual(3, codes.Count());

            // 刪除 C3 
            codeInitializer.RemoveCodeDetail(6);
            // C 只剩下兩個
            codes = codeCache.GetCodeDetails("C");
            Assert.AreEqual(2, codes.Count());
            // 檢查 C3 確定被刪除
            var detail = codeCache.GetCodeDesc("C", "C3");
            Assert.IsTrue(string.IsNullOrEmpty(detail));
            // 檢查 C1 還存在
            detail = codeCache.GetCodeDesc("C", "C1");
            Assert.AreEqual("C1 Detail", detail);
        }

        [TestMethod]
        public void Test_Add_Code_D()
        {
            var mockCodesSet = codefiles.MockDbSet();
            var mockDetailsSet = codedetails.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.CodeFiles.Returns(mockCodesSet);
            mockContext.CodeDetails.Returns(mockDetailsSet);

            var codeManager = new CodeCacheManager(mockContext);
            codeManager.Execute();

            var codeCache = new CodeCache();

            var code = new CodeFile { ID = 4, ITEM_TYPE = "D", TYPE_NAME = "D Name" };
            code.CodeDetails = new List<CodeDetail> { 
                new CodeDetail { CodeFileID = 4, ITEM_CODE = "D1", DESC = "D1 Detail"},
                new CodeDetail { CodeFileID = 4, ITEM_CODE = "D2", DESC = "D2 Detail"},
            };

            var newCodes = codefiles.ToList();       // 將新增資料存入 database
            newCodes.Add(code);

            mockCodesSet = newCodes.AsQueryable().MockDbSet();
            mockContext.CodeFiles.Returns(mockCodesSet);
            codeManager = new CodeCacheManager(mockContext);

            var codes = codeCache.GetCodeDetails("D");
            // 確認原來 cache has no D
            Assert.AreEqual(0, codes.Count());

            codeManager.SetCodeFile(4);        // 使用 database 讀取 codeFile.Id = 4 並存入 cache 中

            // D 變成兩筆
            codes = codeCache.GetCodeDetails("D");
            Assert.AreEqual(2, codes.Count());
            // 檢查 D1 存在
            var detail = codeCache.GetCodeDesc("D", "D1");
            Assert.AreEqual("D1 Detail", detail);
        }

        [TestMethod]
        public void Test_Update_Code_C()
        {
            var mockCodesSet = codefiles.MockDbSet();
            var mockDetailsSet = codedetails.MockDbSet();
            var mockContext = Substitute.For<DataContext>();
            mockContext.CodeFiles.Returns(mockCodesSet);
            mockContext.CodeDetails.Returns(mockDetailsSet);

            var codeManager = new CodeCacheManager(mockContext);
            codeManager.Execute();
            var codeCache = new CodeCache();

            var codes = codeCache.GetCodeDetails("C");
            Assert.AreEqual(3, codes.Count());
            // 測試更改 detail 資料
            var detail = codedetails.FirstOrDefault(p => p.ID == 5);
            detail.DESC = "Changed!";

            codeManager.SetCodeFile(3);

            var desc = codeCache.GetCodeDesc("C", "C2");
            Assert.AreEqual("Changed!", desc);
            // 測試新增 detail 資料
            var codefile = codefiles.FirstOrDefault(p => p.ID == 3);
            codefile.CodeDetails.Add(new CodeDetail { ITEM_CODE = "C4", DESC = "C4 Desc" });

            codeManager.SetCodeFile(3);

            desc = codeCache.GetCodeDesc("C", "C4");
            Assert.AreEqual("C4 Desc", desc);
        }
    }
}