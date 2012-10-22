using System;
using System.Linq;
using LogMonitor.Helpers;

namespace LogMonitor.Output
{
    internal static class Helper
    {
        public static string BuildKey(string key, string keyPrefix)
        {
            return string.IsNullOrEmpty(keyPrefix)
                ? key
                : keyPrefix.EnsureLast('.') + key;
        }
    }
}
