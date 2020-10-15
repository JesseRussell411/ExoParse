using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.elements
{
    public class Messenger : IElement
    {
        public readonly object From;
        public readonly IList<object> Contents;
        public bool DontExecute_flag { get; } = true;
        public IElement Definition { get { return ElementUtils.NullElement; } }

        public virtual double? Execute()
        {
            return null;
        }
        public virtual IElement Pass()
        {
            return this;
        }
        public virtual IElement Calc()
        {
            return this;
        }

        public Messenger(object from, IList<object> contents)
        {
            From = from;
            Contents = contents;
        }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }
    }
    public class TernaryMessenger : Messenger
    {
        public IElement A { get => (IElement)Contents[0]; }
        public IElement B { get => (IElement)Contents[1]; }


        public TernaryMessenger(object from, IList<object> contents)
            : base(from, contents) { }
        public TernaryMessenger(object from, IElement A, IElement B)
            : base(from, new IElement[] { A, B }) { }
    }
}


// They don't ride the short buss for fun, they're special.