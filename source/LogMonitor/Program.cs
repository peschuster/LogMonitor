using System;
using System.Collections.Generic;
using System.Linq;
using LogMonitor.Configuration;
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

            W3CProcessor w3cProcessor = null;
            DefaultPreProcessor preProcessor = new DefaultPreProcessor();

            foreach (WatchElement element in configuration.Watch)
            {
                if (!string.IsNullOrEmpty(element.Type) && "w3c".Equals(element.Type, StringComparison.OrdinalIgnoreCase))
                {
                    preProcessors.Add(element.Path, w3cProcessor ?? (w3cProcessor = new W3CProcessor()));
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

            using (new Kernel(
                processors,
                preProcessors,
                filters))
            {
                Console.ReadLine();
            }

            foreach (IDisposable disposable in processors.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }
    }
}
