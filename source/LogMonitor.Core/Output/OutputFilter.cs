using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LogMonitor.Helpers;

namespace LogMonitor.Output
{
    public class OutputFilter : IDisposable
    {
        private readonly IList<IOutputBackend> backends;

        private readonly IList<OutputTarget> targets;

        private bool disposed;

        public OutputFilter(IList<OutputTarget> targets, IList<IOutputBackend> backends)
        {
            this.targets = targets;
            this.backends = backends;
        }

        public void Process(FileChange change, Metric[] metrics)
        {
            if (this.disposed)
                throw new ObjectDisposedException(typeof(OutputFilter).Name);

            var matchedTargets = targets.Where(t => t.IsFileMatch(change.File));

            if (!matchedTargets.Any())
            {
                Trace.TraceWarning("Not output conifguration matches the file \"{0}\".", change.File);

                return;
            }

            List<Metric> processed = new List<Metric>();

            foreach (OutputTarget target in matchedTargets)
            {
                metrics
                    .Where(m => target.IsMetricMatch(m))
                    .Each(m => 
                    { 
                        target.Process(m);
                        processed.Add(m);
                    });
            }

            var warnings = new StringBuilder();

            foreach (Metric metric in metrics.Except(processed))
            {
                warnings.AppendLine("Not output configuration matches the metric \"{0}\", \"{1}\".".FormatWith(metric.Key, metric.Type));
            }

            if (warnings.Length > 0)
            {
                Trace.TraceWarning(warnings.ToString());
            }
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
                this.targets.OfType<IDisposable>().Each(d => d.Dispose());

                this.backends.OfType<IDisposable>().Each(d => d.Dispose());

                this.disposed = true;
            }
        }
    }
}
