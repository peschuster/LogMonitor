using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace LogMonitor.Processors
{
    public static class PowershellValidator
    {
        private static readonly Predicate<PSToken>[] sequence = new Predicate<PSToken>[]
        {
            new Predicate<PSToken>(t => t.TestToken(PSTokenType.Keyword, "function")),
            new Predicate<PSToken>(t => t.TestToken(PSTokenType.CommandArgument, "MetricProcessor")),
            new Predicate<PSToken>(t => t.TestToken(PSTokenType.GroupStart, "(")),
            new Predicate<PSToken>(t => t.TestToken(PSTokenType.Type, "LogMonitor.FileChange", "[LogMonitor.FileChange]")),
            new Predicate<PSToken>(t => t.TestToken(PSTokenType.Variable, "change")),
            new Predicate<PSToken>(t => t.TestToken(PSTokenType.GroupEnd, ")")),
        };

        public static bool CheckScript(string scriptPath, out IEnumerable<string> errors)
        {
            if (string.IsNullOrEmpty(scriptPath)
                || !File.Exists(scriptPath))
            {
                errors = new [] { "The specified file does not exist." };

                return false;
            }

            string content = File.ReadAllText(scriptPath);

            Collection<PSParseError> parserErrors;

            var tokens = PSParser.Tokenize(content, out parserErrors);

            errors = new List<string>(parserErrors.Select(e => e.Message));

            if (parserErrors.Any())
                return false;

            int index = 0;

            foreach (PSToken token in tokens)
            {
                if (token.Type == PSTokenType.NewLine)
                    continue;

                if (sequence[index].Invoke(token))
                {
                    index++;
                }
                else
                {
                    index = 0;
                }

                if (index == sequence.Length)
                    return true;
            }

            ((List<string>)errors).Add("Function \"MetricProcessor ([LogMonitor.FileChange] change)\" not found in script."); 

            return false;
        }

        internal static bool TestToken(this PSToken token, PSTokenType type, params string[] content)
        {
            return type == token.Type
                && content.Any(c => c.Equals(token.Content, StringComparison.OrdinalIgnoreCase));
        }
    }
}
