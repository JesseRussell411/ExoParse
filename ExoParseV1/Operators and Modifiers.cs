using System;
using System.Numerics;

namespace ExoParsev1
{
    //turns out finding the factorial of a fractional number is REALLY complicated
    /*
    public class Factorial : Modifier
    {
        public override string Symbol { get; } = "!";
        public override Orientation Orientation { get; } = Orientation.After;
        protected override Valuable Calc(Valuable v)
        {
            throw new NotImplementedException();
        }
    }
    */
    public interface SymbolRepresented
    {
        public string Symbol { get; }
        public int Priority { get; }
    }

    public enum Direction
    {
        LeftToRight,
        RightToLeft
    }
    public enum Orientation
    {
        Before,
        After
    }

    public abstract class Operator : SymbolRepresented
    {
        #region properties
        public abstract string Symbol { get; }
        public abstract int Priority { get; }
        public virtual Direction Direction { get; } = Direction.LeftToRight;
        public string Id
        {
            get
            {
                return Symbol;
            }
        }
        #endregion

        #region methods
        public Valuable Calculate(Valuable a, Valuable b)
        {
            bool a_void = (a == null);
            bool b_void = (b == null);

            if (a_void & b_void)
            {
                return CalcNone();
            }
            else if (a_void & !b_void)
            {
                return CalcBOnly(b);
            }
            else if (!a_void & b_void)
            {
                return CalcAOnly(a);
            }
            else /* if (!a_void & !b_void) */
            {
                return Calc(a, b);
            }
        }

        public override string ToString()
        {
            if (toStringPadding)
            {
                return $" {Symbol} ";
            }
            else
            {
                return Symbol;
            }
        }
        #endregion

        #region hidden
        protected virtual bool toStringPadding { get; } = true;
        protected abstract Valuable Calc(Valuable a, Valuable b);
        protected virtual Valuable CalcAOnly(Valuable a)
        {
            return null;
        }
        protected virtual Valuable CalcBOnly(Valuable b)
        {
            return null;
        }
        protected virtual Valuable CalcNone()
        {
            return null;
        }
        #endregion
    }

    public abstract class Modifier : SymbolRepresented
    {
        public abstract string Symbol { get; }
        public virtual Orientation Orientation { get; } = Orientation.Before;
        public abstract int Priority { get; }

        public (string symbol, Orientation orientation) Id
        {
            get
            {
                return (Symbol, Orientation);
            }
        }

        public Valuable Calculate(Valuable v)
        {
            return Calc(v ?? ElementUtils.GetValuableNull());
        }
        public override string ToString()
        {
            return Symbol;
        }
        protected abstract Valuable Calc(Valuable v);
    }

    #region operators and modifiers
    // Rules for operators/modifiers:
    // 1. Operators of the same priority must have the same direction.
    // 2. Operators and Modifiers can never have the same priority.
    // 3. Modifiers of the same priority must have the same orientation.
    // 4. Only Modifiers of Orientation.Before are effected by priority. it doesn't matter what you give Orientation.After modifiers for priority.


    // Priority -3
    public class GetValue : Modifier
    {
        public override string Symbol { get; } = "$";
        public override int Priority { get; } = -3;
        protected override Valuable Calc(Valuable v)
        {
            return v.Value.ToValuable();
        }
    }

