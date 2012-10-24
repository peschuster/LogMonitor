using System;

namespace LogMonitor.Output
{
    internal class ConsoleBackend : IOutputBackend
    {
        private static readonly string[] supportedTypes = new[] { MetricType.Counter, MetricType.Gauge, MetricType.Timing, MetricType.Raw };

        public string[] SupportedTypes
        {
            get { return supportedTypes; }
        }

        public void Submit(Metric metric, string metricsPrefix)
        {
            Console.WriteLine("{0,-40} {1,-8} {2,6} {3,-10}", Helper.BuildKey(metric.Key, metricsPrefix), metric.Type, metric.Value, metric.Timestamp);
        }
    }
}
