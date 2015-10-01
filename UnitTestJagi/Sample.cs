using AutoMapper;
using Jagi.Mapping;
using System;

namespace UnitTestJagi
{
    public class Sample
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public bool IsChinese { get; set; }
        public DateTime StartDate { get; set; }
        public string EndDate { get; set; }
        public int? NullableInt { get; set; }
    }

    public class SampleCopy1 : IMapFrom<Sample>
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class SampleCopy2 : IMapFrom<Sample>
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public DateTime? StartDate { get; set; }
        public Sample ComplexProp { get; set; }
    }

    public class SampleCopy3 : IMapFrom<Sample>
    {
        public int Id { get; set; }
        public int Number { get; set; }
        [IgnoreMap]
        public string Text { get; set; }
        public DateTime? EndDate { get; set; }
        public string NullableInt { get; set; }
    }

    public class SampleCopy4 : IMapFrom<Sample>
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public string StartDate { get; set; }
    }
}
