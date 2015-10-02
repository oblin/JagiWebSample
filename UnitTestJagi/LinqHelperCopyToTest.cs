using Jagi.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestJagi
{
    [TestClass]
    public class LinqHelperCopyToTest
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
            };
        }

        [TestMethod]
        public void Test_CopyTo_SampleCopy_IgnoreCase()
        {
            IEnumerable<SampleCopy2> result = samples.CopyTo<Sample, SampleCopy2>();
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(11, result.ToList()[0].Number);
            Assert.AreEqual("ABc", result.ToList()[1].Text);
        }

        [TestMethod]
        public void Test_CopyTo_SampleCopy2_IgnoreCase()
        {
            var result = samples.CopyTo<Sample, SampleCopy3>();
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(11, result.ToList()[0].Number);
            Assert.AreEqual("ABc", result.ToList()[1].Text);
        }
    }
}
