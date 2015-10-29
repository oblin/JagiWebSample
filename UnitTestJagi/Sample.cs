using AutoMapper;
using Jagi.Mapping;
using System;
using System.ComponentModel.DataAnnotations;

namespace UnitTestJagi
{
    public class Sample
    {
        public int Number { get; set; }
        [Display(Prompt ="請輸入任意文字...")]
        public string Text { get; set; }
        public bool IsChinese { get; set; }
        public DateTime StartDate { get; set; }
        public string EndDate { get; set; }
        public int? NullableInt { get; set; }
        public Decimal FloatingPoint { get; set; }
        [DataType(DataType.MultilineText)]
        public string MultiLine { get; set; }
        public string Dropdown1 { get; set; }
        public string Dropdown2 { get; set; }
        public string Dropdown3 { get; set; }
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

    public class SampleValidation
    {
        [Display(Name ="數字")]
        public int Number { get; set; }
        [Display(Prompt = "請輸入任意文字...", Name = "文本"), Required(ErrorMessage = "請輸入數字"), StringLength(5)]
        public string Text { get; set; }
        [Display(Name ="限制長度"), StringLength(maximumLength: 10, MinimumLength = 4)]
        public string LimitTest { get; set; }
        [Display(Name = "是否中文")]
        public bool IsChinese { get; set; }
        public DateTime StartDate { get; set; }
        [DataType(DataType.MultilineText)]
        public string EndDate { get; set; }
        public int? NullableInt { get; set; }
        public Decimal FloatingPoint { get; set; }
    }
}
