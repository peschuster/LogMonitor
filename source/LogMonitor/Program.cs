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

            var preProcessors = new Dictionary<string, IPreProcessor>();
            var filters = new Dictionary<string, string>();

            Lazy<W3CProcessor> w3cProcessor = new Lazy<W3CProcessor>(() => new W3CProcessor());
            DefaultPreProcessor preProcessor = new DefaultPreProcessor();

            foreach (WatchElement element in configuration.Watch)
            {
                if (!string.IsNullOrEmpty(element.Type) && "w3c".Equals(element.Type, StringComparison.OrdinalIgnoreCase))
                {
                    preProcessors.Add(element.Path, w3cProcessor.Value);
                }
                else
                {
                    preProcessors.Add(element.Path, preProcessor);
                }

                if (!string.IsNullOrEmpty(element.Filter))
                {
                    filters.Add(element.Path, element.Filter);
                }
                else
                {
                    filters.Add(element.Path, "*");
                }
            }

            var outputFactory = new OutputFactory();

            using (OutputFilter outputFilter = outputFactory.CreateFilter(
                configuration.Output.Cast<IOutputConfiguration>(),
                GraphiteConfiguration.Instance.Graphite,
                GraphiteConfiguration.Instance.StatsD))
            {
                using (new Kernel(
                    processors,
                    preProcessors,
                    filters,
                    outputFilter))
                {
                    Console.ReadLine();
                }
            }

            processors.OfType<IDisposable>().Each(d => d.Dispose());
        }
    }
}
