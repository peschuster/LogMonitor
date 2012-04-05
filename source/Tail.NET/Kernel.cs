using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tail
{
    class Kernel : IDisposable
    {
        private readonly List<FileNotificationService> watchers = new List<FileNotificationService>();

        private bool disposed;

        public Kernel(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            foreach (string path in paths)
            {
                var info = new FileInfo(path);
                FileNotificationService watcher = null;

                if (info.Attributes.HasFlag(FileAttributes.Directory))
                {
                    DirectoryInfo directory = new DirectoryInfo(path);

                    if (directory.Exists)
                    {
                        watcher = new FileNotificationService(directory, "*.txt");
                    }
                }
                else if (info.Exists)
                {
                    watcher = new FileNotificationService(info);
                }

                if (watcher != null)
                {
                    watcher.ContentAdded += ConsoleWriter.OnContentAdded;
                    this.watchers.Add(watcher);
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
    }
}
