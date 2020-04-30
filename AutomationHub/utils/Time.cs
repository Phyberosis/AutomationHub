using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHub.utils
{
    public class Time
    {
        public static long millis()
        {
            return Stopwatch.GetTimestamp() / (Stopwatch.Frequency * 1000);
        }
    }
}
