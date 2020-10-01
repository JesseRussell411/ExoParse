using ParsingTools;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ExoParseV2
{
    public interface IElement
    {
        public double? Execute();
        public IElement Pass();
        public IElement Pass(out bool dontExecute_flag)
        {
            dontExecute_flag = DontExecute_flag;
            return Pass();
        }
        public virtual bool DontExecute_flag { get { return false; } }
        public IElement Definition { get; }

        public static IElement Null { get { return new Constant(null); } }

        public static IElement Void { get { return null; } }
        public string ToString(SymbolizedIndex si, IExpressionComponent parent);
        
    }

    public interface IDefinable
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

    public struct Constant : IElement
    {
        public readonly double? Value;
        public Constant(double? value)
        {
            Value = value;
        }
        public double? Execute() { return Value; }
        public IElement Pass() { return this; }
        public IElement Definition { get { return this; } }
        public override string ToString()
        {
            if (Value == null) { return ParsingProps.NullLabel; }

            string s = Value.ToString();

            return s.Contains('E') ? $" {s} " : s;
        }

        public string ToString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }

        #region static
        public static Constant Null { get { return new Constant(null); } }
        public static Constant Parse(String s, bool containNegPos = false)
        {
            if (TryParse(s, out Constant result, containNegPos))
            {
                return result;
            }
            else
            {
                throw new FormatException($"\"{s}\" is not a valid constant.");
            }

        }
        public static bool TryParse(String s, out Constant result, bool containNegPos = false)
        {
            if (s.Length == 0) { result = Constant.Null; return false; }
            if (!containNegPos && (s[0] == '-' || s[0] == '+')) { result = Constant.Null; return false; }
            if (s == ParsingProps.NullLabel) { result = Constant.Null; return true; }
            if (double.TryParse(s, out double d))
            {
                result = new Constant(d);
                return true;
            }
            else
            {
                result = Constant.Null;
                return false; //--(FAIL)--
            }
        }
        
        #endregion
    }
    
    public class FinalVariable : ILabeled
    {
        public string Name { get; }
        public IElement Definition { get; }
        public double? Execute() { return Definition?.Execute(); }
        public IElement Pass() { return Definition; }
        public FinalVariable(String name, IElement definition)
        {
            Definition = definition;
            Name = name;
        }
        public FinalVariable(string name, double? value)
        {
            Definition = value.ToElement();
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
        public string ToString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }
    }

    public class Variable : IDefinable, ILabeled
    {
        public string Name { get; }
        public IElement Definition { get; set; }
        public double? Execute() { return Definition?.Execute(); }
        public IElement Pass() { return Definition; }
        public Variable(String name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
        public string ToString(SymbolizedIndex si, IExpressionComponent parent)
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
                return Operator is SetDefinition_op ||
                    Operator is SetAsDefinition_op;
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
            return Pass()?.Execute();
        }
        public IElement Pass()
        {
            return Operator?.Execute(A, B);
        }

        public override string ToString()
        {
            string A_string = A is IExpressionComponent ? A?.ToString()?.Wrap(ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0]) ?? ParsingProps.VoidLabel : A?.ToString() ?? ParsingProps.VoidLabel;
            string B_string = B is IExpressionComponent ? B?.ToString()?.Wrap(ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0]) ?? ParsingProps.VoidLabel : B?.ToString() ?? ParsingProps.VoidLabel;
            return $"{A_string}{Operator}{B_string}";
        }

        public int GetPriority(SymbolizedIndex si)
        {
            return si.GetPriority(Operator);
        }

        public string ToString(SymbolizedIndex si, IExpressionComponent parent)
        {
            bool toWrap = parent != null && parent.GetPriority(si) > ((IExpressionComponent)this).GetPriority(si);

            string returnString, A_string, B_string;

            A_string = A?.ToString(si, this) ?? ParsingProps.VoidLabel;
            B_string = B?.ToString(si, this) ?? ParsingProps.VoidLabel;
            //A_string = A == null ? "" : (A is IExpressionComponent a ? a.ToString(si, this) : A.ToString());
            //B_string = B == null ? "" : (B is IExpressionComponent b ? b.ToString(si, this) : B.ToString());

            returnString = $"{A_string}{Operator}{B_string}";

            if (toWrap)
            {
                return returnString.Wrap(ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0]);
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
            return Modifier?.calc(Item)?.Execute();
        }
        public IElement Pass()
        {
            return Modifier?.calc(Item);
        }
        public override string ToString()
        {
            String itemString;
            itemString = (Item is IExpressionComponent) ? Item.ToString().Wrap(ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0]) : Item.ToString();


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

        public string ToString(SymbolizedIndex si, IExpressionComponent parent)
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
            return Modifier is PreModifier ? $"{Modifier}{Item_string}" : $"{Item_string}{Modifier}".Wrap(ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0]);
        }
    }

    public class Execution : IElement
    {
        public Execution(Function function, params IElement[] arguments)
            : this(function, arguments, null, null) { }
        public Execution(Function function, IElement[] arguments, string openingBracket = null, string closingBracket = null, string delim = null)
        {
            if (openingBracket == null) { openingBracket = ParsingProps.OpenBrackets[0]; }
            if (closingBracket == null) { closingBracket = ParsingProps.CloseBrackets[0]; }
            if (delim == null) { delim = ParsingProps.Delims[0]; }
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
        public IElement Pass()
        {
            return func.Calculate(Arguments);
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(SymbolizedIndex si = null, IExpressionComponent parent = null)
        {
            if (func == null) { return $"{ParsingProps.VoidLabel}{OpeningBracket}{ClosingBracket}"; }
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

    public class Container : IElement, IDefinable
    {
        public Container(IElement item, string openBracket, string closeBracket)
        {
            Item = item;
            OpenBracket = openBracket;
            CloseBracket = closeBracket;
        }
        public Container(IElement item)
            : this(item, ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0])
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
            return Definition;
        }
        public override string ToString()
        {
            //return Definition.NullableToString(ParsingProps.VoidLabel).Wrap(ParsingProps.OpenBrackets[0], ParsingProps.CloseBrackets[0]);
            return Item?.ToString()?.Wrap(OpenBracket, CloseBracket) ?? ParsingProps.VoidLabel;
        }
        public string ToString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return Item?.ToString(si, null)?.Wrap(OpenBracket, CloseBracket) ?? ParsingProps.VoidLabel;
        }
    }

    #region special elements
    public enum MessageType
    {
        Ternary
    }

    public class Messenger : IElement
    {
        public readonly MessageType? Type;
        public readonly object From;
        public readonly ImmutableArray<object> Contents;

        public IElement Definition { get { return ElementUtils.NullElement; } }

        public double? Execute()
        {
            return null;
        }
        public IElement Pass()
        {
            return this;
        }

        public Messenger(object from, ImmutableArray<object> contents, MessageType? type)
        {
            From = from;
            Contents = contents;
            Type = type;
        }
        public string ToString(SymbolizedIndex si, IExpressionComponent parent)
        {
            return ToString();
        }
    }
    #endregion
}
