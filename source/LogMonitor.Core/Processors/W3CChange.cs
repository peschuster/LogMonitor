using System.Collections.Generic;

namespace LogMonitor.Processors
{
    public class W3CChange : FileChange
    {
        public string[] FieldNames { get; set; }

        public IEnumerable<string[]> Values { get; set; }
    }
}
