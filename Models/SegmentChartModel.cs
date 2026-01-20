using System;
using System.Collections.Generic;
using System.Text;

namespace PeopleCounterDesktop.Models
{
    public class SegmentChartModel
    {
        public int SegmentId { get; set; }
        public DateTime Time { get; set; }
        public long TotalIn { get; set; }
        public long TotalOut { get; set; }
    }

    public class SensorTrendDto
    {
        public DateTime Time { get; set; }
        public int In { get; set; }
        public int Out { get; set; }
    }

}
