using Jagi.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;

namespace UnitTestJagi
{
    /// <summary>
    /// Summary description for JsonHelperTest
    /// </summary>
    [TestClass]
    public class JsonHelperTest
    {
        [TestMethod]
        public void Test_Sample_Object_ToJson()
        {
            Sample sample = new Sample { Number = 11, Text = "Sample Text" };
            string jsonString = sample.ToJson();

            bool isCamelText = jsonString.IndexOf("text", 0, StringComparison.Ordinal) > 0;
            Assert.IsTrue(isCamelText);

            bool isPascalNumber = jsonString.IndexOf("Number", 0, StringComparison.Ordinal) > 0;
            Assert.IsFalse(isPascalNumber);
        }

        [TestMethod]
        public void Test_Deserialized_Json_To_Dynamic()
        {
            Sample sample = new Sample { Number = 11, Text = "Sample Text" };
            string jsonString = sample.ToJson();

            var resultObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

            Assert.AreEqual(11, (int)resultObject.number);
            Assert.AreEqual("Sample Text", (string)resultObject.text);

            Assert.AreEqual("11", (string)resultObject["number"]);
        }

        [TestMethod]
        public void Test_Deserialized_Json_To_Dynamic_Without_Null()
        {
            SampleCopy2 sample = new SampleCopy2 { Number = 11, Text = "Sample Text" };
            string jsonString = sample.ToJson(false);

            var resultObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

            Assert.AreEqual(11, (int)resultObject.number);
            Assert.IsNull(resultObject.startDate);

            sample = new SampleCopy2 { Number = 11, Text = "Sample Text", StartDate = DateTime.Now };
            jsonString = sample.ToJson(false);

            resultObject = JsonConvert.DeserializeObject<dynamic>(jsonString);

            Assert.AreEqual(11, (int)resultObject.number);
            Assert.IsNotNull(resultObject.startDate);

            Assert.IsInstanceOfType((DateTime)resultObject.startDate, typeof(DateTime));
        }

        [TestMethod]
        public void Test_Expression_ToCamelCase_Number()
        {
            Expression<Func<Sample, int>> expr = a => a.Number;

            string result = expr.ToCamelCaseName();

            Assert.AreEqual("number", result);
        }

        [TestMethod]
        public void Test_Expression_ToCamelCase_ComplexProp_Number()
        {
            Expression<Func<SampleCopy2, int>> expr = a => a.ComplexProp.Number;

            string result = expr.ToCamelCaseName();

            Assert.AreEqual("complexProp.number", result);
        }
    }
}
