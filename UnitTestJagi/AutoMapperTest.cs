using AutoMapper;
using Jagi.Helpers;
using Jagi.Mapping;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace UnitTestJagi
{
    [TestClass]
    public class AutoMapperTest
    {
        private Sample sample;

        [TestInitialize]
        public void Setup() 
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            MappingBaseConfig config = new MappingBaseConfig(assembly);
            config.Execute();

            sample = new Sample
            {
                Number = 1,
                Text = "Test",
                IsChinese = false,
                StartDate = "2015/02/2".ConvertToDateTime()
            };

        }
        
        [TestMethod]
        public void Test_Sample_MapTo_SampleCopy1()
        {
            SampleCopy1 copy = Mapper.Map<SampleCopy1>(sample);
            
            Assert.AreEqual(1, copy.Number);
            Assert.AreEqual("Test", copy.Text);
            Assert.AreEqual(sample.StartDate, copy.StartDate);
        }

        [TestMethod]
        public void Test_Sample_MapTo_SampleCopy2()
        {
            SampleCopy2 copy = Mapper.Map<SampleCopy2>(sample);

            Assert.AreEqual(1, copy.Number);
            Assert.AreEqual("Test", copy.Text);
            Assert.AreEqual(sample.StartDate, copy.StartDate);
        }

        [TestMethod]
        public void Test_Sample_MapTo_SampleCopy3_IgnoreMapAttribute()
        {
            SampleCopy3 copy = Mapper.Map<SampleCopy3>(sample);

            Assert.AreEqual(1, copy.Number);
            Assert.IsNull(copy.Text);
        }

        [TestMethod]
        public void Test_Sample_MapTo_String_DateTime()
        {
            SampleCopy3 copy = Mapper.Map<SampleCopy3>(sample);
            Assert.AreEqual(1, copy.Number);
            Assert.IsNull(copy.EndDate);

            sample.EndDate = "2015/3/3";
            copy = Mapper.Map<SampleCopy3>(sample);
            Assert.AreEqual("2015/3/3".ConvertToDateTime(), copy.EndDate);
        }

        [TestMethod]
        public void Test_Sample_MapTo_NullableInt_String()
        {
            SampleCopy3 copy = Mapper.Map<SampleCopy3>(sample);
            Assert.AreEqual(1, copy.Number);
            Assert.IsTrue(string.IsNullOrEmpty(copy.NullableInt));

            sample.NullableInt = 1234;
            copy = Mapper.Map<SampleCopy3>(sample);
            Assert.AreEqual("1234", copy.NullableInt);
        }
    }
}
