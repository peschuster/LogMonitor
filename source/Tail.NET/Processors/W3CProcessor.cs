using System;
using System.Collections.Generic;
using System.Linq;

namespace Tail.Processors
{
    class W3CProcessor : IProcessor
    {
        private readonly W3CParser parser;

        private readonly Dictionary<string, Func<IEnumerable<string>, Dictionary<string, int>>> fields;

        public event EventHandler<MetricsEventArgs> PopulateMetrics;

        public W3CProcessor()
        {
            this.parser = new W3CParser();

            this.fields = new Dictionary<string, Func<IEnumerable<string>, Dictionary<string, int>>>
            {
                { "cs-method", this.CountDictinct },
                { "sc-status", this.ExtractStatusCodes },
                { "time-taken", this.CountDictinct }
            };
        }

        public void OnContentAdded(object sender, ContentEventArgs e)
        {
            if (this.PopulateMetrics == null)
                return;

            var values = this.parser.GetFields(e.FullName, e.AddedContent, this.fields.Keys.ToArray());

            foreach (string field in this.fields.Keys)
            {
                Dictionary<string, int> metrics = this.fields[field](values[field])
                    .ToDictionary(x => field + "." + x.Key, x => x.Value);

                this.PopulateMetrics(
                    this, 
                    new MetricsEventArgs(e.AddedContent, metrics));
            }
        }

        private Dictionary<string, int> ExtractStatusCodes(IEnumerable<string> values)
        {
            return new Dictionary<string, int>
            {
                { "1xx", values.Count(s => s.StartsWith("1")) },
                { "2xx", values.Count(s => s.StartsWith("2")) },
                { "3xx", values.Count(s => s.StartsWith("3")) },
                { "4xx", values.Count(s => s.StartsWith("4")) },
                { "5xx", values.Count(s => s.StartsWith("5")) },
            };
        }

        private Dictionary<string, int> CountDictinct(IEnumerable<string> values)
        {
            return values
                .GroupBy(s => s)
                .ToDictionary(s => s.Key, s => s.Count());
        }
    }
}
