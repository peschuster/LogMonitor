using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LogMonitor.Processors;

namespace LogMonitor
{
    class ChangeManager
    {
        private readonly BlockingCollection<FileChange> inputQueue = new BlockingCollection<FileChange>();

        private readonly BlockingCollection<Metric[]> outputQueue = new BlockingCollection<Metric[]>();

        private readonly IProcessor[] processors;

        private readonly CancellationToken cancellation;
        
        public ChangeManager(IEnumerable<IProcessor> processors)
        {
            if (processors == null)
                throw new ArgumentNullException("processors");

            this.cancellation = new CancellationToken();

            this.processors = processors.ToArray();
        }

        public void Add(FileChange item)
        {
            this.inputQueue.Add(item);
        }

        private void ProcessInput()
        {
            while (!this.cancellation.IsCancellationRequested)
            {
                FileChange item = this.inputQueue.Take(this.cancellation);

                if (this.cancellation.IsCancellationRequested)
                    return;

                IEnumerable<Metric>[] metrics = new IEnumerable<Metric>[this.processors.Length];

                Sequential.For(
                    0, 
                    this.processors.Length,
                    i => metrics[i] = this.processors[i].ParseLine(item));

                this.outputQueue.Add(
                    metrics.SelectMany(m => m).ToArray());
            }
        }

        private void ProcessMetrics()
        {
            while (!this.cancellation.IsCancellationRequested)
            {
                Metric[] metrics = this.outputQueue.Take(this.cancellation);

                if (this.cancellation.IsCancellationRequested)
                    return;
            }
        }
    }
}
