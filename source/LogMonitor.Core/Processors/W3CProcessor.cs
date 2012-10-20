using System;
using System.Collections.Generic;

namespace LogMonitor.Processors
{
    public class W3CProcessor : IPreProcessor
    {
        private readonly W3CFieldCache cache;

        public W3CProcessor()
        {
            this.cache = new W3CFieldCache(new FileHandler());
        }

        public W3CChange Process(FileChange change)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            try
            {
                return this.Process(change.File, change.Content);
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(exception.Message, "change", exception);
            }
        }

        public W3CChange Process(string fileName, string content)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException("content");

            string[] lines = Helper.SplitLines(content);

            string[] fieldNames = this.cache.Get(fileName, lines);

            var values = new List<string[]>(lines.Length);

            foreach (string line in lines)
            {
                if (line.StartsWith("#"))
                    continue;

                values.Add(line.Split(' '));
            }

            return new W3CChange
            {
                File = fileName,
                Content = content,
                FieldNames = fieldNames,
                Values = values,
            };
        }

        FileChange IPreProcessor.Process(string fileName, string content)
        {
            return this.Process(fileName, content);
        }
    }
}
