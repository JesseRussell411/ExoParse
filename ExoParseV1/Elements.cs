using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ExoParsev1
{
    public static class ElementUtils
    {
        public static double ToDouble(this bool self)
        {
            if (self)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }
        public static double? ToNullableDouble(this bool self)
        {
            return ((bool?)self).ToNullableDouble();
        }
        public static double? ToNullableDouble(this bool? self)
        {
            if (self == true)
            {
                return 1.0;
            }
            else if (self == false)
            {
                return 0.0;
            }
            else
            {
                return null;
            }
        }
        public static double? ToNullableDouble(this double? self)
        {
            if (self == 1.0)
            {
                return true.ToNullableDouble();
            }
            else if (self == 0.0)
            {
                return false.ToNullableDouble();
            }
            else
            {
                return (double?)null;
            }
        }

        #region toValuable
        public static Valuable ToValuable(this double? self)
        {
            return new constant(self);
        }
        public static Valuable ToValuable(this double self)
        {
            return new constant(self);
        }
        public static Valuable ToValuable(this int? self)
        {
            return new constant(self);
        }
        public static Valuable ToValuable(this int self)
        {
            return new constant(self);
        }
        public static Valuable ToValuable(this long self)
        {
            return new constant(self);
        }
        public static Valuable ToValuable(this bool self)
        {
            return self.ToDouble().ToValuable();
        }
        public static Valuable ToValuable(this string self)
        {
            return new Variable(self);
        }
        #endregion
        public static Valuable GetValuableNull()
        {
            return ((double?)null).ToValuable();
        }
        public static bool? ToNullableBool(this double? self)
        {
            if (self == 1.0)
            {
                return true;
            }
            else if (self == 0.0)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public static Valuable Equals(Valuable a, Valuable b) { return Equals(a?.Value, b?.Value).ToValuable(); }
        public static double? Equals(double? a, double? b)
        {
            return (a == b).ToNullableDouble();
        }
        public static Valuable NotEqual(Valuable a, Valuable b) { return NotEqual(a?.Value, b?.Value).ToValuable(); }
        public static double? NotEqual(double? a, double? b)
        {
            return (a != b).ToNullableDouble();
        }
        public static Valuable GreaterThan(Valuable a, Valuable b) { return GreaterThan(a?.Value, b?.Value).ToValuable(); }
        public static double? GreaterThan(double? a, double? b)
        {
            return (a > b).ToNullableDouble();
        }
        public static Valuable LessThan(Valuable a, Valuable b) { return LessThan(a?.Value, b?.Value).ToValuable(); }
        public static double? LessThan(double? a, double? b)
        {
            return (a < b).ToNullableDouble();
        }
        public static Valuable GreaterThanOrEqual(Valuable a, Valuable b) { return GreaterThanOrEqual(a?.Value, b?.Value).ToValuable(); }
        public static double? GreaterThanOrEqual(double? a, double? b)
        {
            return (a >= b).ToNullableDouble();
        }
        public static Valuable LessThanOrEqual(Valuable a, Valuable b) { return LessThanOrEqual(a?.Value, b?.Value).ToValuable(); }
        public static double? LessThanOrEqual(double? a, double? b)
        {
            return (a <= b).ToNullableDouble();
        }
        public static Valuable And(Valuable a, Valuable b) { return And(a?.Value, b?.Value).ToValuable(); }
        public static double? And(double? a, double? b)
        {
            return (a.ToNullableBool() & b.ToNullableBool()).ToNullableDouble();
        }
        public static Valuable Or(Valuable a, Valuable b) { return Or(a?.Value, b?.Value).ToValuable(); }
        public static double? Or(double? a, double? b)
        {
            return (a.ToNullableBool() | b.ToNullableBool()).ToNullableDouble();
        }
        public static Valuable Xor(Valuable a, Valuable b) { return Xor(a?.Value, b?.Value).ToValuable(); }
        public static double? Xor(double? a, double? b)
        {
            return (a.ToNullableBool() ^ b.ToNullableBool()).ToNullableDouble();
        }
        public static Valuable Not(Valuable n) { return Not(n?.Value).ToValuable(); }
        public static double? Not(double? n)
        {
            return (!n.ToNullableBool()).ToNullableDouble();
        }
        public static Valuable Neg(Valuable n) { return Neg(n?.Value).ToValuable(); }
        public static double? Neg(double? n)
        {
            return -n;
        }
        public static Valuable Pos(Valuable n) { return Pos(n?.Value).ToValuable(); }
        public static double? Pos(double? n)
        {
            return n;
        }
        public static Valuable Buffer(Valuable n) { return Buffer(n?.Value).ToValuable(); }
        public static double? Buffer(double? n)
        {
            return n.ToNullableDouble();
        }
        public static Valuable Min(Valuable a, Valuable b) { return Min(a?.Value, b?.Value).ToValuable(); }
        public static double? Min(double? a, double? b)
        {
            if (a == null) { return null; }
            if (b == null) { return null; }
            if (a.Value > b.Value)
            {
                return b;
            }
            else
            {
                return a;
            }
        }
        public static Valuable Max(Valuable a, Valuable b) { return Max(a?.Value, b?.Value).ToValuable(); }
        public static double? Max(double? a, double? b)
        {
            if (a == null) { return null; }
            if (b == null) { return null; }
            if (a.Value < b.Value)
            {
                return b;
            }
            else
            {
                return a;
            }
        }

        public static (string name, int argCount) FuncId(string name, int argCount)
        {
            return (name: name, argCount: argCount);
        }
    }

    #region interfaces and abstract classes
    public interface Element
    {
        public bool NonReplaceable { get; set; }
    }
    public interface Valuable : Element
    {
        #region properties
        public double? Value { get; }
        #endregion
        #region methods
        public Valuable Execute();
        #endregion
    }
    public interface Setable : Valuable
    {
        #region properties
        public new double? Value { get; set; }
        #endregion
    }
    public interface Defined : Element
    {
        #region properties
        public Valuable Definition { get; }
        #endregion
    }
    public interface Redefinable : Defined
    {
        #region properties
        public new Valuable Definition { get; set; }
        #endregion
    }
    #endregion

    #region Valuable objects
    public struct constant : Valuable
    {
        public bool NonReplaceable { get { return false; } set { } }
        public double? Value { get; }
        public constant(double? value)
        {
            Value = value;
        }
        public Valuable Execute()
        {
            return this;
        }
        public override string ToString()
        {
            return Value.NullableToString();
        }
    }
    public class Variable : Setable, Redefinable
    {
        #region properties
        public bool NonReplaceable { get; set; } = false;
        public double? Value
        {
            get
            {
                return Definition?.Value;
            }
            set
            {
                Definition = value.ToValuable();
            }
        }
        public Valuable Definition { get; set; } = null;
        public string Name { get; }
        #endregion
        #region constructors
        public Variable(string name)
        {
            Name = name;
        }
        #endregion
        #region methods
        public Valuable Execute()
        {
            return Definition;
        }
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
    public class ValuableModifier : Valuable
    {
        public bool NonReplaceable { get; set; } = false;
        public Valuable Item { get; set; }
        public Modifier Modifier { get; set; }
        public double? Value
        {
            get
            {
                return Execute()?.Value;
            }
        }

        public ValuableModifier()
        {

        }

        public ValuableModifier(Modifier modifier, Valuable item)
            : this()
        {
            Modifier = modifier;
            Item = item;
        }

        public Valuable Execute()
        {
            return Modifier?.Calculate(Item) ?? null;
        }
        public override string ToString()
        {
            return ToString(null);
        }
        public string ToString(int? parentPriority)
        {
            //bool toWrap = false;
            //else if (parentPriority != null && Modifier?.Orientation == Orientation.Before)
            //{
            //    toWrap = Modifier == null ? false : parentPriority < Modifier.Priority;
            //}

            string item_string;
            if (Item == null)
            {
                item_string = "void";
            }
            else if (Item is ValuableModifier || (Item is Expression && ((Expression)Item).Items.Length > 1))
            {
                item_string = $"({Item.NullableToString("void")})";
            }
            else
            {
                item_string = $"{Item.NullableToString("void")}";
            }

            if (Modifier == null)
            {
                return item_string;
            }
            else if (Modifier.Orientation == Orientation.After)
            {
                return $"{item_string}{Modifier.Symbol}";
            }
            else /*if(Modifier.Orientation == Orientation.Before)*/
            {
                //if (toWrap)
                //{
                //    return $"({Modifier.Symbol}{item_string})";
                //}
                //else
                //{
                return $"{Modifier.Symbol}{item_string}";
                //}
            }
        }
    }
    public class Expression : Valuable, IEnumerable<(Operator op, Valuable val)>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<(Operator op, Valuable val)> GetEnumerator()
        {
            foreach(var item in Items)
            {
                yield return item;
            }
        }
        #region properties
        public bool NonReplaceable { get; set; } = false;
        public double? Value { get { return Execute()?.Value; } }
        public int? CommonPriority
        {
            get
            {
                if (commonIsSet)
                {
                    return commonPriority;
                }
                else
                {
                    return null;
                }
            }
        }
        public Direction? CommonDirection
        {
            get
            {
                if (commonIsSet)
                {
                    return commonDirection;
                }
                else
                {
                    return null;
                }
            }
        }

        #region item management(0)
        public Valuable[] ValuedItems { get { return items.Select(v => v.val).ToArray(); } }
        public Operator[] OperatorItems { get { return items.Select(v => v.op).ToArray(); } }
        public (Operator op, Valuable val)[] Items { get { return items.ToArray(); } }
        public void SetValue(int index, Valuable val)
        {
            items[index] = (items[index].op, val);
        }
        public void SetOperator(int index, Operator op)
        {
            items[index] = (op, items[index].val);
        }
        public void SetItem(int index, (Operator op, Valuable val) item)
        {
            items[index] = item;
        }
        #endregion
        #endregion

        #region constructors
        public Expression()
        { }

        public Expression(Valuable item) : this()
        {
            this.AddItem(item);
        }
        #endregion

        #region methods
        public Valuable Execute()
        {
            #region sanity checking
            int items_count = items.Count;
            if (items_count == 0)
            {
                return ((double?)null).ToValuable();
            }
            else if (items_count == 1)
            {
                (Operator op, Valuable val) item = items[0];
                if (item.op == null)
                {
                    return item.val;
                }
                else
                {
                    return item.op.Calculate(null, item.val);
                }
            }
            #endregion

            int lowBound = 0;
            int highBound = items_count - 1;

            // Left to right instead of right to left: this boolean stores if the direction is left to right as opose to righ to left.
            bool LtoR = CommonDirection == Direction.LeftToRight;
            //bool LtoR = CommonDirection.GetType() == typeof(LeftToRight);

            int index = LtoR ? lowBound : highBound;

            int endBound = LtoR ? highBound : lowBound;

            int lead = LtoR ? 1 : -1;
            Valuable carry;
            if (LtoR)
            {
                if (items[0].op == null)
                {
                    carry = items[1].op.Calculate(items[0].val, items[1].val);
                    index = 2;
                }
                else
                {
                    carry = items[0].op.Calculate(null, items[0].val);
                    index = 1;
                }
            }
            else
            {
                carry = items[highBound].op.Calculate(items[highBound - 1].val, items[highBound].val);
                index = highBound - 2;
            }

            Func<int, int, bool> pastEnd;
            Func<List<(Operator op, Valuable val)>, int, Operator> getOp;
            Func<List<(Operator op, Valuable val)>, int, Valuable, Valuable> selectA;
            Func<List<(Operator op, Valuable val)>, int, Valuable, Valuable> selectB;
            if (LtoR)
            {
                pastEnd = (i, ei) => i > ei;
                getOp = (items, i) => items[i].op;
                selectA = (items, i, carry) => carry;
                selectB = (items, i, carry) => items[i].val;
            }
            else
            {
                pastEnd = (i, ei) => i < ei;
                getOp = (items, i) => items[i + 1].op;
                selectA = (items, i, carry) => items[i].val;
                selectB = (items, i, carry) => carry;
            }

            while (!pastEnd(index, endBound))
            {
                carry = getOp(items, index).Calculate(selectA(items, index, carry), selectB(items, index, carry));
                index += lead;
            }

            if (!LtoR)
            {
                if (items[0].op != null)
                {
                    carry = items[0].op.Calculate(null, carry);
                }
            }

            return carry;
        }

        #region item management(1)
        public bool AddItem(Operator op)
        {
            return AddItem(op, null);
        }
        public bool AddItem(double? value)
        {
            return AddItem(value.ToValuable());
        }
        public bool AddItem(Operator op, double value)
        {
            return AddItem(op, value.ToValuable());
        }
        public bool AddItem(Valuable item)
        {
            return AddItem(lastItemOperator, item);
        }
        public bool AddItem(Operator op, Valuable item)
        {
            if (items.Any() && op == null) { return false; }

            bool Doit()
            {
                items.Add((op: op, val: item));
                return true;
            }

            if (!commonIsSet)
            {
                setCommon(op);
                return Doit();
            }
            else if (op.Direction == CommonDirection && op.Priority == CommonPriority)
            {
                return Doit();
            }
            else
            {
                return false;
            }
        }

        public void ClearItems()
        {
            items.Clear();
            resetCommon();
        }

        public void RemoveItemAt(int index)
        {
            items.RemoveAt(index);
            if (!items.Any())
            {
                resetCommon();
            }
        }
        #endregion
        public override string ToString()
        {
            return ToString(null);
        }
        public string ToString(int? parentPriority)
        {
            // This method takes care of calling the proper toString method when getting the item's value's string.
            string getString(Valuable val)
            {
                if (val == null) { return "void"; }
                if (val.GetType() == typeof(Expression))
                {
                    return ((Expression)val).ToString(CommonPriority);
                }
                else
                {
                    return val.NullableToString();
                }
            }

            // Should this expression be wrapped in parathesis to protect the order of operations?
            bool wrap = false;
            if (parentPriority == null)
            {
                wrap = false;
            }
            else if (!commonIsSet)
            {
                wrap = true;
            }
            else
            {
                wrap = parentPriority >= CommonPriority;
            }

            if (Items.Length == 1)
            {
                wrap = true;
            }

            // Return "void" if there are no items
            if (Items.Length == 0)
            {
                return "void";
            }

            // Create the StringBuilder (Mutable string) and prime it with an opening parenthasis if necesary.
            StringBuilder outBuilder = new StringBuilder(wrap ? "(" : "");

            // Append each item's value preceded by it's operator.
            foreach (var item in Items)
            {
                outBuilder.Append(item.op?.ToString());
                outBuilder.Append(getString(item.val));
            }

            // Add the ending parenthesis if necesary.
            if (wrap)
            {
                outBuilder.Append(")");
            }

            // Return.
            return outBuilder.ToString();
        }
        #endregion

        #region hidden
        private Operator lastItemOperator
        {
            get
            {
                return items.LastOrDefault().op;
            }
        }

        private List<(Operator op, Valuable val)> items = new List<(Operator op, Valuable val)>();

        #region common operator management
        private int commonPriority = 0;
        private Direction commonDirection = Direction.LeftToRight;
        private bool commonIsSet = false;
        private void resetCommon()
        {
            commonIsSet = false;
        }
        private bool setCommon(Operator op)
        {
            if (op != null)
            {
                commonDirection = op.Direction;
                commonPriority = op.Priority;
                commonIsSet = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool setCommon(Direction direction, int priority)
        {
            commonDirection = direction;
            commonPriority = priority;
            commonIsSet = true;
            return true;
        }
        #endregion
        #endregion
    }
    public class FunctionHolder : Valuable
    {
        #region properties
        public double? Value { get { return Execute()?.Value; } }
        public bool NonReplaceable { get; set; } = false;
        public Function Function
        {
            get
            {
                return function;
            }
            set
            {
                function = value;
                if (function == null)
                {
                    Arguments = new Valuable[0];
                }
                else
                {
                    Arguments = new Valuable[function.Arguments.Length];
                }
            }
        }
        private Function function = null;
        public Valuable[] Arguments { get; private set; }
        public FunctionHolder()
        {

        }
        public FunctionHolder(Function function)
            : this()
        {
            Function = function;
        }
        public FunctionHolder(Function function, Valuable[] arguments)
            : this()
        {
            Function = function;
            TrySetArguments(arguments);
        }
        #endregion
        public Valuable Execute()
        {
            return function.Calculate(Arguments);
        }

        public bool TrySetArguments(Valuable[] values)
        {
            if (values.Length != Arguments.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < Arguments.Length; i++)
                {
                    Arguments[i] = values[i];
                }
                return true;
            }
        }
        public override string ToString()
        {
            if (function == null)
            {
                return "void";
            }
            else
            {
                return $"{function.Name}({Arguments.ToDelimString(", ")})";
            }
        }
    }

    public struct messenger : Valuable
    {
        public bool NonReplaceable { get; set; }
        public double? Value { get { return null; } }
        public Valuable Execute() { return this; }
        public string Message { get; set; }
        public Valuable[] Packages { get; set; }
        public object GenericPackage { get; set; }
        public messenger(string message, Valuable[] packages)
        {
            NonReplaceable = false;
            Message = message;
            Packages = packages;
            GenericPackage = null;
        }
    }
    #endregion
}