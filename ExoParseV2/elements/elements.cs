using ParsingTools;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ExoParseV2.elements
{
    /// <summary>
    /// The simplest element. Contains a read-only property for the double? value and nothing else.
    /// </summary>
    public struct Literal : IElement
    {
        public readonly double? Value;
        public Literal(double? value)
        {
            Value = value;
        }
        public bool DontExecute_flag { get { return false; } }
        public double? Execute() { return Value; }
        public IElement Pass() { return this; }
        public IElement Calc() { return this; }
        public IElement Definition { get { return this; } }
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
        public static Literal Null { get { return new Literal(null); } }

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
            if (s.Length == 0) { result = Literal.Null; return false; }//--(FAIL)--
            if (!trapNegPos && (s[0] == '-' || s[0] == '+')) { result = Literal.Null; return false; }//--(FAIL)--
            if (s == StringProps.NullLabel) { result = Literal.Null; return true; }//--(PASS)--

            if (double.TryParse(s, out double d))
            {
                result = new Literal(d);
                return true;//--(PASS)--
            }
            else
            {
                result = Literal.Null;
                return false; //--(FAIL)--
            }
        }

        #endregion
    }

    public class Constant : ILabeled
    {
        public string Name { get; }
        public bool DontExecute_flag { get; } = false;
        public IElement Definition { get; }
        public double? Execute() { return Definition?.Execute(); }
        public IElement Pass() { return this; }
        public IElement Calc() { return Definition; }
        public Constant(String name, IElement definition)
        {
            Definition = definition;
            Name = name;
        }
        public Constant(string name, double? value)
        {
            Definition = value.ToElement();
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }
    }

    public class Variable : IRedefinable, ILabeled
    {
        public string Name { get; }
        public IElement Definition { get; set; }
        public bool DontExecute_flag { get; } = false;
        public double? Execute() { return Definition?.Execute(); }
        public IElement Pass() { return this; }
        public IElement Calc() { return Definition; }
        public Variable(String name)
        {
            Name = name;
        }
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
        public IElement A { get; set; }
        public IElement B { get; set; }
        public Operator Operator { get; set; }
        public IElement Definition { get { return this; } }
        public bool DontExecute_flag
        {
            get
            {
                return Operator.dontExecute_flag(A, B, this);
            }
        }


        public Operation(Operator op, double? a, double? b)
            : this(op, a.ToElement(), b.ToElement()) { }

        public Operation(Operator op, long? a, long? b)
            : this(op, a.ToElement(), b.ToElement()) { }


        public Operation(Operator op, IElement a, IElement b)
        {
            Operator = op;
            A = a;
            B = b;
        }

        public double? Execute()
        {
            //return Pass()?.Execute();
            return Operator?.Calc(A, B)?.Execute();
        }
        public IElement Calc()
        {
            return Operator?.Calc(A, B);
        }
        public IElement Pass()
        {
            //return Operator?.Execute(A?.Pass(), B?.Pass());
            return Operator?.Pass(A, B, this);
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
            //A_string = A == null ? "" : (A is IExpressionComponent a ? a.ToString(si, this) : A.ToString());
            //B_string = B == null ? "" : (B is IExpressionComponent b ? b.ToString(si, this) : B.ToString());

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

        public IElement Definition { get { return this; } }
        public bool DontExecute_flag
        {
            get
            {
                return Modifier is Dereference_mod;
            }
        }

        public double? Execute()
        {
            return Modifier?.Calc(Item?.Pass())?.Execute();
            //return Modifier?.calc(Item)?.Execute();
        }
        public IElement Calc()
        {
            return Modifier?.Calc(Item?.Pass());
            //return Modifier?.calc(Item)?.Execute();
        }
        public IElement Pass()
        {
            return Modifier?.Pass(Item.Pass(), this);
            //return Modifier?.calc(Item?.Pass());
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

        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(Modifier);
        }

        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            bool toWrap = true;
            if (parent == null)
            {
                toWrap = false;
            }
            else
            {
                bool checkPriority = true;
                if (parent is Operation)
                {
                    IElement check = Modifier is PreModifier ? ((Operation)parent)?.A : ((Operation)parent)?.B;
                    checkPriority = (this != check);
                }

                if (checkPriority)
                {
                    toWrap = (parent.GetPriority(si) < ((IExpressionComponent)this).GetPriority(si));
                }
                else
                {
                    toWrap = false;
                }
            }
            string Item_string = Item.ToString(si, this);
            //string Item_string = Item is IExpressionComponent component ? component.ToString(si, this) : Item.ToString();
            return Modifier is PreModifier ? $"{Modifier}{Item_string}" : $"{Item_string}{Modifier}".Wrap(StringProps.OpenBrackets[0], StringProps.CloseBrackets[0]);
        }
    }

    public class Execution : IElement
    {
        public bool DontExecute_flag { get; } = false;
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
        public string OpeningBracket { get; set; }
        public string ClosingBracket { get; set; }
        public string Delim { get; set; }
        public IElement[] Arguments { get; private set; }
        public IElement Definition { get { return this; } }
        public double? Execute()
        {
            return func.Calculate(Arguments)
                       .Execute();
        }
        public IElement Calc()
        {
            return func.Calculate(Arguments);
        }
        public IElement Pass()
        {
            return this;
        }

        public override string ToString()
        {
            return ToSiString(null);
        }

        public string ToSiString(SymbolizedIndex si = null, IExpressionComponent parent = null)
        {
            if (func == null) { return $"{StringProps.VoidLabel}{OpeningBracket}{ClosingBracket}"; }
            Func<IElement[], string> ds;
            if (si == null)
            {
                ds = (IElement[] e) => e.ToDelimString(Delim + " ");
            }
            else
            {
                ds = (IElement[] e) => e.ToDelimString(si, Delim + " ");
            }
            return $"{func.Name}{ds(Arguments).Wrap(OpeningBracket, ClosingBracket)}";
        }




        private Function func;
    }

    public class Container : IElement
    {
        public bool DontExecute_flag
        {
            get
            {
                return Definition?.DontExecute_flag ?? false;
            }
        }
        public Container(IElement item, string openBracket, string closeBracket)
        {
            Item = item;
            OpenBracket = openBracket;
            CloseBracket = closeBracket;
        }
        public Container(IElement item)
            : this(item, StringProps.OpenBrackets[0], StringProps.CloseBrackets[0])
        {
        }
        public IElement Item { get; set; }
        public string OpenBracket { get; set; }
        public string CloseBracket { get; set; }
        public IElement Definition
        {
            get
            {
                return Item;
            }
            set
            {
                Item = value;
            }
        }
        public double? Execute()
        {
            return Definition?.Execute();
        }
        public IElement Pass()
        {
            return Definition?.Pass();
        }
        public IElement Calc()
        {
            return Definition?.Calc();
        }
        public override string ToString()
        {
            //return Definition.NullableToString(ParsingProps.VoidLabel).Wrap(ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0]);
            return Item?.ToString()?.Wrap(OpenBracket, CloseBracket) ?? StringProps.VoidLabel;
        }
        public string ToSiString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return Item?.ToString(si, null)?.Wrap(OpenBracket, CloseBracket) ?? StringProps.VoidLabel;
        }
    }

    #region special elements
    public class Messenger : IElement
    {
        public readonly object From;
        public readonly IList<object> Contents;
        public bool DontExecute_flag { get; } = true;
        public IElement Definition { get { return ElementUtils.NullElement; } }

        public double? Execute()
        {
            return null;
        }
        public IElement Pass()
        {
            return this;
        }
        public IElement Calc()
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
        public IElement A { get => (IElement) Contents[0]; }
        public IElement B { get => (IElement) Contents[1]; }
        

        public TernaryMessenger(object from, IList<object> contents)
            : base(from, contents) { }
        public TernaryMessenger(object from, IElement A, IElement B)
            : base(from, new IElement[] { A, B }) { }
    }
    #endregion
}
