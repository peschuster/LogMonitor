using System;
using System.Collections.Generic;
using System.IO;
using LogMonitor.Processors;

namespace LogMonitor
{
    internal class Kernel : IDisposable
    {
        private readonly List<FileNotificationService> watchers = new List<FileNotificationService>();

        private readonly ChangeManager manager;

        private bool disposed;

        public Kernel(IProcessor[] processors, string[] paths, IDictionary<string, IPreProcessor> preProcessors = null, string filter = "*.*")
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            if (processors == null)
                throw new ArgumentNullException("processors");
            
            this.manager = new ChangeManager(processors);

            foreach (string path in paths)
            {
                FileNotificationService watcher = this.CreateWatcher(path, filter);

                if (watcher != null)
                {
                    this.watchers.Add(watcher);

                    IPreProcessor preProcessor = preProcessors != null && preProcessors.ContainsKey(path)
                        ? preProcessors[path]
                        : new DefaultPreProcessor();

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
                foreach (var watcher in this.watchers)
                {
                    watcher.Dispose();
                }

                this.watchers.Clear();

                this.disposed = true;
            }
        }

        private FileNotificationService CreateWatcher(string path, string filter = "*.*")
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
