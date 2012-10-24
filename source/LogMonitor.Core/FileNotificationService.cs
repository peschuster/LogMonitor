using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using LogMonitor.Helpers;

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

        public FileNotificationService(DirectoryInfo directory, bool fullLines, string filter, int bufferTime)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (!directory.Exists)
                throw new ArgumentException("Directory does not exist.");

            this.fullLines = fullLines;

            var watcher = new FileSystemWatcher(directory.FullName, filter);
            IEnumerable<FileInfo> files = directory.EnumerateFiles(filter);
            
            this.io = new FileHandler();

            this.Init(watcher, files, bufferTime);
        }

        public FileNotificationService(FileInfo file, bool fullLines, int bufferTime)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (!file.Exists)
                throw new ArgumentException("File does not exist.");

            this.fullLines = fullLines;

            FileSystemWatcher watcher = new FileSystemWatcher(file.DirectoryName, file.Name);

            this.io = new FileHandler();

            this.Init(watcher, new[] { file }, bufferTime);
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

        private void Init(FileSystemWatcher watcher, IEnumerable<FileInfo> files, int bufferTime)
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

            var triggerInterval = TimeSpan.FromSeconds(5);

            IObservable<IList<FileSystemEventArgs>> listener2 = Observable.Interval(triggerInterval)
                .Debug(() => "Interval listener triggered {0}.".FormatWith(DateTime.Now.ToLongTimeString()))
                .SelectMany(l => this.states.Files)
                .Where(this.SizeChanged)
                .Select(file => new FileSystemEventArgs(WatcherChangeTypes.Changed, file.DirectoryName, file.Name))
                .Buffer(triggerInterval);

            this.subscription = listener
                .Buffer(TimeSpan.FromMilliseconds(bufferTime))
                .Merge(listener2)
                .Subscribe(OnObjectChanged);

            watcher.EnableRaisingEvents = true;

            this.watcher = watcher;
        }

        private bool SizeChanged(FileInfo file)
        {
            long position = this.states.GetPosition(file.FullName);

            file.Refresh();

            return file.Length != position;
        }

        private void OnObjectRenamed(object sender, RenamedEventArgs e)
        {
            this.states.RenameFile(e.OldFullPath, e.FullPath);
        }

        private void OnObjectChanged(IList<FileSystemEventArgs> eventList)
        {
            if (eventList.Count == 0)
                return;

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

                Func<string> read = () => this.io.Read(fileName, ref position);
                string content = read.Retry<IOException, string>(4, 2);

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