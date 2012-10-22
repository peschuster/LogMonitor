using System;
using System.Collections.Generic;
using System.Linq;
using Graphite.Configuration;
using LogMonitor.Configuration;
using LogMonitor.Helpers;
using LogMonitor.Output;
using LogMonitor.Processors;

namespace LogMonitor
{
    public static class Program
    {
        public static void Main()
        {
            var processorsFactory = new ProcessorFactory();

            LogMonitorConfiguration configuration = LogMonitorConfiguration.Instance;

            var processors = new List<IProcessor>();

            foreach (ParserElement parser in configuration.Parser)
            {
                processors.Add(processorsFactory.Create(parser.ScriptPath, parser.Pattern));
            }

            var outputFactory = new OutputFactory(
                GraphiteConfiguration.Instance == null ? null : GraphiteConfiguration.Instance.Graphite,
                GraphiteConfiguration.Instance == null ? null : GraphiteConfiguration.Instance.StatsD);

            using (OutputFilter outputFilter = outputFactory.CreateFilter(
                configuration.Output.Cast<IOutputConfiguration>()))
            {
                using (new Kernel(
                    processors,
                    configuration.Watch.Cast<IWatchConfiguration>(),
                    outputFilter))
                {
                    Console.ReadLine();
                }
            }

            processors.OfType<IDisposable>().Each(d => d.Dispose());
        }
    }
}
