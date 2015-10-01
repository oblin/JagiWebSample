using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jagi.Helpers;
using Jagi.Mapping;
using AutoMapper;
using System.Reflection;

namespace UnitTestJagi
{
    [TestClass]
    public class ConfigureAutoMapperTest
    {
        private Sample sample;

        [TestInitialize]
        public void Setup()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            MapperConfig config = new MapperConfig(assembly);
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
        public void Test_Mapper_Customer_Config_DateTime()
        {
            SampleCopy4 copy = Mapper.Map<SampleCopy4>(sample);

            Assert.AreEqual(1, copy.Number);
            Assert.AreEqual("Test", copy.Text);
            Assert.AreEqual("2015/02/02", copy.StartDate);
        }
    }

    class MapperConfig : MappingBaseConfig
    {
        public MapperConfig(Assembly assembly) : base(assembly)
        {

        }

        public override void ConverterSettings()
        {
            Mapper.CreateMap<DateTime, string>().ConvertUsing<DateTimeToString>();

            base.ConverterSettings();
        }
    }

    class DateTimeToString : TypeConverter<DateTime, string>
    {
        protected override string ConvertCore(DateTime source)
        {
            return source.ToString("yyyy/MM/dd");
        }
    }
}
