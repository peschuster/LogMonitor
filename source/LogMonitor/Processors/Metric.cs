using System;

namespace LogMonitor.Processors
{
    public class Metric
    {
        public string Key { get; set; }

        public float Value { get; set; }

        public string Type { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
