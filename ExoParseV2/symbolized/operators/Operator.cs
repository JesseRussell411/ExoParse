﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public abstract class Operator : ISymbolized
    {
        public abstract string Symbol { get; }
        public virtual bool ToStringPadding { get; } = true;
        public virtual IElement Pass(IElement a, IElement b, Operation parent)
        {
            if (a == IElement.Void || b == IElement.Void || parent == null) { return IElement.Void; }
            return pass(a, b, parent);
        }
        public virtual IElement Execute(IElement a, IElement b)
        {
            if (a == IElement.Void || b == IElement.Void) { return IElement.Void; }
            return calc(a, b);
        }
        protected abstract IElement calc(IElement a, IElement b);
        protected virtual IElement pass(IElement a, IElement b, Operation parent)
        {
            return calc(a, b);
        }

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
