using ExoParseV2.utilities;
using ParsingTools;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Security;
using System.Text;
using MathTypes;
using System.Numerics;

namespace ExoParseV2.elements
{
    /// <summary>
    /// The simplest element. Contains a read-only property for a IntFloat? value and nothing else.
    /// </summary>
    public struct Literal : IElement
    {
        public Literal(IntFloat? value)
        {
            Value = value;
        }
        public readonly IntFloat? Value;

        public IElement Pass() { return this; }
        public IElement Calc() { return this; }
        public IntFloat? Execute() { return Value; }

        public bool DontExecute_flag { get { return false; } }
        public override string ToString()
        {
            if (Value == null) { return StringProps.NullLabel; }

            string s = Value.ToString();

            return s.Contains('E') ? $" {s} " : s; // *add extra padding around scientific notation to make sure the output can be parsed.
        }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }

        #region static
        public static implicit operator Literal(IntFloat? ift) => new Literal(ift);
        public static implicit operator Literal(IntFloat ift) => new Literal(ift);
        public static implicit operator IntFloat?(Literal lit) => lit.Value;
        public static Literal Default { get { return new Literal(null); } }

        public static Literal Parse(String s, bool trapNegPos = false)
        {
            if (TryParse(s, out Literal result, trapNegPos))
            {
                return result;
            }
            else
            {
                throw new FormatException($"\"{s}\" is not a valid constant.");
            }

        }
        public static bool TryParse(String s, out Literal result, bool trapNegPos = false)
        {
            if (s.Length == 0) { result = Literal.Default; return false; }//--(FAIL)--

            if (!trapNegPos && !s.Select(c => char.ToLower(c)).Contains('e') && (s[0] == '-' || s[0] == '+'))
            { result = Literal.Default; return false; }//--(FAIL)--

            if (s == StringProps.NullLabel) { result = Literal.Default; return true; }//--(PASS)--

            if (IntFloat.TryParse(s, out IntFloat d))
            {
                result = new Literal(d);
                return true;//--(PASS)--
            }
            else
            {
                result = Literal.Default;
                return false; //--(FAIL)--
            }
        }
        #endregion
    }

    public class Constant : IReference
    {
        public Constant(String name, IElement definition)
        {
            Definition = definition;
            Name = name;
        }
        public Constant(string name, IntFloat? value)
        {
            Definition = value.ToElement();
            Name = name;
        }
        public string Name { get; }
        public IElement Definition { get; }


        public IElement Pass() { return this; }
        public IElement Calc() { return Definition; }
        public IntFloat? Execute() { return Definition?.Execute(); }

        public override string ToString()
        {
            return Name;
        }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }
    }
    public class BuiltInConstant : Constant
    {
        public BuiltInConstant(string name, IElement definition)
            : base(name, definition) { }
        public BuiltInConstant(string name, IntFloat? value)
            : base(name, value) { }
    }

    public class Variable : IReference, IRedefinable
    {
        public Variable(String name)
        {
            Name = name;
        }
        public string Name { get; }
        public IElement Definition { get; set; }

        public IElement Pass() { return this; }
        public IElement Calc() { return this; }
        public IntFloat? Execute() { return Definition?.Execute(); }

        public bool DontExecute_flag { get; } = false;
        public override string ToString()
        {
            return Name;
        }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }
    }
    public class Operation : IExpressionComponent
    {
        #region constructors
        public Operation(Operator op, IElement a, IElement b)
        {
            Operator = op;
            A = a;
            B = b;
        }
        public Operation(Operator op, IntFloat? a, IntFloat? b)
            : this(op, a.ToElement(), b.ToElement()) { }

        public Operation(Operator op, long? a, long? b)
            : this(op, a.ToElement(), b.ToElement()) { }
        #endregion

        #region Properties
        public IElement A { get; set; }
        public IElement B { get; set; }
        public Operator Operator { get; set; }
        public IElement Definition { get { return Pass(); } }
        public bool DontExecute_flag
        { get { return Operator?.DontExecute_flag(A, B, this) ?? false; } }
        #endregion


        #region methods
        public IElement Pass()
        {
            return Operator?.Pass(A?.Pass(), B?.Pass(), this);
        }
        public IElement Calc()
        {
            return Operator?.Calc(A?.Pass(), B?.Pass());
        }
        public IntFloat? Execute()
        {
            return Operator?.Execute(A?.Pass(), B?.Pass(), this);
            //return Pass()?.Execute();
            //return Operator?.Calc(A?.Pass(), B?.Pass())?.Execute();
        }

        public override string ToString()
        {
            string A_string = A is IExpressionComponent ? A?.ToString()?.Wrap(StringProps.OpenBrackets[0], StringProps.CloseBrackets[0]) ?? StringProps.VoidLabel : A?.ToString() ?? StringProps.VoidLabel;
            string B_string = B is IExpressionComponent ? B?.ToString()?.Wrap(StringProps.OpenBrackets[0], StringProps.CloseBrackets[0]) ?? StringProps.VoidLabel : B?.ToString() ?? StringProps.VoidLabel;
            return $"{A_string}{Operator}{B_string}";
        }

        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(Operator);
        }

        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            bool toWrap = parent != null && parent.GetPriority(si) > ((IExpressionComponent)this).GetPriority(si);

            string returnString, A_string, B_string;

            A_string = A?.ToString(si, this) ?? StringProps.VoidLabel;
            B_string = B?.ToString(si, this) ?? StringProps.VoidLabel;

            returnString = $"{A_string}{Operator}{B_string}";

            if (toWrap)
            {
                return returnString.Wrap(StringProps.OpenBrackets[0], StringProps.CloseBrackets[0]);
            }
            else
            {
                return returnString;
            }
        }
        #endregion
    }

    public class Modification : IExpressionComponent
    {
        public Modification(Modifier mod, IElement item)
        {
            Modifier = mod;
            Item = item;
        }
        public IElement Item { get; set; }
        public Modifier Modifier { get; set; }

        public IElement Definition { get { return Pass(); } }
        public bool DontExecute_flag { get { return Modifier?.DontExecute_flag(Item, this) ?? false; }
        }

        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(Modifier);
        }

        public IElement Pass()
        {
            return Modifier?.Pass(Item?.Pass(), this);
        }
        public IElement Calc()
        {
            return Modifier?.Calc(Item?.Pass());
        }
        public IntFloat? Execute()
        {
            return Modifier?.Execute(Item?.Pass(), this);
            //return Modifier?.Calc(Item?.Pass())?.Execute();
        }

        public override string ToString()
        {
            String itemString;
            itemString = (Item is IExpressionComponent) ? Item.ToString().Wrap(StringProps.OpenBrackets[0], StringProps.CloseBrackets[0]) : Item.ToString();


            if (Modifier is PreModifier)
            {
                return $"{Modifier}{itemString}";
            }
            else
            {
                return $"{itemString}{Modifier}";
            }
        }

        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {

            bool toWrap = parent is Modification;
            //toWrap || !toWarp, that is the question!

            string Item_str = Item.ToString(si, this);
            string modifier_str = Modifier is PreModifier ? $"{Modifier}{Item_str}" : $"{Item_str}{Modifier}";
            return toWrap ? modifier_str.Wrap(StringProps.OpenBrackets[0], StringProps.CloseBrackets[0]) : modifier_str;
        }
    }

    public class Execution : IElement
    {
        #region Constructors
        public Execution(Function function, params IElement[] arguments)
            : this(function, arguments, null, null) { }
        public Execution(Function function, IElement[] arguments, string openingBracket = null, string closingBracket = null, string delim = null)
        {
            if (openingBracket == null) { openingBracket = StringProps.OpenBrackets[0]; }
            if (closingBracket == null) { closingBracket = StringProps.CloseBrackets[0]; }
            if (delim == null) { delim = StringProps.Delims[0]; }
            OpeningBracket = openingBracket;
            ClosingBracket = closingBracket;
            Delim = delim;
            Function = function;
            for (int i = 0; i < arguments.Length; i++)
            {
                Arguments[i] = arguments[i];
            }
        }
        #endregion

        public Function Function
        {
            get
            {
                return func;
            }
            set
            {
                func = value;
                Arguments = new IElement[func?.ParamCount ?? 0];
            }
        }
        public IElement[] Arguments { get; private set; }
        public string OpeningBracket { get; set; }
        public string ClosingBracket { get; set; }
        public string Delim { get; set; }
        public bool DontExecute_flag { get; } = false;

        #region methods
        public IElement Pass()
        {
            return this;
        }
        public IElement Calc()
        {
            return func?.Calculate(Arguments);
        }
        public IntFloat? Execute()
        {
            return func?.Calculate(Arguments)?.Execute();
        }

        public override string ToString()
        {
            return ToSiString(null);
        }

        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent = null)
        {
            if (func == null) { return $"{StringProps.VoidLabel}{OpeningBracket}{ClosingBracket}"; }
            string ds = Arguments.Select(a => a.ToString(si)).ToDelimString($"{Delim} ");
            return $"{func.Name}{ds.Wrap(OpeningBracket, ClosingBracket)}";
        }
        #endregion

        private Function func;
    }

    public class Container : IElement
    {
        public Container(IElement item, string openBracket, string closeBracket)
        {
            Item = item;
            OpenBracket = openBracket;
            CloseBracket = closeBracket;
        }
        public Container(IElement item)
            : this(item, StringProps.OpenBrackets[0], StringProps.CloseBrackets[0]) { }

        public IElement Item { get; set; }
        public IElement Definition { get { return Item; } }
        public string OpenBracket { get; set; }
        public string CloseBracket { get; set; }

        public IElement Pass()
        {
            return Definition?.Pass();
        }
        public IElement Calc()
        {
            return Definition?.Calc();
        }
        public IntFloat? Execute()
        {
            return Definition?.Execute();
        }

        public override string ToString()
        {
            return (Item?.ToString() ?? StringProps.VoidLabel).Wrap(OpenBracket, CloseBracket);
        }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return (Item?.ToString(si) ?? StringProps.VoidLabel).Wrap(OpenBracket, CloseBracket);
        }
    }

    //public class FractionElement : IElement
    //{
    //    public BigFraction Value { get; set; }
    //    public FractionElement(BigFraction value)
    //    {
    //        Value = value;
    //    }
    //    public FractionElement Simplify()
    //    {
    //        Value = Value.Simplify();
    //        return this;
    //    }
    //    public IElement Calc() => new FractionElement(Value.Simplify());

    //    public IntFloat? Execute() => Value.ToIntFloat();
    //    public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
    //    {
    //        return $"frac({Value.Numerator}, {Value.Denominator})";
    //    }
    //    public override string ToString()
    //    {
    //        return ToSiString(null, null);
    //    }
    //}

    //public class ArraySelection : IElement
    //{
    //    public Array Array { get; set; }
    //    public IElement Index { get; set; }

    //    public IElement calc()
    //    {
    //        return Array.Get(index);
    //    }
    //}

    //public class Array
    //{
    //    public string Name { get; }
    //    public Dictionary<int, IElement> Contents { get; } = new Dictionary<int, IElement>();
    //    public Array(string name)
    //    {
    //        Name = name;
    //    }
    //    public IElement Get(IElement index, SymbolizedIndex si)
    //    {
    //        int? index_i = (int?)index.Execute();
    //        if (index_i == null) 
    //        { 
    //            throw new ExecutionException($"Array index for {Name}[{index}] cannot be {(index == null ? StringProps.VoidLabel : StringProps.NullLabel)}."); 
    //        }


    //        if (Contents.TryGetValue((int)index_i, out IElement value)){
    //            return value;
    //        }
    //        else
    //        {
    //            return IElement.Void;
    //        }
    //    }
    //}

}
