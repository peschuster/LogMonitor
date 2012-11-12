using System;

namespace LogMonitor
{
    public class Metric
    {
        public string Key { get; set; }

        public float Value { get; set; }

        public string Type { get; set; }

        public static Metric Create(string key, float value, string type)
        {
            return new Metric
            {
                Key = string.Intern(key),
                Value = value,
                Type = type,
            };
        }
    }
}
