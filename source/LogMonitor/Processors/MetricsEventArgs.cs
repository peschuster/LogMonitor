using System;
using System.Collections.Generic;

namespace LogMonitor.Processors
{
    class MetricsEventArgs : EventArgs
    {
        public MetricsEventArgs(string raw, Dictionary<string, int> data)
        {
            this.Raw = raw;
            this.Data = data;
        }

        public string Raw { get; private set; }

        public Dictionary<string, int> Data { get; private set; }
    }
}
