using System;
using System.Threading;

namespace LogMonitor.Helpers
{
    public static class ActionExtensions
    {
        public static TResult Retry<TException, TResult>(this Func<TResult> action, int initialMs, int retries)
            where TException: Exception
        {
            try
            {
                return action();
            }
            catch (TException)
            {
                if (retries <= 0)
                    throw;

                Thread.Sleep(initialMs);

                return action.Retry<TException, TResult>(2 * initialMs, --retries);
            }
        }
    }
}
