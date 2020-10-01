using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.the_universe
{
    public class Universe
    {
        public Action<string> PrintFunction { get; set; }
        public Func<string> ReadFunction { get; set; }
        public Environment Environment { get; set; }
        public SymbolizedIndex SymbolizedIndex { get; set; }
    }
}
