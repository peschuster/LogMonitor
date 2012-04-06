using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tail.Processors
{
    public class W3CParser
    {
        private readonly Dictionary<string, Dictionary<string, int>> cache = new Dictionary<string, Dictionary<string, int>>();

        public string[] GetLines(string content)
        {
            return Helper.SplitLines(content);
        }

        public Dictionary<string, List<string>> GetFields(string filename, string content, string[] fieldNames)
        {
            string[] lines = this.GetLines(content);

            var configuration = this.GetFieldConfiguration(filename, lines);

            var result = new Dictionary<string, List<string>>();

            foreach (string fieldName in fieldNames)
            {
                result.Add(fieldName, new List<string>());
            }

            foreach (string line in lines)
            {
                string[] fieldValues = line.Split(' ');

                foreach (string fieldName in fieldNames)
                {
                    if (!configuration.ContainsKey(fieldName))
                        continue;

                    int index = configuration[fieldName];

                    if (fieldValues.Length > index)
                    {
                        result[fieldName].Add(fieldValues[index]);
                    }
                }
            }

            return result;
        }

        public Dictionary<string, int> GetFieldConfiguration(string filename, string[] lines)
        {
            if (!this.cache.ContainsKey(filename))
            {
                var configuration = this.ReadFieldConfiguration(lines);

                if (configuration == null || configuration.Count == 0)
                {
                    string content = Helper.ReadFile(filename);

                    string[] allLines = this.GetLines(content);
                    
                    configuration = this.ReadFieldConfiguration(allLines);
                }

                this.cache.Add(filename, configuration);
            }

            return this.cache[filename];
        }

        private Dictionary<string, int> ReadFieldConfiguration(string[] lines)
        {
            var result = new Dictionary<string, int>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Trim().StartsWith("#Fields:"))
                    continue;

                string cleaned = line
                    .Replace("#", string.Empty)
                    .Replace("Fields:", string.Empty)
                    .Trim();

                string[] parts = cleaned.Split(' ');

                for (int index = 0; index < parts.Length; index++)
                {
                    result.Add(parts[index], index);
                }

                break;
            }

            return result;
        }
    }
}
