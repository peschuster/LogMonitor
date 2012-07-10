using System.Collections.Generic;
using LogMonitor.Configuration.W3CReader;

namespace LogMonitor.Processors
{
    internal class W3CReaderFactory
    {
        private readonly W3CParser parser;

        public W3CReaderFactory()
        {
            this.parser = new W3CParser();
        }

        public IEnumerable<W3CReader> CreateReaders(ParserElementCollection collection)
        {
            if (collection == null)
                yield break;

            foreach (ParserElement element in collection)
            {
                yield return new W3CReader(this.parser)
                {
                    FileMatch = element.FileMatch,
                    Key = element.Key,
                    KeyPattern = element.KeyPattern,
                    KeyPatternTarget = element.KeyPatternTarget,
                    Target = element.Target,
                    Pattern = element.Pattern,
                };
            }
        }
    }
}
