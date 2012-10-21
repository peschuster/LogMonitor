using System;
using System.Collections.Generic;
using System.IO;

namespace LogMonitor
{
    public class FileStateManager : IDisposable
    {
        private readonly Dictionary<string, long> positions = new Dictionary<string, long>();

        private bool disposed;

        public FileStateManager(IEnumerable<FileInfo> files)
        {
            if (files != null)
            {
                // Add initial sizes
                foreach (FileInfo file in files)
                {
                    this.positions.Add(file.FullName, file.Length);
                }
            }
        }

        public long GetPosition(string fullPath)
        {
            if (this.positions.ContainsKey(fullPath))
            {
                return this.positions[fullPath];
            }

            return 0L;
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
            }
        }

        public bool RenameFile(string oldPath, string fullPath)
        {
            if (!this.positions.ContainsKey(oldPath))
                return false;

            long position = this.positions[oldPath];

            this.positions.Remove(oldPath);
            this.positions.Add(fullPath, position);

            return true;
        }

        public bool Remove(string fullPath)
        {
            return this.positions.Remove(fullPath);
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

                this.disposed = true;
            }
        }
    }
}
