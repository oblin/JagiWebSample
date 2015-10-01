using Jagi.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestJagi
{
    [TestClass]
    public class LinqHelperEqualTest
    {
        private List<Sample> samples;

        [TestInitialize]
        public void Setup()
        {
            samples = new List<Sample>
            {
                new Sample { Number = 11, Text = "Abc" },
                new Sample { Number = 12, Text = "ABc" },
                new Sample { Number = 13, Text = "AbD" },
                new Sample { Number = 13, Text = "Def" },
                new Sample { Number = 14, Text = "def" },
                new Sample { Number = 15, Text = "DEf", IsChinese = false },
                new Sample { Number = 13, Text = "DAb", IsChinese = true },
                new Sample { Number = 16, Text = "web", IsChinese = true },
                new Sample { Number = 16, Text = "", IsChinese = true },
            };
        }

        [TestMethod]
        public void Test_Equal_ABC_2_IgnoreCase()
        {
            IEnumerable<Sample> result = samples.Equal("Text", "ABC");
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void Test_Equal_DEF_3_IgnoreCase()
        {
            IEnumerable<Sample> result = samples.Equal("Text", "DEF");
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void Test_Equal_aaa_0()
        {
            IEnumerable<Sample> result = samples.Equal("Text", "aaa");
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Test_Equal_FiledName_IgnoreCase_ABC_2()
        {
            IEnumerable<Sample> result = samples.Equal("TEXT", "ABC");
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void Test_Equal_ABC_CaseSensitive_0()
        {
            IEnumerable<Sample> result = samples.Has("TEXT", "ABC", false);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Test_Equal_Abc_CaseSensitive_1()
        {
            IEnumerable<Sample> result = samples.Equal("TEXT", "Abc", false);
            var res = samples.Where(p => p.Text == "Abc");
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Test_Equal_Ab_0()
        {
            IEnumerable<Sample> result = samples.Equal("TEXT", "Ab");
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Test_Equal_Ab_CaseSensitive_0()
        {
            IEnumerable<Sample> result = samples.Equal("TEXT", "Ab", false);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Test_Equal_EmptyString_1()
        {
            IEnumerable<Sample> result = samples.Equal("TEXT", "");
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Test_Equal_int_10_0()
        {
            IEnumerable<Sample> result = samples.Equal("Number", 10);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Test_Equal_int_11_1()
        {
            IEnumerable<Sample> result = samples.Equal("Number", 11);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Test_Equal_int_13_3()
        {
            IEnumerable<Sample> result = samples.Equal("Number", 13);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void Test_Equal_DateTime()
        {
            DateTime testDate = "2015/11/1".ConvertToDateTime();
            samples[1].StartDate = testDate;
            samples[4].StartDate = testDate;
            IEnumerable<Sample> result = samples.Equal("StartDate", testDate);
            Assert.AreEqual(2, result.Count());
        }
        
        [TestMethod]
        public void Test_Equal_bool_IsChinese_2()
        {
            IEnumerable<Sample> result = samples.Equal("IsChinese", true);
            Assert.AreEqual(3, result.Count());
        }
    }
}
