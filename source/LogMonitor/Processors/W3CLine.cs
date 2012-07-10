using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogMonitor.Processors
{
    public class W3CLine : Dictionary<string, string>
    {
        public W3CLine(IDictionary<string, string> values)
        {
            foreach (var item in values)
            {
                this.Add(item.Key, item.Value);
            }
        }
    }
}
