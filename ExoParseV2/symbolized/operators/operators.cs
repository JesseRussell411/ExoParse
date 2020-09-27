using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
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
        public override string Symbol { get; } = "==";
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
            return (a.Execute() == b.Execute()).ToElement();
        }
    }
    public class LessThanEqualTo_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "<=";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() < b.Execute()).ToElement();
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
    #endregion

    #region definiton
    public class SetDefinition_op : RightToLeftOperator
    {
        public override string Symbol { get; } = ":=";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b);
        }
    }
    public class SetAsDefinition_op : RightToLeftOperator
    {
        public override string Symbol { get; } = ":=$";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b.Definition);
        }
    }
    #endregion

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
            throw new NotImplementedException();
        }
    }

    public class TernarySeperator_op : RightToLeftOperator
    {
        public override string Symbol { get; } = ":";
        protected override IElement calc(IElement a, IElement b)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
