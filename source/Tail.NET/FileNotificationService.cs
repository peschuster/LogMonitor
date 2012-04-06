using System;
using System.Collections.Generic;
using System.IO;

namespace Tail
{
    internal class FileNotificationService : IDisposable
    {
        private readonly Dictionary<string, long> positions = new Dictionary<string, long>();

        private FileSystemWatcher watcher;

        private bool disposed;

        public event EventHandler<ContentEventArgs> ContentAdded;

        public FileNotificationService(DirectoryInfo directory, string filter = "*.*")
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (!directory.Exists)
                throw new ArgumentException("Directory does not exist.");

            FileSystemWatcher watcher = new FileSystemWatcher(directory.FullName, filter);
            IEnumerable<FileInfo> files = directory.EnumerateFiles(filter);

            this.Init(watcher, files);
        }

        public FileNotificationService(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            if (!file.Exists)
                throw new ArgumentException("File does not exist.");

            FileSystemWatcher watcher = new FileSystemWatcher(file.DirectoryName, file.Name);

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
                this.watcher.Dispose();
                
                this.positions.Clear();

                this.disposed = true;
            }
        }

        private void Init(FileSystemWatcher watcher, IEnumerable<FileInfo> files)
        {
            // Add initial sizes
            foreach (FileInfo file in files)
            {
                this.positions.Add(file.FullName, file.Length);
            }

            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

            watcher.Created += this.OnObjectChanged;
            watcher.Changed += this.OnObjectChanged;

            watcher.Renamed += this.OnObjectRenamed;
            watcher.Deleted += this.OnObjectChanged;

            watcher.EnableRaisingEvents = true;

            this.watcher = watcher;
        }

        private void TriggerContentAdded(string fileName, string addedContent)
        {
            if (this.ContentAdded != null && !string.IsNullOrEmpty(addedContent))
            {
                this.ContentAdded(this, new ContentEventArgs(fileName, addedContent));
            }
        }

        private long GetPosition(string fullPath)
        {
            if (this.positions.ContainsKey(fullPath))
            {
                return this.positions[fullPath];
            }

            return 0L;
        }

        private void UpdatePosition(string fullPath, long position)
        {
            if (this.positions.ContainsKey(fullPath))
            {
                this.positions[fullPath] = position;
            }
            else
            {
                this.positions.Add(fullPath, position);
            }
        }

        private void OnObjectRenamed(object sender, RenamedEventArgs e)
        {
            if (this.positions.ContainsKey(e.OldFullPath))
            {
                long position = this.positions[e.OldFullPath];

                this.positions.Remove(e.OldFullPath);
                this.positions.Add(e.FullPath, position);
            }
        }

        private void OnObjectChanged(object sender, FileSystemEventArgs e)
        {
            string fileName = e.FullPath;

            if (e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Changed)
            {   
                long position = this.GetPosition(fileName);

                string content = this.ReadFileContent(fileName, ref position);

                this.UpdatePosition(fileName, position);

                this.TriggerContentAdded(fileName, content);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                this.positions.Remove(e.FullPath);
            }
        }

        private string ReadFileContent(string fileName, ref long position)
        {
            string content;
            Stream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

                if (position >= stream.Length)
                {
                    position = stream.Length;

                    return null;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;

                    reader.BaseStream.Seek(position, SeekOrigin.Begin);

                    content = reader.ReadToEnd();

                    position = reader.BaseStream.Position;
                }

                return content;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }
    }
}
