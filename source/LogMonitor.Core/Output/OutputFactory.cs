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
        public OutputFilter CreateFilter(IEnumerable<IOutputConfiguration> configurations, IGraphiteConfiguration graphiteConfiguration, IStatsDConfiguration statsDConfiguration)
        {
            Lazy<GraphiteBackend> graphite = new Lazy<GraphiteBackend>(() => new GraphiteBackend(graphiteConfiguration));
            Lazy<StatsDBackend> statsD = new Lazy<StatsDBackend>(() => new StatsDBackend(statsDConfiguration));

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

            var filter = new OutputFilter(targets, backends);

            return filter;
        }
    }
}
