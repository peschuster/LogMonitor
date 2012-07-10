using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogMonitor.Processors
{
    public class W3CReader
    {
        private readonly W3CParser parser;

        public W3CReader(W3CParser parser)
        {
            this.parser = parser;
        }

        public string FileMatch { get; set; }

        public string Key { get; set; }

        public string Target { get; set; }

        public string Pattern { get; set; }

        public string KeyPattern { get; set; }

        public string KeyPatternTarget { get; set; }

        public bool IsMatch(string fileName, string line)
        {
            if (!this.IsFileMatch(fileName))
                return false;

            var fields = this.parser.GetFields(fileName, line, this.ComposeFieldNames());


        }

        public string CreateKey(string fileName, string line)
        {

        }

        public int ReadValue(string fileName, string line)
        {
            if (string.IsNullOrEmpty(this.Target))
                return 1;

            int result;
            
            var fields = this.parser.GetFields(fileName, line, this.ComposeFieldNames());

            if (string.IsNullOrEmpty(this.Pattern))
            {
                int.TryParse(fields[this.Target], out result);
            }
            else
            {
                int.TryParse(
                    Regex.Match(fields[this.Target], this.Pattern).Value,
                    out result);
            }

            return result;
        }

        private bool IsFileMatch(string fileName)
        {
            return string.IsNullOrEmpty(this.FileMatch)
                || Regex.IsMatch(fileName, this.FileMatch);
        }

        private string[] ComposeFieldNames()
        {
            if (string.IsNullOrEmpty(this.Target) && string.IsNullOrEmpty(this.KeyPatternTarget))
            {
                return new string[0];
            }
            else if (string.IsNullOrEmpty(this.Target))
            {
                return new[] { this.KeyPatternTarget };
            }
            else if (string.IsNullOrEmpty(this.KeyPatternTarget))
            {
                return new[] { this.Target };
            }
            else
            {
                return new[] { this.Target, this.KeyPatternTarget };
            }
        }
    }
}
