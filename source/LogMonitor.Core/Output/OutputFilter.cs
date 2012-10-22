using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (OutputTarget target in targets.Where(t => t.IsFileMatch(change.File)))
            {   
                metrics
                    .Where(m => target.IsMetricMatch(m))
                    .Each(m => target.Process(m));
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
