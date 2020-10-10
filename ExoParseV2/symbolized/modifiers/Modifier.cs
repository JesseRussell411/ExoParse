using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.elements;

namespace ExoParseV2
{
    public abstract class Modifier : ISymbolized
    {
        public abstract string Symbol { get; }
        public virtual IElement Calc(IElement item)
        {
            if (item == IElement.Void) { return IElement.Void; }
            return calc(item);
        }
        public virtual IElement Pass(IElement item, Modification parent)
        {
            if (item == IElement.Void || parent == null) { return IElement.Void; }
            return pass(item, parent);
        }
        protected abstract IElement calc(IElement item);
        protected virtual IElement pass(IElement item, Modification parent)
        {
            return calc(item);
        }

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