    // Priority -2 R->L
    public class SetDefinition : Operator
    {
        public override string Symbol { get; } = ":=";
        public override int Priority { get; } = -2;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            try
            {
                ((Redefinable)a).Definition = b;
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
        protected override Valuable CalcAOnly(Valuable a)
        {
            try
            {
                ((Redefinable)a).Definition = ElementUtils.GetValuableNull();
                return a;
            }
            catch (InvalidCastException ice)
            {
                return null;
            }
        }
    }
    public class NullCoalescingSetDefinition : Operator
    {
        public override string Symbol { get; } = "?:=";
        public override int Priority { get; } = -2;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (a is Redefinable && ((Redefinable)a).Definition == null)
            {
                ((Redefinable)a).Definition = b;
                return a;
            }
            else
            {
                throw new MessageException($"{a} is not redefinable.");
            }
        }
        protected override Valuable CalcAOnly(Valuable a)
        {
            if (a.Value == null)
            {
                try
                {
                    ((Redefinable)a).Definition = ElementUtils.GetValuableNull();
                    return a;
                }
                catch (InvalidCastException ice)
                {
                    return a;
                }
            }
            else
            {
                return null;
            }
        }
    }
    public class NullCoalescingSetAsDefinition : Operator
    {
        public override string Symbol { get; } = "?:=@";
        public override int Priority { get; } = -2;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (a is Redefinable && ((Redefinable)a).Definition == null)
            {
                ((Redefinable)a).Definition = ((Defined)b).Definition;
                return a;
            }
            else
            {
                throw new MessageException($"{a} is not redefinable.");
            }
        }
        protected override Valuable CalcAOnly(Valuable a)
        {
            if (a.Value == null)
            {
                try
                {
                    ((Redefinable)a).Definition = ElementUtils.GetValuableNull();
                    return a;
                }
                catch (InvalidCastException ice)
                {
                    throw new MessageException($"{a} is not redefinable.");
                }
            }
            else
            {
                return null;
            }
        }
    }
    public class SetAsDefinition : Operator
    {
        public override string Symbol { get; } = ":=@";
        public override int Priority { get; } = -2;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (!(a is Redefinable)) { throw new MessageException($"{a} is not redefinable."); }
            if (!(b is Defined)) { throw new MessageException($"{b} is not definable."); }
            ((Redefinable)a).Definition = ((Defined)b).Definition;
            return a;
        }
        protected override Valuable CalcAOnly(Valuable a)
        {
            try
            {
                ((Redefinable)a).Definition = ElementUtils.GetValuableNull();
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not redefinable.");
            }
        }
    }

    // Priority -1 R->L
    public class TernaryStatement : Operator
    {
        public override string Symbol { get; } = "?";
        public override int Priority { get; } = -1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            Valuable b_Execute = b.Execute();
            if (b_Execute is messenger && ((messenger)b_Execute).Packages.Length == 2)
            {
                double? a_Value = a.Value;
                if (a_Value == 0.0)
                {
                    return ((messenger)b_Execute).Packages[1];
                }
                else if (a_Value == 1.0)
                {
                    return ((messenger)b_Execute).Packages[0];
                }
            }
            return null;
        }
    }
    public class TernarySeperator : Operator
    {
        public override string Symbol { get; } = ":";
        public override int Priority { get; } = -1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return new messenger("", new Valuable[] { a, b });
        }
    }


    // Priority 0    R->L
    public class And : Operator
    {
        public override string Symbol { get; } = "&";
        public override int Priority { get; } = 0;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.And(a.Value, b.Value).ToValuable();
        }
    }
    public class ConditionalAnd : Operator
    {
        public override string Symbol { get; } = "&&";
        public override int Priority { get; } = 0;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            double? a_Value = a.Value;
            if (a_Value == 1.0)
            {
                return ElementUtils.And(a_Value, b.Value).ToValuable();
            }
            else if (a_Value == 0.0)
            {
                return 0.0.ToValuable();
            }
            else
            {
                return ElementUtils.GetValuableNull();
            }
        }
    }
    public class Or : Operator
    {
        public override string Symbol { get; } = "|";
        public override int Priority { get; } = 0;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.Or(a.Value, b.Value).ToValuable();
        }
    }
    public class ConditionalOr : Operator
    {
        public override string Symbol { get; } = "||";
        public override int Priority { get; } = 0;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            double? a_Value = a.Value;
            if (a_Value == 0.0)
            {
                return ElementUtils.Or(a_Value, b.Value).ToValuable();
            }
            else if (a_Value == 1.0)
            {
                return 1.0.ToValuable();
            }
            else
            {
                return ElementUtils.GetValuableNull();
            }
        }
    }
    public class Xor : Operator
    {
        public override string Symbol { get; } = "^^";
        public override int Priority { get; } = 0;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.Xor(a.Value, b.Value).ToValuable();
        }
    }

    // Priority 1    R->L
    public class CheckEqual : Operator
    {
        public override string Symbol { get; } = "==";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.Equals(a, b);
        }
    }
    public class CheckNotEqual : Operator
    {
        public override string Symbol { get; } = "!=";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.NotEqual(a, b);
        }
    }
    public class GreaterThen : Operator
    {
        public override string Symbol { get; } = ">";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.GreaterThan(a, b);
        }
    }
    public class Lessthen : Operator
    {
        public override string Symbol { get; } = "<";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.LessThan(a, b);
        }
    }
    public class GreaterThenEqualTo : Operator
    {
        public override string Symbol { get; } = ">=";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.GreaterThanOrEqual(a, b);
        }
    }
    public class LessthenEqualto : Operator
    {
        public override string Symbol { get; } = "<=";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return ElementUtils.LessThanOrEqual(a, b);
        }
    }
    public class CheckDef : Operator
    {
        public override string Symbol { get; } = ":==";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (a is Defined && b is Defined)
            {
                return (((Defined)a).Definition == ((Defined)b).Definition).ToValuable();
            }
            else
            {
                return null;
            }
        }
    }
    public class CheckNotDef : Operator
    {
        public override string Symbol { get; } = ":!=";
        public override int Priority { get; } = 1;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (a is Defined && b is Defined)
            {
                return (((Defined)a).Definition != ((Defined)b).Definition).ToValuable();
            }
            else
            {
                return null;
            }
        }
    }

    // Priority 2

    // Priority 3
    public class PlusEquals : Operator
    {
        public override string Symbol { get; } = "+=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            try
            {
                ((Setable)a).Value = a.Value + b.Value;
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
    }
    public class MinusEquals : Operator
    {
        public override string Symbol { get; } = "-=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            try
            {
                ((Setable)a).Value = a.Value - b.Value;
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
    }
    public class TimesEquals : Operator
    {
        public override string Symbol { get; } = "*=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            try
            {
                ((Setable)a).Value = a.Value * b.Value;
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
    }
    public class DivEquals : Operator
    {
        public override string Symbol { get; } = "/=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            try
            {
                ((Setable)a).Value = a.Value / b.Value;
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
    }
    public class FloorDivEquals : Operator
    {
        public override string Symbol { get; } = "//=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (!(a is Setable)) { throw new MessageException($"{a} is not setable."); }

            double? a_value = a.Value;
            double? b_value = b.Value;

            if (a_value == null || b_value == null)
            {
                ((Setable)a).Value = null;
                return a;
            }
            else
            {
                ((Setable)a).Value = Math.Floor((double)(a_value / b_value));
                return a;
            }
        }
    }
    public class ExponentiateEquals : Operator
    {
        public override string Symbol { get; } = "^=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (!(a is Setable)) { throw new MessageException($"{a} is not setable."); }

            double? a_value = a.Value;
            double? b_value = b.Value;

            if (a_value == null || b_value == null)
            {
                ((Setable)a).Value = null;
                return a;
            }
            else
            {
                ((Setable)a).Value = Math.Pow((double)a_value, (double)b_value);
                return a;
            }
        }
    }
    public class ModulusEquals : Operator
    {
        public override string Symbol { get; } = "%=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (!(a is Setable)) { throw new MessageException($"{a} is not setable."); }

            double? a_value = a.Value;
            double? b_value = b.Value;

            if (a_value == null || b_value == null)
            {
                ((Setable)a).Value = null;
                return a;
            }
            else
            {
                ((Setable)a).Value = a_value % b_value;
                return a;
            }
        }
    }
    public class NullcoalescingSetEquals : Operator
    {
        public override string Symbol { get; } = "?=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (a is Setable && a.Value == null)
            {
                ((Setable)a).Value = b.Value;
                return a;
            }
            else
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
    }
    public class SetEqual : Operator
    {
        public override string Symbol { get; } = "=";
        public override int Priority { get; } = 3;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            try
            {
                ((Setable)a).Value = b.Value;
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
        protected override Valuable CalcAOnly(Valuable a)
        {
            try
            {
                ((Setable)a).Value = null;
                return a;
            }
            catch (InvalidCastException ice)
            {
                throw new MessageException($"{a} is not setable.");
            }
        }
    }

    // Priority 4    L->R
    public class Addition : Operator
    {
        public override string Symbol { get; } = "+";
        public override int Priority { get; } = 4;

        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return (a.Value + b.Value).ToValuable();
        }
        protected override Valuable CalcBOnly(Valuable b)
        {
            return (+b.Value).ToValuable();
        }
    }
    public class Subtraction : Operator
    {
        public override string Symbol { get; } = "-";
        public override int Priority { get; } = 4;

        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return (a.Value - b.Value).ToValuable();
        }
        protected override Valuable CalcBOnly(Valuable b)
        {
            return (-b.Value).ToValuable();
        }
    }
    // Priority 5
    public class Multiplication : Operator
    {
        public override string Symbol { get; } = "*";
        public override int Priority { get; } = 5;

        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return (a.Value * b.Value).ToValuable();
        }
    }
    public class Division : Operator
    {
        public override string Symbol { get; } = "/";
        public override int Priority { get; } = 5;

        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return (a.Value / b.Value).ToValuable();
        }
    }
    public class FloorDivision : Operator
    {
        public override string Symbol { get; } = "//";
        public override int Priority { get; } = 5;

        protected override Valuable Calc(Valuable a, Valuable b)
        {
            double? returnVal = (a.Value / b.Value);
            return (returnVal == null) ? null : Math.Floor((double)returnVal).ToValuable();
        }
    }
    public class Modulus : Operator
    {
        public override string Symbol { get; } = "%";
        public override int Priority { get; } = 5;

        protected override Valuable Calc(Valuable a, Valuable b)
        {
            return (a.Value % b.Value).ToValuable();
        }
    }

    // Priority 6    L->R
    public class Neg : Modifier
    {
        public override string Symbol { get; } = "-";
        public override int Priority { get; } = 6;

        protected override Valuable Calc(Valuable v)
        {
            return ElementUtils.Neg(v);
        }
    }
    public class Pos : Modifier
    {
        public override string Symbol { get; } = "+";
        public override int Priority { get; } = 6;

        protected override Valuable Calc(Valuable v)
        {
            return ElementUtils.Pos(v);
        }
    }

    // Priority 7    R->L
    public class Exponentiation : Operator
    {
        public override string Symbol { get; } = "^";
        public override int Priority { get; } = 7;
        public override Direction Direction { get; } = Direction.RightToLeft;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            if (a.Value == null | b.Value == null)
            {
                return ElementUtils.GetValuableNull();
            }

            return Math.Pow((double)a.Value, (double)b.Value).ToValuable();
        }
        protected override bool toStringPadding { get; } = false;
    }

    // Priority 8
    public class Deref : Modifier
    {
        public override string Symbol { get; } = "@";
        public override int Priority { get; } = 8;
        protected override Valuable Calc(Valuable v)
        {
            if (v is Defined)
            {
                return ((Defined)v).Definition;
            }
            else
            {
                throw new MessageException($"{v} is not definable.");
            }
        }
    }

    // Priority 9 L->R
    public class NullCoalesecer : Operator
    {
        public override string Symbol { get; } = "??";
        public override int Priority { get; } = 9;
        protected override Valuable Calc(Valuable a, Valuable b)
        {
            double? a_Value = a.Value;
            if (a_Value == null)
            {
                return b;
            }
            else
            {
                return a_Value.ToValuable();
            }
        }
    }

    // Priority 10
    public class Not : Modifier
    {
        public override string Symbol { get; } = "!";
        public override int Priority { get; } = 10;

        // bool = !(b == a)
        protected override Valuable Calc(Valuable v)
        {
            return ElementUtils.Not(v.Value).ToValuable();
        }
    }
    public class IncrementBefore : Modifier
    {
        public override string Symbol { get; } = "++";
        public override int Priority { get; } = 10;
        public virtual new Orientation Orientation { get; } = Orientation.Before;
        protected override Valuable Calc(Valuable v)
        {
            if (v is Setable)
            {
                ((Setable)v).Value = v.Value + 1;
                return v;
            }
            else
            {
                return (v.Value + 1).ToValuable();
            }
        }
    }
    public class DecrementBefore : Modifier
    {
        public override string Symbol { get; } = "--";
        public override int Priority { get; } = 10;
        public virtual new Orientation Orientation { get; } = Orientation.Before;

        protected override Valuable Calc(Valuable v)
        {
            if (v is Setable)
            {
                ((Setable)v).Value = v.Value - 1;
                return v;
            }
            else
            {
                return (v.Value - 1).ToValuable();
            }
        }
    }

    // After-Oriented Modifiers (infinite priority no matter what you set priority to)
    public class IncrementAfter : Modifier
    {
        public override string Symbol { get; } = "++";
        public override int Priority { get; } = 8;
        public override Orientation Orientation { get; } = Orientation.After;

        protected override Valuable Calc(Valuable v)
        {
            if (v is Setable)
            {
                double? val = v.Value;
                ((Setable)v).Value = v.Value + 1;
                return val.ToValuable();
            }
            else
            {
                return v;
            }
        }
    }
    public class DecrementAfter : Modifier
    {
        public override string Symbol { get; } = "--";
        public override int Priority { get; } = 8;
        public override Orientation Orientation { get; } = Orientation.After;

        protected override Valuable Calc(Valuable v)
        {
            if (v is Setable)
            {
                double? val = v.Value;
                ((Setable)v).Value = v.Value - 1;
                return val.ToValuable();
            }
            else
            {
                return v;
            }
        }
    }
    public class Factorial : Modifier
    {
        public override string Symbol { get; } = "!";
        public override int Priority { get; } = 8;
        public override Orientation Orientation { get; } = Orientation.After;

        protected override Valuable Calc(Valuable v)
        {
            if (v == null) { return null; }
            double? v_Value = v.Value;
            if (v_Value == null) { return ElementUtils.GetValuableNull(); }

            BigInteger v_long = new BigInteger((double)v_Value);
            if (v_Value < 0 || v_Value != Math.Floor((double)v_long))
            {
                throw new MessageException($"The factorial modifier can only be used with positive integers.");
            }

            BigInteger fact = v_long;
            v_long--;
            for (; v_long > 1; v_long--)
            {
                fact *= v_long;
            }
            return new constant((double) fact);
        }
    }
    #endregion
}