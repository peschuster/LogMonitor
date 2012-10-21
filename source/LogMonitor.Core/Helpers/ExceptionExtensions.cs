using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LogMonitor.Helpers
{
    /// <remarks>Credits to GitHub / GitHub.Core</remarks>
    public static class ExceptionExtensions
    {
        private static readonly Lazy<Regex> stackFrameRegex = new Lazy<Regex>(() => new Regex("^\\s+\\S+\\s+\\S+\\(.*?\\)", RegexOptions.Multiline | RegexOptions.Compiled));

        public static string Format(this Exception exception)
        {
            var builder = new StringBuilder();

            exception.WalkReverse(e =>
                {
                    if (builder.Length > 0)
                        builder.AppendLine("---------------");

                    builder.AppendLine(e.DetailedMessage());

                    builder.AppendLine(e.StackTrace);
                });

            return builder.ToString();
        }

        public static string DetailedMessage(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            string input = exception.ToString();

            int length = input.IndexOf(" --->", StringComparison.Ordinal);

            if (length >= 0)
                return input.Substring(0, length);

            Match match = stackFrameRegex.Value.Match(input);

            return match.Success
                ? input.Substring(0, match.Index)
                : input;
        }

        public static void WalkReverse(this Exception exception, Action<Exception> visitor)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            ReflectionTypeLoadException typeLoadException = exception as ReflectionTypeLoadException;

            if (typeLoadException != null)
            {
                typeLoadException.LoaderExceptions.Each<Exception>(e => e.WalkReverse(visitor));
            }

            AggregateException aggregateException = exception as AggregateException;

            if (aggregateException != null)
            {
                aggregateException.InnerExceptions.Each<Exception>(e => e.WalkReverse(visitor));
            }
            else if (exception.InnerException != null)
            {
                exception.InnerException.WalkReverse(visitor);
            }

            visitor(exception);
        }
    }
}