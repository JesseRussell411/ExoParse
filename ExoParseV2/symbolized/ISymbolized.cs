using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public interface ISymbolized
    {
        string Symbol { get; }
        public int GetPriority(SymbolizedIndex si);
    }
}
