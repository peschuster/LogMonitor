using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using LogMonitor.Helpers;

namespace LogMonitor.Processors
{
    public class PowershellProcessor : IProcessor, IDisposable
    {
        private readonly string filePatten;

        private Runspace runspace;

        private PowerShell shell;

        private bool disposed;

        public PowershellProcessor(string scriptPath, string filePatten = null)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                throw new ArgumentNullException("script");

            this.filePatten = filePatten;

            this.Initialize(scriptPath);
        }

        public static bool CheckScript(string scriptPath)
        {
            if (string.IsNullOrEmpty(scriptPath)
                || !File.Exists(scriptPath))
                return false;

            string content = File.ReadAllText(scriptPath);

            Collection<PSParseError> errors;

            var tokens = PSParser.Tokenize(content, out errors);

            if (errors.Any())
                return false;

            var sequence = new KeyValuePair<PSTokenType, string>[]
            {
                new KeyValuePair<PSTokenType, string>(PSTokenType.Keyword, "function"),
                new KeyValuePair<PSTokenType, string>(PSTokenType.CommandArgument, "MetricProcessor"),
                new KeyValuePair<PSTokenType, string>(PSTokenType.GroupStart, "("),
                new KeyValuePair<PSTokenType, string>(PSTokenType.Type, "LogMonitor.FileChange"),
                new KeyValuePair<PSTokenType, string>(PSTokenType.Variable, "change"),
                new KeyValuePair<PSTokenType, string>(PSTokenType.GroupEnd, ")"),
            };

            int index = 0;

            foreach (PSToken token in tokens)
            {
                if (token.Type == PSTokenType.NewLine)
                    continue;

                if (token.Type == sequence[index].Key && sequence[index].Value.Equals(token.Content, StringComparison.OrdinalIgnoreCase))
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

            return false;
        }

        public bool IsMatch(string fileName)
        {
            return string.IsNullOrEmpty(this.filePatten) 
                || Regex.IsMatch(fileName, this.filePatten, RegexOptions.IgnoreCase);
        }

        public IEnumerable<Metric> ParseLine(FileChange change)
        {
            this.shell
                .AddCommand("MetricProcessor")
                .AddParameter("change", change);

            Collection<PSObject> results;

            try
            {
                results = this.shell.Invoke();
            }
            catch (RuntimeException exception)
            {
                Trace.TraceError(exception.Format());

                yield break;
            }

            foreach (var obj in results)
            {
                var metric = obj.BaseObject as Metric;

                if (metric != null)
                    yield return metric;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                if (this.shell != null)
                    this.shell.Dispose();

                if (this.runspace != null)
                    this.runspace.Dispose();

                this.disposed = true;
            }
        }

        private void Initialize(string scriptPath)
        {
            string script = File.ReadAllText(scriptPath);

            this.runspace = RunspaceFactory.CreateRunspace();
            this.runspace.Open();

            this.shell = PowerShell.Create();
            this.shell.Runspace = this.runspace;

            this.shell
                .AddCommand("Add-Type")
                .AddParameter("Path", typeof(FileChange).Assembly.Location);

            this.shell.Invoke();

            this.shell.AddScript(script);
            this.shell.Invoke();
        }
    }
}