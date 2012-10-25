using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Graphite.Configuration;
using LogMonitor.Configuration;
using LogMonitor.Helpers;
using LogMonitor.Output;
using LogMonitor.Processors;

namespace LogMonitor
{
    internal class ConfigurationAppStarter : IDisposable
    {
        private bool disposed;

        private readonly List<IProcessor> processors;

        private readonly OutputFilter outputFilter;

        public ConfigurationAppStarter()
        {
            var processorsFactory = new ProcessorFactory();

            LogMonitorConfiguration configuration = LogMonitorConfiguration.Instance;

            this.processors = new List<IProcessor>();

            // Set to right "context"
            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(Kernel).Assembly.Location);

            foreach (ParserElement parser in configuration.Parser)
            {
                processors.Add(processorsFactory.Create(parser.ScriptPath, parser.Pattern));
            }

            var outputFactory = new OutputFactory(
                GraphiteConfiguration.Instance == null ? null : GraphiteConfiguration.Instance.Graphite,
                GraphiteConfiguration.Instance == null ? null : GraphiteConfiguration.Instance.StatsD);

            this.outputFilter = outputFactory.CreateFilter(
                configuration.Output.Cast<IOutputConfiguration>());
        }

        public Kernel Start()
        {
            LogMonitorConfiguration configuration = LogMonitorConfiguration.Instance;

            return new Kernel(
                this.processors,
                configuration.Watch.Cast<IWatchConfiguration>(),
                this.outputFilter);
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
                if (this.outputFilter != null)
                    this.outputFilter.Dispose();

                this.processors.OfType<IDisposable>().Each(d => d.Dispose());

                this.disposed = true;
            }
        }
    }
}
