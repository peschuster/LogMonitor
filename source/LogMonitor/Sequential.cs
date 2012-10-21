using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LogMonitor
{
    internal static class Sequential
    {
        [DebuggerStepThrough]
        public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int> body)
        {
            for (int i = fromInclusive; i < toExclusive; i++)
            {
                body.Invoke(i);
            }

            return new ParallelLoopResult();
        }
    }
}
