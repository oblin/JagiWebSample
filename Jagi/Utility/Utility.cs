﻿using System;
using System.Diagnostics;

namespace Jagi.Utility
{
    public class Utility
    {
        public static TimeSpan Timing(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}
