using Jagi.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestJagi
{
    [TestClass]
    public class LinqHelperDateCompareTest
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
                new Sample { Number = 16, Text = "web", StartDate = "2015/3/2".ConvertToDateTime() }
            };
        }

        [TestMethod]
        public void Test_Greater_20150301_1()
        {
            IEnumerable<Sample> result = samples.DateGreaterThanOrEqual("StartDate", "2015/03/01".ConvertToDateTime());
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Test_Less_20150301_7()
        {
            IEnumerable<Sample> result = samples.DateLessThanOrEqual("StartDate", "2015/03/01".ConvertToDateTime());
            Assert.AreEqual(7, result.Count());
        }

        [TestMethod]
        public void Test_Less_20150302_8()
        {
            IEnumerable<Sample> result = samples.DateLessThanOrEqual("StartDate", "2015/03/02".ConvertToDateTime());
            Assert.AreEqual(8, result.Count());
        }
    }
}
