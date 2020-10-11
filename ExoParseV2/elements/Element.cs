using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.elements
{
    public interface IElement
    {
        //There are three "levels of aggression" for getting an element's value:
        //Execute
        public double? Execute();
        public IElement Calc();
        //Pass
        public IElement Pass();
        public virtual IElement Pass(out bool dontExecute_flag)
        {
            dontExecute_flag = DontExecute_flag;
            return Pass();
        }
        //Definition
        public IElement Definition { get; }


        //Execute: Returns the element's numeric value, if the element involves actions that mutate variables, these actions will take place. Such as a++ for example.
        //Pass: Returns the elements base value, this is usually just the element itself like in the case of variables or literals or constants, but in the case of Operations, Modifiers, or Containers, Something more is given.



        public bool DontExecute_flag { get; }

        public static IElement Null { get { return new Literal(null); } }

        public static IElement Void { get { return null; } }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent);

    }

    public interface IRedefinable
    {
        public IElement Definition { set; }
    }

    public interface ILabeled : IElement
    {
        public string Name { get; }
    }

    public interface IExpressionComponent : IElement
    {
        public int GetPriority(SymbolizedIndex si);
    }
}
