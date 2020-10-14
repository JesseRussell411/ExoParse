using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.elements;

namespace ExoParseV2
{
    public abstract class Operator : ISymbolized
    {
        public abstract string Symbol { get; }
        public virtual bool DontExecute_flag(IElement a, IElement b, Operation parent)
        {
            return false;
        }
        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(this);
        }

        #region pass and calc
        public virtual IElement Pass(IElement a, IElement b, Operation parent)
        {
            if (a == IElement.Void || b == IElement.Void || parent == null) { return IElement.Void; }
            return pass(a, b, parent);
        }
        public virtual IElement Calc(IElement a, IElement b)
        {
            if (a == IElement.Void || b == IElement.Void) { return IElement.Void; }
            return calc(a, b);
        }
        protected virtual IElement pass(IElement a, IElement b, Operation parent) { return parent; }
        protected abstract IElement calc(IElement a, IElement b);
        #endregion

        public override string ToString()
        {
            return $" {Symbol} ";
        }
    }

    public abstract class LeftToRightOperator : Operator
    {
    }

    public abstract class RightToLeftOperator : Operator
    {
    }
}
