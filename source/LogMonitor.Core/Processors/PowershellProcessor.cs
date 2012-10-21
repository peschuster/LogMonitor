using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using LogMonitor.Helpers;

namespace LogMonitor.Processors
{
    public class PowershellProcessor : IProcessor, IDisposable
    {
        private readonly string filePattern;

        private Runspace runspace;

        private PowerShell shell;

        private bool disposed;

        public PowershellProcessor(string scriptPath, string filePattern = null)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                throw new ArgumentNullException("scriptPath");

            this.filePattern = filePattern;

            this.Initialize(scriptPath);
        }

        public bool IsMatch(string fileName)
        {
            return string.IsNullOrEmpty(this.filePattern) 
                || Regex.IsMatch(fileName, this.filePattern, RegexOptions.IgnoreCase);
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