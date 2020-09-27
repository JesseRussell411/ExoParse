using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public abstract class Modifier : ISymbolized
    {
        public abstract string Symbol { get; }
        public virtual IElement Execute(IElement item)
        {
            if (item == IElement.Void) { return IElement.Void; }
            return calc(item);
        }
        public abstract IElement calc(IElement item);

        public override string ToString()
        {
            return Symbol;
        }

        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(this);
        }
    }

    public abstract class PreModifier : Modifier
    {
    }

    public abstract class PostModifier : Modifier
    {
    }
}
