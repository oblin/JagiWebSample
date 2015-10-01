using Jagi.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestJagi
{
    [TestClass]
    public class ObjectHelperCopyToTest
    {
        private List<SampleCopy2> samples;

        [TestInitialize]
        public void Setup()
        {
            samples = new List<SampleCopy2>
            {
                new SampleCopy2 { Number = 11, Text = "Abc" },
                new SampleCopy2 { Number = 12, Text = "ABc" },
                new SampleCopy2 { Number = 16, Text = "web", StartDate = "2015/3/2".ConvertToDateTime() }
            };
        }

        [TestMethod]
        public void Test_CopyTo_11()
        {
            var result = samples[0].CopyTo();
            Assert.AreEqual(11, result.Number);

            result = new SampleCopy2();
            samples[1].CopyTo(result);
            Assert.AreEqual(12, result.Number);
        }

        [TestMethod]
        public void Test_CopyTo_Date()
        {
            var result = samples[2].CopyTo();
            Assert.AreEqual(samples[2].StartDate.GetValueOrDefault().ToShortDateString(), result.StartDate.GetValueOrDefault().ToShortDateString());
        }

        [TestMethod]
        public void Test_CopyToExclueNull_Date()
        {
            var result = samples[1].CopyToExcludeNull();
            Assert.IsNull(result.StartDate);

            // 測試 Exclude null 不會將空白資料 copy to 欄位
            result = new SampleCopy2();
            DateTime targetDate = "2015/03/04".ConvertToDateTime();
            result.StartDate = targetDate;
            samples[1].CopyToExcludeNull(result);
            Assert.AreEqual(targetDate, result.StartDate);

            // 測試 Copy 會將 null copy to 欄位
            samples[1].CopyTo(result);
            Assert.IsNull(result.StartDate);
        }

        [TestMethod]
        public void Test_CopyTo_Complex_Property()
        {
            SampleCopy2 source = new SampleCopy2
            {
                Number = 1,
                Text = "ComplexField"
            };

            var result = source.CopyTo();
            Assert.AreEqual(1, result.Number);
            Assert.IsNull(result.StartDate);

            DateTime testDate = "2012/2/2".ConvertToDateTime();
            source.ComplexProp = new Sample { Number = 10, Text = "Detail", StartDate = testDate };
            result = source.CopyTo();
            Assert.AreEqual(1, result.Number);
            Assert.AreEqual(10, result.ComplexProp.Number);
            Assert.AreEqual("Detail", result.ComplexProp.Text);
            Assert.AreEqual(testDate, result.ComplexProp.StartDate);
        }

        [TestMethod]
        public void Test_CopyToExclude_Complex_Property()
        {
            SampleCopy2 source = new SampleCopy2
            {
                Number = 1,
                Text = "ComplexField"
            };

            Assert.IsNull(source.ComplexProp);

            Sample testSample = new Sample { Number = 10, Text = "Detail" };
            DateTime testDate = "2012/2/2".ConvertToDateTime();
            SampleCopy2 target = new SampleCopy2
            {
                StartDate = testDate,
                ComplexProp = testSample
            };

            source.CopyToExcludeNull(target);
            Assert.AreEqual(1, target.Number);
            Assert.IsNotNull(target.ComplexProp);
            Assert.AreEqual(10, target.ComplexProp.Number);
            Assert.AreEqual("Detail", target.ComplexProp.Text);
        }

        [TestMethod]
        public void Test_CopyTo_Differenct_Class()
        {
            Sample testSample = new Sample { Number = 10, Text = "Detail" };
            DateTime testDate = "2012/2/2".ConvertToDateTime();
            SampleCopy2 source = new SampleCopy2
            {
                Number = 1,
                Text = "ComplexField",
                StartDate = testDate,
                ComplexProp = testSample
            };

            SampleCopy3 target = new SampleCopy3
            {
                Id = 1,
                Number = 2
            };

            source.CopyTo(target);

            Assert.AreEqual(1, target.Number);
            Assert.AreEqual(1, target.Id);
        }
    }
}
