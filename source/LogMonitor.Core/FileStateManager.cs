using System;
using System.Collections.Generic;
using System.IO;

namespace LogMonitor
{
    public class FileStateManager : IDisposable
    {
        private readonly Dictionary<string, long> positions = new Dictionary<string, long>();

        private readonly Dictionary<string, FileInfo> info = new Dictionary<string, FileInfo>();

        private bool disposed;

        public FileStateManager(IEnumerable<FileInfo> files, Predicate<FileInfo> inactive = null)
        {
            if (files != null)
            {
                // Add initial sizes
                foreach (FileInfo file in files)
                {
                    file.Refresh();
                    this.positions.Add(file.FullName, file.Length);

                    if (inactive == null || !inactive(file))
                    {
                        this.info.Add(file.FullName, file);
                    }
                }
            }
        }

        public IList<FileInfo> Files
        {
            get { return new List<FileInfo>(this.info.Values); }
        }

        public long GetPosition(string fullPath)
        {
            if (this.positions.ContainsKey(fullPath))
            {
                return this.positions[fullPath];
            }

            return 0L;
        }

        public bool SizeChanged(string fullPath)
        {
            if (this.positions.ContainsKey(fullPath) && this.info.ContainsKey(fullPath))
            {
                FileInfo file = this.info[fullPath];
                file.Refresh();

                return this.positions[fullPath] != file.Length;
            }

            return true;
        }

        public void UpdatePosition(string fullPath, long position)
        {
            if (this.positions.ContainsKey(fullPath))
            {
                this.positions[fullPath] = position;
            }
            else
            {
                this.positions.Add(fullPath, position);
                this.info.Add(fullPath, new FileInfo(fullPath));
            }

            if (!this.info.ContainsKey(fullPath))
            {
                this.info.Add(fullPath, new FileInfo(fullPath));
            }
        }

        public bool RenameFile(string oldPath, string fullPath)
        {
            return this.MoveEntry(this.positions, oldPath, fullPath)
                || this.MoveEntry(this.info, oldPath, fullPath);
        }

        public bool Remove(string fullPath)
        {
            return this.positions.Remove(fullPath)
                || this.info.Remove(fullPath);
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
                this.positions.Clear();
                this.info.Clear();

                this.disposed = true;
            }
        }

        private bool MoveEntry<TResult>(Dictionary<string, TResult> dict, string key, string newKey)
        {
            if (!dict.ContainsKey(key) || dict.ContainsKey(newKey))
                return false;

            TResult item = dict[key];

            dict.Remove(key);
            dict.Add(newKey, item);

            return true;
        }
    }
}
