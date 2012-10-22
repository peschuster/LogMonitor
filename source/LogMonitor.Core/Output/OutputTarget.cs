using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogMonitor.Output
{
    public class OutputTarget
    {
        private readonly Regex fileCheck;

        private readonly Regex typeCheck;

        private readonly IOutputBackend backend;

        private readonly string metricsPrefix;

        public OutputTarget(string pathPattern, string types, string metricsPrefix, IOutputBackend backend)
        {
            if (backend == null)
                throw new ArgumentNullException("backend");

            this.backend = backend;
            
            this.fileCheck = string.IsNullOrEmpty(pathPattern)
                ? null
                : new Regex(pathPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            this.typeCheck = new Regex(types, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            this.metricsPrefix = metricsPrefix;
        }

        public bool IsFileMatch(string filePath)
        {
            return this.fileCheck == null || this.fileCheck.IsMatch(filePath);
        }

        public bool IsMetricMatch(Metric metric)
        {
            if (metric == null)
                return false;

            return this.typeCheck.IsMatch(metric.Type) 
                && this.backend.SupportedTypes.Contains(metric.Type, StringComparer.OrdinalIgnoreCase);
        }

        public void Process(Metric metric)
        {
            this.backend.Submit(metric, this.metricsPrefix);
        }
    }
}
