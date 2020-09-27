using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExoParseV2
{
    public class SymbolizedIndex
    {
        public ImmutableArray<ImmutableArray<ISymbolized>>                              Items { get; }
        public ImmutableArray<ImmutableDictionary<string, ImmutableArray<ISymbolized>>> ItemsFull { get; }
        public ImmutableDictionary<string, ImmutableArray<ISymbolized>>                 SymbolIndex { get; }
        public ImmutableDictionary<ISymbolized, int>                                    PriorityIndex { get; }

        public IEnumerable<string> AllSymbols
        {
            get
            {
                return SymbolIndex.Keys;
            }
        }
        public IEnumerable<ISymbolized> ItemsFlat
        {
            get
            {
                return PriorityIndex.Keys;
            }
        }
        public ImmutableDictionary<String, ImmutableArray<ISymbolized>> GetItems(int priority)
        {
            return ItemsFull[priority];
        }

        public IEnumerable<string> GetSymbols(int priority)
        {
            if (priority < 0 || priority >= ItemsFull.Length) { return new string[0]; }
            return ItemsFull[priority].Keys;
        }

        public int PriorityCount
        {
            get { return Items.Length; }
        }

        public IEnumerable<int> GetAllPriorities()
        {
            return ItemsFull.Select((item, index) => index);
        }

        public int GetPriority(ISymbolized item)
        {
            return PriorityIndex.TryGetValue(item, out int p) ? p : -1;
        }
        
        public ImmutableArray<ISymbolized> GetItems(string symbol)
        {
            return SymbolIndex.TryGetValue(symbol, out ImmutableArray<ISymbolized> items) ? items : new ImmutableArray<ISymbolized>();
        }

        public bool TryGetItems(String symbol, out ImmutableArray<ISymbolized> result)
        {
            return SymbolIndex.TryGetValue(symbol, out result) ;
        }

        public SymbolizedIndex(IEnumerable<IEnumerable<ISymbolized>> items)
        {
            Items = items.Select(row => row.ToImmutableArray()).ToImmutableArray();
            ItemsFull = Items.Select(row => row.GroupBy(ri => ri.Symbol).Select(g => new KeyValuePair<String, ImmutableArray<ISymbolized>>(g.Key, g.ToImmutableArray())).ToImmutableDictionary()).ToImmutableArray();
            PriorityIndex = Items.Select((row, index) => row.Select(ri => new KeyValuePair<ISymbolized, int>(ri, index))).SelectMany(r => r).ToImmutableDictionary();

            SymbolIndex = ItemsFlat.GroupBy(i => i.Symbol).Select(g => new KeyValuePair<String, ImmutableArray<ISymbolized>>(g.Key, g.ToImmutableArray())).ToImmutableDictionary();
        }
    }
}
