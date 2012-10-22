using System;
using System.Linq;
using Graphite;
using Graphite.Configuration;
using Graphite.Infrastructure;

namespace LogMonitor.Output
{
    internal class GraphiteBackend : IOutputBackend, IDisposable
    {
        private static readonly string[] supportedTypes = new[] { MetricType.Raw, MetricType.Gauge };

        private readonly ChannelFactory factory;

        private readonly IMonitoringChannel graphiteChannel;

        private bool disposed;

        public GraphiteBackend(IGraphiteConfiguration configuration)
        {
            this.factory = new ChannelFactory(configuration, null);

            this.graphiteChannel = this.factory.CreateChannel("gauge", "graphite");
        }

        public string[] SupportedTypes
        {
            get
            {
                return supportedTypes;
            }
        }

        public void Submit(Metric metric, string metricsPrefix)
        {
            if (this.disposed)
                throw new ObjectDisposedException(typeof(StatsDBackend).Name);

            if (!supportedTypes.Contains(metric.Type, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Metric type not supported.", "metric");

            this.graphiteChannel.Report(
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