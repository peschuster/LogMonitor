using System.Globalization;

namespace LogMonitor.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Applies <code>String.Format()</code> to the string with specified paramters.
        /// The format provider defaults to <c>InvariantCulture</c>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The formated string.</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return format.FormatWith(CultureInfo.InvariantCulture, args);
        }

        /// <summary>
        /// Applies <code>String.Format()</code> to the string with specified paramters.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="culture">The format provider.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The formated string.</returns>
        public static string FormatWith(this string format, CultureInfo culture, params object[] args)
        {
            if (string.IsNullOrEmpty(format))
                return string.Empty;

            return string.Format(culture, format, args);
        }
    }
}
