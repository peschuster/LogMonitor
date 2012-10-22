using System;
using System.Collections.Generic;
using System.Linq;
using Graphite.Configuration;
using LogMonitor.Configuration;
using LogMonitor.Helpers;

namespace LogMonitor.Output
{
    public class OutputFactory
    {
        private readonly IStatsDConfiguration statsDConfiguration;

        private readonly IGraphiteConfiguration graphiteConfiguration;

        public OutputFactory(IGraphiteConfiguration graphiteConfiguration, IStatsDConfiguration statsDConfiguration)
        {
            this.graphiteConfiguration = graphiteConfiguration;
            this.statsDConfiguration = statsDConfiguration;
        }

        public OutputFilter CreateFilter(IEnumerable<IOutputConfiguration> configurations)
        {
            Lazy<GraphiteBackend> graphite = new Lazy<GraphiteBackend>(() => new GraphiteBackend(this.graphiteConfiguration));
            Lazy<StatsDBackend> statsD = new Lazy<StatsDBackend>(() => new StatsDBackend(this.statsDConfiguration));
            Lazy<ConsoleBackend> console = new Lazy<ConsoleBackend>(() => new ConsoleBackend());

            List<OutputTarget> targets = new List<OutputTarget>();

            foreach (var configuration in configurations)
            {
                IOutputBackend backend = null;

                if ("graphite".Equals(configuration.Target, StringComparison.OrdinalIgnoreCase))
                {
                    backend = graphite.Value;
                }
                else if ("statsd".Equals(configuration.Target, StringComparison.OrdinalIgnoreCase))
                {
                    backend = statsD.Value;
                }
                else if ("console".Equals(configuration.Target, StringComparison.OrdinalIgnoreCase))
                {
                    backend = console.Value;
                }
                else
                {
                    throw new ArgumentException("Unknown output target \"{0}\".".FormatWith(configuration.Target), "configurations");
                }

                var target = new OutputTarget(
                    configuration.PathPattern, 
                    configuration.Type, 
                    configuration.MetricsPrefix, 
                    backend);

                targets.Add(target);
            }

            List<IOutputBackend> backends = new List<IOutputBackend>();

            if (graphite.IsValueCreated)
                backends.Add(graphite.Value);

            if (statsD.IsValueCreated)
                backends.Add(statsD.Value);

            if (console.IsValueCreated)
                backends.Add(console.Value);

            var filter = new OutputFilter(targets, backends);

            return filter;
        }
    }
}
