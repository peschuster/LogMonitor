using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogMonitor
{
    internal class FileHandler
    {
        /// <summary>
        /// Reads all file content, beginning at specified <paramref name="offset" /> position.
        /// If the offset position is larger than the size of the file, null is returned and <paramref name="offset"/> set to the file length.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="offset">The offset position. After reading file content offset is set to the file length.</param>
        /// <returns></returns>
        public string Read(string fileName, ref long offset)
        {
            string content;
            Stream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

                if (offset >= stream.Length)
                {
                    offset = stream.Length;

                    return null;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;

                    reader.BaseStream.Seek(offset, SeekOrigin.Begin);

                    content = reader.ReadToEnd();

                    offset = reader.BaseStream.Position;
                }

                return content;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        public IEnumerable<string> ReadLines(string filename)
        {
            Stream stream = null;

            try
            {
                stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                
                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;

                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }
    }
}
