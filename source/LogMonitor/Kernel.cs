using System;
using System.Collections.Generic;
using System.IO;
using LogMonitor.Configuration;
using LogMonitor.Output;
using LogMonitor.Processors;

namespace LogMonitor
{
    internal class Kernel : IDisposable
    {
        private readonly List<FileNotificationService> watchers = new List<FileNotificationService>();

        private readonly ChangeManager manager;

        private bool disposed;

        public Kernel(IEnumerable<IProcessor> processors, IEnumerable<IWatchConfiguration> watchList, OutputFilter outputFilter)
        {
            if (watchList == null)
                throw new ArgumentNullException("watchList");

            if (processors == null)
                throw new ArgumentNullException("processors");

            this.manager = new ChangeManager(processors, outputFilter);

            Lazy<W3CProcessor> w3CProcessor = new Lazy<W3CProcessor>(() => new W3CProcessor());
            DefaultPreProcessor preProcessor = new DefaultPreProcessor();

            foreach (var item in watchList)
            {
                FileNotificationService watcher = this.CreateWatcher(item);

                if (watcher != null)
                {
                    this.watchers.Add(watcher);

                    IPreProcessor selectedPreProcessor;

                    if (!string.IsNullOrEmpty(item.Type) && "w3c".Equals(item.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedPreProcessor = w3CProcessor.Value;
                    }
                    else
                    {
                        selectedPreProcessor = preProcessor;
                    }

                    watcher.ContentAdded += (object o, ContentEventArgs e) => this.manager.Add(selectedPreProcessor.Process(e.FileName, e.AddedContent));
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

        private FileNotificationService CreateWatcher(IWatchConfiguration configuration)
        {
            var info = new FileInfo(configuration.Path);
            FileNotificationService watcher = null;

            if (info.Attributes.HasFlag(FileAttributes.Directory))
            {
                DirectoryInfo directory = new DirectoryInfo(configuration.Path);

                if (directory.Exists)
                {
                    watcher = new FileNotificationService(directory, true, configuration.Filter ?? "*", configuration.BufferTime);
                }
            }
            else if (info.Exists)
            {
                watcher = new FileNotificationService(info, true, configuration.BufferTime);
            }

            return watcher;
        }
    }
}
