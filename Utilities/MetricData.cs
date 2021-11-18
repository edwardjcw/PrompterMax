using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    internal class MetricData
    {
        public DateTime Date { get; internal set; }
        public double SpeechToNoise { get; internal set; }
        public int Correct { get; internal set; }

        public MetricData(DateTime modifiedOnDate)
        {
            Date = modifiedOnDate;
        }
    }
}
