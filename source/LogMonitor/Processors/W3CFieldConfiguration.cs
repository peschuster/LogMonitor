using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogMonitor.Processors
{
    internal class W3CFieldConfiguration
    {
        /// <summary>
        /// Cached field configurations (per file)
        /// </summary>
        /// <remarks>
        /// Storage format: file name -> { field1, field 2, ... }
        /// </remarks>
        private readonly IDictionary<string, string[]> cache = new Dictionary<string, string[]>();

        private readonly FileHandler io;

        public W3CFieldConfiguration(FileHandler io)
        {
            if (io == null)
                throw new ArgumentNullException("io");
            
            this.io = io;
        }

        public string[] Get(string filename, string[] lines)
        {
            if (!this.cache.ContainsKey(filename))
            {
                if (!File.Exists(filename))
                    throw new ArgumentException("Specified file does not exist.", "filename");

                // Try to read the configuration from specified files.
                var configuration = this.SeekFile(lines);

                if (configuration == null || configuration.Length == 0)
                {
                    // No configuration is included in specified lines -> read the whole file for a w3c fields line.

                    IEnumerable<string> allLines = this.io.ReadLines(filename);

                    configuration = this.SeekFile(allLines);
                }

                // Still no configuration? -> return null (don't cache!)
                if (configuration == null)
                    return null;

                this.cache.Add(filename, configuration);
            }

            // Is already in cache -> return cached configuration.
            return this.cache[filename];
        }

        private string[] SeekFile(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Trim().StartsWith("#Fields:"))
                    continue;

                return line
                    .Replace("#", string.Empty)
                    .Replace("Fields:", string.Empty)
                    .Trim()
                    .Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .ToArray();
            }

            return null;
        }
    }
}
