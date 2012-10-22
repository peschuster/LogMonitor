using System.Text.RegularExpressions;

namespace LogMonitor.Processors
{
    internal static class Helper
    {
        private static readonly Regex lineSplit = new Regex(@"\r\n|\r(?!\n)|(?<!\r)\n", RegexOptions.Compiled);

        /// <summary>
        /// Splits content into lines (regardless of unix or windows line endings).
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] SplitLines(string content)
        {
            if (string.IsNullOrEmpty(content))
                return new string[0];

            return lineSplit.Split(content);
        }
    }
}
