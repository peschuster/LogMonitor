using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;

namespace LogMonitor.Processors
{
    public class PowerShellProcessor : IProcessor, IDisposable
    {
        private readonly string filePatten;

        private Runspace runspace;

        private PowerShell ps;

        private bool disposed;

        public PowerShellProcessor(string scriptPath, string filePatten = null)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                throw new ArgumentNullException("script");

            this.filePatten = filePatten;

            this.Initialize(scriptPath);
        }

        public bool IsMatch(string fileName)
        {
            return string.IsNullOrEmpty(filePatten)
                || Regex.IsMatch(fileName, filePatten, RegexOptions.IgnoreCase);
        }

        private void Initialize(string scriptPath)
        {
            string script = File.ReadAllText(scriptPath);

            this.runspace = RunspaceFactory.CreateRunspace();
            this.runspace.Open();

            this.ps = PowerShell.Create();
            ps.Runspace = runspace;

            ps.AddCommand("Add-Type").AddParameter("Path", typeof(FileChange).Assembly.Location);
            ps.Invoke();

            ps.AddScript(script);
            ps.Invoke();
        }

        public IEnumerable<Metric> ParseLine(FileChange change)
        {
            ps.AddCommand("MetricProcessor").AddParameter("change", change);

            // Execute...
            Collection<PSObject> results;

            try
            {
                results = ps.Invoke();
            }
            catch (RuntimeException exception)
            {
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
                if (this.runspace != null)
                    this.runspace.Dispose();

                this.disposed = true;
            }
        }
    }
}
