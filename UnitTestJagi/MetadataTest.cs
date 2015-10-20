using System;
using Jagi.Mvc.Angular;
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

            var jsonResult = sampleModel.ValidationsFor<SampleValidation>();
            var stringResult = jsonResult.ToString();
            Assert.IsTrue(stringResult.Contains("\"message\":\"欄位 【數字】 必須要輸入\""));
            Assert.IsTrue(stringResult.Contains("[{\"propertyName\":\"number\",\"rules\":{\"required\":{\"message\":\"欄位 【數字】 必須要輸入\"}}}"));

            var resultObject = JsonConvert.DeserializeObject<List<dynamic>>(stringResult);
            Assert.AreEqual(6, resultObject.Count);
        }
    }
}
