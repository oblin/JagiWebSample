using System;
using Jagi.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UnitTestJagi
{
    [TestClass]
    public class MetadataTest
    {
        [TestMethod]
        public void Test_Sample_Object_ToJson()
        {
            var mvc = new MockMvc();
            var sampleModel = mvc.CreateHtmlHelper<SampleValidation>();

            var result = sampleModel.ValidationsFor<SampleValidation>();

            var resultObject = JsonConvert.DeserializeObject<List<dynamic>>(result.ToString());

            Assert.AreEqual(6, resultObject.Count);
        }
    }
}
