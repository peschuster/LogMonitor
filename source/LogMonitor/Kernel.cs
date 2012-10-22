using System;
using System.Collections.Generic;
using System.IO;
using LogMonitor.Output;
using LogMonitor.Processors;

namespace LogMonitor
{
    internal class Kernel : IDisposable
    {
        private readonly List<FileNotificationService> watchers = new List<FileNotificationService>();

        private readonly ChangeManager manager;

        private bool disposed;

        public Kernel(IEnumerable<IProcessor> processors, IDictionary<string, IPreProcessor> preProcessors, IDictionary<string, string> filters, OutputFilter outputFilter)
        {
            if (preProcessors == null)
                throw new ArgumentNullException("preProcessors");

            if (processors == null)
                throw new ArgumentNullException("processors");

            this.manager = new ChangeManager(processors, outputFilter);

            foreach (string path in preProcessors.Keys)
            {
                FileNotificationService watcher = this.CreateWatcher(path, filters[path]);

                if (watcher != null)
                {
                    this.watchers.Add(watcher);

                    IPreProcessor preProcessor = preProcessors[path];

                    watcher.ContentAdded += (object o, ContentEventArgs e) => this.manager.Add(preProcessor.Process(e.FileName, e.AddedContent));
                }
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
                if (this.manager != null)
                    this.manager.Dispose();

                foreach (var watcher in this.watchers)
                {
                    watcher.Dispose();
                }

                this.watchers.Clear();

                this.disposed = true;
            }
        }

        private FileNotificationService CreateWatcher(string path, string filter)
        {
            var info = new FileInfo(path);
            FileNotificationService watcher = null;

            if (info.Attributes.HasFlag(FileAttributes.Directory))
            {
                DirectoryInfo directory = new DirectoryInfo(path);

                if (directory.Exists)
                {
                    watcher = new FileNotificationService(directory, true, filter);
                }
            }
            else if (info.Exists)
            {
                watcher = new FileNotificationService(info, true);
            }

            return watcher;
        }
    }
}
