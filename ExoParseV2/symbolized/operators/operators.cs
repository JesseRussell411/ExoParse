using ConvenienceTools;
using ExoParseV2.elements;
using System;
using System.Collections.Generic;
using System.Text;
using IntegerFloatingPoint;

namespace ExoParseV2
{
    #region mathematical operations
    public class Addition_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "+";

        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() + b.Execute()).ToElement();
        }
    }
    public class Subtraction_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "-";

        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() - b.Execute()).ToElement();
        }
    }
    public class Multiplication_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "*";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() * b.Execute()).ToElement();
        }
    }
    public class Division_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "/";
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.Div(a.Execute(), b.Execute()).ToElement();
            //return (a.Execute() / b.Execute()).ToElement();
        }
    }
    public class FloorDivision_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "//";
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.FloorDiv(a.Execute(), b.Execute()).ToElement();
        }
    }

    public class Modulus_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "%";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() % b.Execute()).ToElement();
        }
    }

    public class Exponentiation_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "^";
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.Power(a.Execute(), b.Execute()).ToElement();
        }
        public override string ToString()
        {
            return Symbol;
        }
    }
    #endregion

    #region boolean logic
    public class And_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "&";

        protected override IElement calc(IElement a, IElement b)
        {
            return LogicUtils.And(a.Execute(), b.Execute()).ToElement();
        }
    }
    public class Or_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "|";

        protected override IElement calc(IElement a, IElement b)
        {
            return LogicUtils.Or(a.Execute(), b.Execute()).ToElement();
        }
    }
    public class Xor_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "^^"; // "^" was taken by Exponentiation_op

        protected override IElement calc(IElement a, IElement b)
        {
            return LogicUtils.And(a.Execute(), b.Execute()).ToElement();
        }
    }
    #region conditional
    public class ConditionalAnd_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "&&";

        protected override IElement calc(IElement a, IElement b)
        {
            bool? a_bool = a.Execute().ToBool();
            if (a_bool == null)
            {
                return IElement.Null;
            }
            else if (a_bool == true)
            {
                return (a_bool & b.Execute().ToBool()).ToElement();
            }
            else
            {
                return true.ToElement();
            }
        }
    }
    public class ConditionalOr_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "||";
        protected override IElement calc(IElement a, IElement b)
        {
            bool? a_bool = a.Execute().ToBool();
            if (a_bool == null)
            {
                return IElement.Null;
            }
            else if (a_bool == false)
            {
                return (a_bool | b.Execute().ToBool()).ToElement();
            }
            else
            {
                return true.ToElement();
            }
        }
    }
    #endregion
    #endregion

    #region comparison
    public class CheckEqual_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "==";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() == b.Execute()).ToElement();
        }
    }

    public class CheckNotEqual_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "!=";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() != b.Execute()).ToElement();
        }
    }

    public class GreaterThan_op : LeftToRightOperator
    {
        public override string Symbol { get; } = ">";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() > b.Execute()).ToElement();
        }
    }
    public class LessThan_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "<";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() < b.Execute()).ToElement();
        }
    }
    public class GreaterThanEqualTo_op : LeftToRightOperator
    {
        public override string Symbol { get; } = ">=";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() >= b.Execute()).ToElement();
        }
    }
    public class LessThanEqualTo_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "<=";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() <= b.Execute()).ToElement();
        }
    }
    #endregion

    #region mutation and modification
    public class SetEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, other => other.Execute().ToElement());
        }
    }

    public class PlusEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "+=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() + other.Execute()).ToElement());
        }
    }
    public class MinusEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "-=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() - other.Execute()).ToElement());
        }
    }
    public class TimesEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "*=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() * other.Execute()).ToElement());
        }
    }
    public class DivEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "/=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => MathUtils.Div(self.Execute(), other.Execute()).ToElement());
        }
    }
    public class FloorDivsEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "//=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => MathUtils.FloorDiv(self.Execute(), other.Execute()).ToElement());
        }
    }
    public class PowerEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "^=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => MathUtils.Power(self.Execute(), other.Execute()).ToElement());
        }
    }
    public class ModEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "%=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() % other.Execute()).ToElement());
        }
    }
    #region logic equals
    public class AndEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "&=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => LogicUtils.And(a.Execute(), b.Execute()).ToElement());
        }
    }
    public class OrEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "|=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => LogicUtils.Or(a.Execute(), b.Execute()).ToElement());
        }
    }
    public class XorEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "^^="; // ^= was taken by PowerEqual_op
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => LogicUtils.Xor(a.Execute(), b.Execute()).ToElement());
        }
    }
    #endregion
    #endregion

    #region definition
    public class SetDefinition_op : RightToLeftOperator
    {
        public override string Symbol { get; } = ":=";
        public override bool DontExecute_flag(IElement a, IElement b, Operation parent) => true;
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return calc(a, b);
        }
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b);
        }
        protected override IntFloat? execute(IElement a, IElement b, Operation parent)
        {
            calc(a, b);
            return null;
        }
    }

    //// Not really needed anymore now that .Pass(), .Calc(), and .Execute() are better defined. the dereference operator ($) can actually return a variable's definition through pass now, so you can just use := and $ like this b := $a and it will actually work.
    //public class SetAsDefinition_op : RightToLeftOperator
    //{
    //    public override string Symbol { get; } = ":=$";

    //    public override bool DontExecute_flag(IElement a, IElement b, Operation parent) => true;
    //    protected override IElement calc(IElement a, IElement b)
    //    {
    //        return a.TrySetDefinition(b.Definition);
    //    }
    //    protected override IElement pass(IElement a, IElement b, Operation parent)
    //    {
    //        return calc(a, b);
    //    }
    //}
    #endregion

    #region misc
    public class NullCoalescing_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "??";
        protected override IElement calc(IElement a, IElement b)
        {
            IntFloat? a_Execute = a.Execute();
            if (a_Execute == null)
            {
                return b;
            }
            else
            {
                return a_Execute.ToElement();
            }
        }
    }
    #region program flow
    #region ternary
    public class Ternary_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "?";
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return parent;
        }
        protected override IElement calc(IElement a, IElement b)
        {
            if (b is TernaryMessenger tm)
            {
                IntFloat? a_Execute = a.Execute();
                if (a_Execute == LogicUtils.True_IntFloat)
                {
                    return tm.A;
                }
                else if (a_Execute == LogicUtils.False_IntFloat)
                {
                    return tm.B;
                }
                else
                {
                    return ElementUtils.NullElement;
                }
            }
            else
            {
                IntFloat? a_Execute = a.Execute();
                if (a_Execute == LogicUtils.True_IntFloat)
                {
                    return b;
                }
                else
                {
                    return ElementUtils.NullElement;
                }
            }
        }
    }

    public class TernarySeperator_op : RightToLeftOperator
    {
        public override string Symbol { get; } = ":";
        protected override IElement calc(IElement a, IElement b)
        {
            return new TernaryMessenger(this, a, b);
        }
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return calc(a, b);
        }
    }
    #endregion
    public class Semicolon_op : LeftToRightOperator
    {
        public override string Symbol { get; } = ";";
        public override bool DontExecute_flag(IElement a, IElement b, Operation parent)
        {
            return b?.DontExecute_flag ?? false;
        }

        protected override IElement calc(IElement a, IElement b)
        {
            if (!a.DontExecute_flag) { a.Execute(); }
            return b.Calc();
        }
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return parent;
        }
        public override string ToString()
        {
            return $"{Symbol} ";
        }
    }
    #endregion
#endregion

}
