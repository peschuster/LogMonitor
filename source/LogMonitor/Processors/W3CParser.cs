using System;
using System.Collections.Generic;
using System.Linq;

namespace LogMonitor.Processors
{
    public class W3CParser
    {
        private readonly FileHandler io;

        private readonly W3CFieldConfiguration configuration;

        public W3CParser()
        {
            this.io = new FileHandler();
            this.configuration = new W3CFieldConfiguration(this.io);
        }

        public Dictionary<string, string> GetFields(string filename, string line, string[] fieldNames)
        {
            var configuration = this.configuration.Get(filename, new[] { line });

            var result = new Dictionary<string, string>();

            // No configuration? -> nothing to read!
            if (configuration == null)
                return result;

            string[] fieldValues = line.Split(' ');

            foreach (string fieldName in fieldNames)
            {
                if (!configuration.Contains(fieldName))
                    continue;

                int index = Array.IndexOf(configuration, fieldName);

                if (fieldValues.Length < index)
                {
                    result.Add(fieldName, fieldValues[index]);
                }
            }

            return result;
        }
    }
}
