using System;
using System.Collections.Generic;
using System.Linq;

namespace Tail.Processors
{
    public class W3CParser
    {
        private readonly FileHandler io;

        private readonly W3CFieldConfiguration configuration;

        public W3CParser ()
        {
            this.io = new FileHandler();
            this.configuration = new W3CFieldConfiguration(this.io);
        }

        public Dictionary<string, List<string>> GetFields(string filename, string content, string[] fieldNames)
        {
            string[] lines = Helper.SplitLines(content);

            var configuration = this.configuration.Get(filename, lines);

            var result = new Dictionary<string, List<string>>();

            // No configuration? -> nothing to read!
            if (configuration == null)
                return result;

            // Initialize result data.
            foreach (string fieldName in fieldNames)
            {
                result.Add(fieldName, new List<string>());
            }

            foreach (string line in lines)
            {
                string[] fieldValues = line.Split(' ');

                foreach (string fieldName in fieldNames)
                {
                    if (!configuration.Contains(fieldName))
                        continue;

                    int index = Array.IndexOf(configuration, fieldName);

                    if (fieldValues.Length < index)
                    {
                        result[fieldName].Add(fieldValues[index]);
                    }
                }
            }

            return result;
        }
    }
}
