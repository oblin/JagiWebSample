using Jagi.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestJagi
{
    [TestClass]
    public class LinqHelperHasTest
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
                new Sample { Number = 13, Text = "DAb" },
                new Sample { Number = 13, Text = "Def" },
                new Sample { Number = 14, Text = "def" },
                new Sample { Number = 15, Text = "DEf" },
                new Sample { Number = 16, Text = "web" }
            };
        }

        [TestMethod]
        public void Test_Has_ABC_2_IgnoreCase()
        {
            IEnumerable<Sample> result = samples.Has("Text", "ABC");
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void Test_Has_DEF_3_IgnoreCase()
        {
            IEnumerable<Sample> result = samples.Has("Text", "DEF");
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void Test_Has_aaa_0()
        {
            IEnumerable<Sample> result = samples.Has("Text", "aaa");
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Test_Has_FiledName_IgnoreCase_ABC_2()
        {
            IEnumerable<Sample> result = samples.Has("TEXT", "ABC");
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void Test_Has_ABC_CaseSensitive_0()
        {
            IEnumerable<Sample> result = samples.Has("TEXT", "ABC", false);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Test_Has_ABC_CaseSensitive_1()
        {
            IEnumerable<Sample> result = samples.Has("TEXT", "Abc", false);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Test_Has_Ab_3()
        {
            IEnumerable<Sample> result = samples.Has("TEXT", "Ab");
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void Test_Has_Ab_CaseSensitive_2()
        {
            IEnumerable<Sample> result = samples.Has("TEXT", "Ab", false);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void Test_has_null_0()
        {
            IEnumerable<Sample> result = samples.Has("TEXT", null);
            Assert.AreEqual(8, result.Count());
        }
    }
}
