namespace ExoParseV2
{
    public interface ISymbolized
    {
        string Symbol { get; }
        public int GetPriority(SymbolizedIndex si);
        string Definition { get; }
    }
}
