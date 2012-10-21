using System;

namespace LogMonitor.Processors
{
    public class Metric
    {
        public string Key { get; set; }

        public float Value { get; set; }

        public string Type { get; set; }

        public DateTime? Timestamp { get; set; }

        public static Metric Create(string key, float value, string type)
        {
            return new Metric
            {
                Key = key,
                Value = value,
                Type = type,
                Timestamp = DateTime.Now,
            };
        }
    }
}
