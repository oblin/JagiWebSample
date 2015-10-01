using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestJagi
{
    class Sample
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public bool IsChinese { get; set; }
        public DateTime StartDate { get; set; }
    }

    class SampleCopy
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public DateTime? StartDate { get; set; }
        public Sample ComplexProp { get; set; }
    }

    class SampleCopy2
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
    }
}
