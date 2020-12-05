using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JesseRussell.Numerics;

namespace ExoParseV2.elements
{
    public interface IElement
    {
        /// <summary>
        /// Called when received by an Operation or Modification.
        /// </summary>
        /// <returns></returns>
        public virtual IElement Pass() { return this; }

        public IElement Calc();
        public IntFloatFrac? Execute();
        public virtual IElement Pass(out bool dontExecute_flag)
        {
            dontExecute_flag = DontExecute_flag;
            return Pass();
        }
        public virtual bool DontExecute_flag { get { return false; } }
        //Definition
        public virtual IElement Definition { get { return this; } }


        // *The REAL version of this is found in utilities.ElementUtils.
        //  It's an extension method so it can take null pointers into account and return "void" instead of crashing.
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent);

        #region static
        public static IElement Null { get { return new Literal(null); } }
        public static IElement Void { get { return null; } }
        #endregion

    }

    public interface IRedefinable
    {
        public IElement Definition { set; }
    }

    public interface IReference : IElement
    {
        public string Name { get; }
    }

    public interface IExpressionComponent : IElement
    {
        public int GetPriority(SymbolizedIndex si);
    }
}
