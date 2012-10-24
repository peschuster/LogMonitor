using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogMonitor.Helpers;
using LogMonitor.Output;
using LogMonitor.Processors;

namespace LogMonitor
{
    internal class ChangeManager : IDisposable
    {
        private readonly BlockingCollection<FileChange> inputQueue = new BlockingCollection<FileChange>();

        private readonly BlockingCollection<ResultContainer> outputQueue = new BlockingCollection<ResultContainer>();

        private readonly IProcessor[] processors;

        private readonly CancellationTokenSource cancellation;

        private readonly TaskFactory taskFactory;

        private readonly OutputFilter outputFilter;

        private bool disposed;

        public ChangeManager(IEnumerable<IProcessor> processors, OutputFilter outputFilter)
        {
            if (processors == null)
                throw new ArgumentNullException("processors");

            if (outputFilter == null)
                throw new ArgumentNullException("outputFilter");

            this.processors = processors.ToArray();
            this.outputFilter = outputFilter;

            this.cancellation = new CancellationTokenSource();

            this.taskFactory = new TaskFactory(this.cancellation.Token);
            
            this.taskFactory.StartNew(() => this.ProcessInput(), TaskCreationOptions.LongRunning);
            this.taskFactory.StartNew(() => this.ProcessMetrics(), TaskCreationOptions.LongRunning);
        }

        public void Add(FileChange item)
        {
            this.inputQueue.Add(item);
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
                this.cancellation.Cancel();

                this.disposed = true;
            }
        }

        private void ProcessInput()
        {
            while (!this.cancellation.IsCancellationRequested)
            {
                Debug.WriteLine("Input queue: listening...");

                FileChange item = this.inputQueue.Take(this.cancellation.Token);
                Debug.WriteLine("Input queue: item received...");

                if (this.cancellation.IsCancellationRequested)
                    return;

                IProcessor[] matchingProcessors = this.processors
                    .Where(p => p.IsMatch(item.File))
                    .ToArray();

                Debug.WriteLine("Input queue: {0} matching processors.".FormatWith(matchingProcessors.Length));

                IEnumerable<Metric>[] metrics = new IEnumerable<Metric>[matchingProcessors.Length];

                Sequential.For(
                    0,
                    matchingProcessors.Length,
                    i => metrics[i] = matchingProcessors[i].ParseLine(item));

                this.outputQueue.Add(new ResultContainer
                {
                    Change = item,
                    Metrics = metrics.SelectMany(m => m).ToArray()
                });
                Debug.WriteLine("Input queue: item procesed.");
            }
        }

        private void ProcessMetrics()
        {
            while (!this.cancellation.IsCancellationRequested)
            {
                Debug.WriteLine("Output queue: listening...");

                ResultContainer item = this.outputQueue.Take(this.cancellation.Token);
                Debug.WriteLine("Output queue: item received.");

                if (this.cancellation.IsCancellationRequested)
                    return;

                this.outputFilter.Process(item.Change, item.Metrics);
                Debug.WriteLine("Output queue: item procesed.");
            }
        }
    }
}
