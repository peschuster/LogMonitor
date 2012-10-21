using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LogMonitor.Helpers;
using LogMonitor.Processors;

namespace LogMonitor
{
    public class ProcessorFactory
    {
        public IProcessor Create(string scriptPath, string pattern)
        {
            IEnumerable<string> errors;

            if (!PowershellValidator.CheckScript(scriptPath, out errors))
            {
                throw new ArgumentException("Invalid script ({0}):{1}".FormatWith(scriptPath, Environment.NewLine + string.Join(Environment.NewLine, errors)), "scriptPath");
            }

            ArgumentException regexException;

            if (!string.IsNullOrEmpty(pattern) && !IsRegexPatternValid(pattern, out regexException))
            {
                throw new ArgumentException("Invalid file pattern \"{0}\".".FormatWith(pattern), "pattern", regexException);
            }

            return new PowershellProcessor(scriptPath, pattern);
        }

        private static bool IsRegexPatternValid(string pattern, out ArgumentException exception)
        {
            try
            {
                new Regex(pattern);

                exception = null;
            }
            catch (ArgumentException regexException)
            {
                exception = regexException;

                return false;
            }

            return true;
        }

    }
}
