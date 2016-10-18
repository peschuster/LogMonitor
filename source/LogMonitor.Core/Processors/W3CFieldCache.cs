using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogMonitor.Processors
{
    internal class W3CFieldCache
    {
        private readonly object lockObject = new object();

        /// <summary>
        /// Cached field configurations (per file)
        /// </summary>
        /// <remarks>
        /// Storage format: file name -> { field1, field 2, ... }
        /// </remarks>
        private readonly IDictionary<string, string[]> cache = new Dictionary<string, string[]>();

        private readonly FileHandler ioHandler;

        public W3CFieldCache(FileHandler ioHandler)
        {
            if (ioHandler == null)
                throw new ArgumentNullException("ioHandler");

            this.ioHandler = ioHandler;
        }

        public string[] Get(string filename, string[] lines)
        {
            lock (this.lockObject)
            {
                if (!this.cache.ContainsKey(filename)
                    || this.ContainsConfiguration(lines))
                {
                    // Try to read the configuration from specified files.
                    string[] configuration = this.ParseConfiguration(lines);

                    if (configuration == null || configuration.Length == 0)
                    {
                        if (!File.Exists(filename))
                            throw new ArgumentException("Specified file does not exist.", "filename");

                        // No configuration is included in specified lines -> read the whole file for a w3c fields line.
                        IEnumerable<string> linesOfFile = this.ioHandler.ReadLines(filename);

                        configuration = this.ParseConfiguration(linesOfFile);
                    }

                    // Still no configuration? -> return null (don't cache!)
                    if (configuration == null)
                        return null;

                    if (this.cache.ContainsKey(filename))
                    {
                        this.cache[filename] = configuration;
                    }
                    else
                    {
                        this.cache.Add(filename, configuration);
                    }
                }

                // Is already in cache -> return cached configuration.
                return this.cache[filename];
            }
        }

        private bool ContainsConfiguration(IEnumerable<string> lines)
        {
            return lines != null
                && lines.Any(l => l.Contains("#Fields:"));
        }

        private string[] ParseConfiguration(IEnumerable<string> lines)
        {
            if (lines == null)
                return null;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)
                    || !line.Trim().StartsWith("#Fields:"))
                    continue;

                return line
                    .Replace("#Fields:", string.Empty)
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