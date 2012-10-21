using System.Collections.Generic;

namespace LogMonitor.Processors
{
    /// <summary>
    /// Processor for log file lines.
    /// </summary>
    public interface IProcessor
    {
        bool IsMatch(string fileName);

        /// <summary>
        /// Parses the specified lines and returns metrics.
        /// </summary>
        /// <param name="change">File change.</param>
        /// <returns></returns>
        IEnumerable<Metric> ParseLine(FileChange change);
    }
}
