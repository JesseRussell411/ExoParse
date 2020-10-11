using ConvenienceTools;
using ExoParseV2.elements;
using System;
using System.Collections.Generic;
using System.Text;

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
            return (a.Execute() / b.Execute()).ToElement();
        }
    }
    public class FloorDivision_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "//";
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.Floor(a.Execute() / b.Execute()).ToElement();
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
        public override bool ToStringPadding { get; } = false;
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.Power(a.Execute(), b.Execute()).ToElement();
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
            return a.TrySetDefinition(b, (self, other) => (self.Execute() / other.Execute()).ToElement());
        }
    }
    public class FloorDivsEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "//=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => MathUtils.Floor(self.Execute() / other.Execute()).ToElement());
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
        public override bool dontExecute_flag(IElement a, IElement b, Operation parent) => true;
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b);
        }
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return calc(a, b);
        }
    }
    public class SetAsDefinition_op : RightToLeftOperator
    {
        public override string Symbol { get; } = ":=$";

        public override bool dontExecute_flag(IElement a, IElement b, Operation parent) => true;
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b.Definition);
        }
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return calc(a, b);
        }
    }
    #endregion

    #region misc
    public class NullCoalescing_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "??";
        protected override IElement calc(IElement a, IElement b)
        {
            if (a.Execute() == null)
            {
                return b;
            }
            else
            {
                return a;
            }
        }
    }
    #region ternary
    public class Ternary_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "?";
        protected override IElement calc(IElement a, IElement b)
        {
            if (b is TernaryMessenger tm)
            {
                double? a_Execute = a.Execute();
                if (a_Execute == LogicUtils.True_double)
                {
                    var result =  tm.A?.Pass();
                    return result;
                }
                else if (a_Execute == LogicUtils.False_double)
                {
                    return tm.B?.Pass();
                }
                else
                {
                    return ElementUtils.NullElement;
                }
            }
            else
            {
                double? a_Execute = a.Execute();
                if (a_Execute == LogicUtils.True_double)
                {
                    return b.Pass();
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
        public override bool ToStringPadding { get; } = false;
        public override string Symbol { get; } = ";";
        public override bool dontExecute_flag(IElement a, IElement b, Operation parent)
        {
            return b?.DontExecute_flag ?? false;
        }

        protected override IElement calc(IElement a, IElement b)
        {
            var p = a.Pass(out bool dontExecute_flag);
            if (!dontExecute_flag) { a.Execute(); }
            return b.Pass();
        }
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return calc(a, b);
        }
        public override string ToString()
        {
            return $"{Symbol} ";
        }
    }
    #endregion

}
