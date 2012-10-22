using System;
using System.Collections.Generic;
using System.Linq;
using Graphite;
using Graphite.Configuration;
using Graphite.Infrastructure;
using LogMonitor.Helpers;

namespace LogMonitor.Output
{
    internal class StatsDBackend : IOutputBackend, IDisposable
    {
        private static readonly string[] supportedTypes = new[] { MetricType.Counter, MetricType.Gauge, MetricType.Timing };

        private readonly ChannelFactory factory;

        private readonly IDictionary<string, IMonitoringChannel> channels;

        private bool disposed;

        public StatsDBackend(IStatsDConfiguration configuration)
        {
            this.factory = new ChannelFactory(null, configuration);

            this.channels = new Dictionary<string, IMonitoringChannel>
            {
                { MetricType.Counter, this.factory.CreateChannel("counter", "statsd") },
                { MetricType.Gauge, this.factory.CreateChannel("gauge", "statsd") },
                { MetricType.Timing, this.factory.CreateChannel("timing", "statsd") },
            };
        }

        public string[] SupportedTypes
        {
            get { return supportedTypes; }
        }

        public void Submit(Metric metric, string metricsPrefix)
        {
            if (this.disposed)
                throw new ObjectDisposedException(typeof(StatsDBackend).Name);

            if (!supportedTypes.Contains(metric.Type, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Metric type not supported.", "metric");

            string normalizedType = supportedTypes.First(s => s.Equals(metric.Type, StringComparison.OrdinalIgnoreCase));

            this.channels[normalizedType].Report(
                Helper.BuildKey(metric.Key, metricsPrefix), 
                (int)metric.Value);
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                if (this.factory != null)
                    this.factory.Dispose();

                this.disposed = true;
            }
        }
    }
}
