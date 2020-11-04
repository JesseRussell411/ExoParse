using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.elements;
using MathTypes;

namespace ExoParseV2
{
    public abstract class Modifier : ISymbolized
    {
        public abstract string Symbol { get; }
        public virtual bool DontExecute_flag(IElement item, Modification parent) { return false; }
        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(this);
        }

        #region pass, calc, and execute
        public virtual IElement Pass(IElement item, Modification parent)
        {
            if (item == IElement.Void || parent == null) { return IElement.Void; }
            return pass(item, parent);
        }
        public virtual IElement Calc(IElement item)
        {
            if (item == IElement.Void) { return IElement.Void; }
            return calc(item);
        }
        public virtual IntFloat? Execute(IElement item, Modification parent)
        {
            if (item == IElement.Void || parent == null) { return null; }
            return execute(item, parent);
        }

        protected virtual IElement pass(IElement item, Modification parent) { return parent; }
        protected abstract IElement calc(IElement item);
        protected virtual IntFloat? execute(IElement item, Modification parent) { return calc(item).Execute(); }
        #endregion

        public override string ToString()
        {
            return Symbol;
        }

    }

    public abstract class PreModifier : Modifier
    {
    }

    public abstract class PostModifier : Modifier
    {
    }
}
