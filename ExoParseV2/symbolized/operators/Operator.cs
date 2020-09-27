using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public abstract class Operator : ISymbolized
    {
        public abstract string Symbol { get; }
        public virtual bool ToStringPadding { get; } = true;
        public virtual IElement Execute(IElement a, IElement b)
        {
            if (a == IElement.Void || b == IElement.Void) { return IElement.Void; }
            return calc(a, b);
        }
        protected abstract IElement calc(IElement a, IElement b);

        public override string ToString()
        {
            if (ToStringPadding)
            {
                return $" {Symbol} ";
            }
            else
            {
                return Symbol;
            }
        }
        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(this);
        }
    }

    public abstract class LeftToRightOperator : Operator
    {
    }

    public abstract class RightToLeftOperator : Operator
    {
    }
}
