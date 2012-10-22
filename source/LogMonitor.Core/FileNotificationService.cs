using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace LogMonitor
{
    public class FileNotificationService : IDisposable
    { 
        private readonly FileHandler io;

        private FileStateManager states;

        private FileSystemWatcher watcher;

        private IDisposable subscription;

        private bool disposed;

        private readonly bool fullLines;

        public event EventHandler<ContentEventArgs> ContentAdded;

        public event FileSystemEventHandler Changed
        {
            add { this.watcher.Changed += value; }
            remove { this.watcher.Changed -= value; }
        }

        public FileNotificationService(DirectoryInfo directory, bool fullLines, string filter)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (!directory.Exists)
                throw new ArgumentException("Directory does not exist.");

            this.fullLines = fullLines;

            var watcher = new FileSystemWatcher(directory.FullName, filter);
            IEnumerable<FileInfo> files = directory.EnumerateFiles(filter);
            
            this.io = new FileHandler();

            this.Init(watcher, files);
        }

        public FileNotificationService(FileInfo file, bool fullLines)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (!file.Exists)
                throw new ArgumentException("File does not exist.");

            this.fullLines = fullLines;

            FileSystemWatcher watcher = new FileSystemWatcher(file.DirectoryName, file.Name);

            this.io = new FileHandler();

            this.Init(watcher, new [] { file });
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
                this.states.Dispose();

                this.watcher.Dispose();

                if (this.subscription != null)
                    this.subscription.Dispose();

                this.disposed = true;
            }
        }

        private void Init(FileSystemWatcher watcher, IEnumerable<FileInfo> files)
        {
            this.states = new FileStateManager(files);

            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

            watcher.Created += this.OnObjectChanged;
            watcher.Renamed += this.OnObjectRenamed;
            watcher.Deleted += this.OnObjectChanged;
            
            IObservable<FileSystemEventArgs> listener = Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                handler => (sender, e) => { handler(e); },
                h => watcher.Changed += h,
                h => watcher.Changed -= h);

            this.subscription = listener
                .Buffer(TimeSpan.FromMilliseconds(500))
                .Delay(TimeSpan.FromMilliseconds(10))
                .Subscribe(OnObjectChanged);

            watcher.EnableRaisingEvents = true;

            this.watcher = watcher;
        }

        private void OnObjectRenamed(object sender, RenamedEventArgs e)
        {
            this.states.RenameFile(e.OldFullPath, e.FullPath);
        }

        private void OnObjectChanged(IList<FileSystemEventArgs> eventList)
        {
            foreach (var fileEvent in eventList.GroupBy(l => l.FullPath))
            {
                // in the mean time there might have been a 'deleted' or 'renamed' event
                if (!File.Exists(fileEvent.Key))
                    continue;

                this.OnObjectChanged(this.watcher, fileEvent.Last());
            }
        }

        private void OnObjectChanged(object sender, FileSystemEventArgs e)
        {
            string fileName = e.FullPath;

            if (e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Changed)
            {   
                long position = this.states.GetPosition(fileName);
                long initialPosition = position;

                string content = this.io.Read(fileName, ref position);

                if (!string.IsNullOrEmpty(content) && this.fullLines)
                {
                    int lastLineBreak = Math.Max(content.LastIndexOf('\n'), content.LastIndexOf('\r'));

                    if (position > (initialPosition + lastLineBreak))
                    {
                        position = initialPosition + lastLineBreak;
                        content = content.Substring(0, Math.Min(content.LastIndexOf('\n'), content.LastIndexOf('\r')) + 1);
                    }
                }

                this.states.UpdatePosition(fileName, position);

                this.TriggerContentAdded(fileName, content);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                this.states.Remove(e.FullPath);
            }
        }

        private void TriggerContentAdded(string fileName, string addedContent)
        {
            if (this.ContentAdded != null && !string.IsNullOrEmpty(addedContent))
            {
                this.ContentAdded(this, new ContentEventArgs(fileName, addedContent));
            }
        }
    }
}