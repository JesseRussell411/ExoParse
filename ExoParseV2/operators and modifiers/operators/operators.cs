using ExoParseV2.elements;
using JesseRussell.Numerics;

namespace ExoParseV2
{
    #region mathematical operations
    public class Addition_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "+";
        public override string Definition => "Adds the two elements and returns the result.";

        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() + b.Execute()).ToElement();
        }
    }
    public class Subtraction_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "-";
        public override string Definition => "Subtracts the two elements and returns the result.";

        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() - b.Execute()).ToElement();
        }
    }
    public class Multiplication_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "*";
        public override string Definition => "Multiplies the two elements and returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() * b.Execute()).ToElement();
        }
    }
    public class Division_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "/";
        public override string Definition => "Divides the two elements and returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.Div(a.Execute(), b.Execute()).ToElement();
            //return (a.Execute() / b.Execute()).ToElement();
        }
    }
    public class FloorDivision_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "//";
        public override string Definition => "Divides the two elements and returns the floor of he result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.FloorDiv(a.Execute(), b.Execute()).ToElement();
        }
    }
    public class FloatDivision_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "///";
        public override string Definition => "Adds the two elements and returns the result as floating point even if the result is an integer or fraction.";
        protected override IElement calc(IElement a, IElement b)
        {
            return MathUtils.FloatDiv(a.Execute(), b.Execute()).ToElement();
        }
    }

    public class Modulus_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "%";
        public override string Definition => "Returns the modulus of the two elements.";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() % b.Execute()).ToElement();
        }
    }

    public class Exponentiation_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "^";
        public override string Definition => "Returns the left element raised to the power of the right element.";
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
        public override string Definition => "Performs logical and on the two elements and returns the results. The elements must be either 0 or 1, otherwise, null is returned.";

        protected override IElement calc(IElement a, IElement b)
        {
            return LogicUtils.And(a.Execute(), b.Execute()).ToElement();
        }
    }
    public class Or_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "|";
        public override string Definition => "Performs logical or on the two elements and returns the results. The elements must be either 0 or 1, otherwise, null is returned.";

        protected override IElement calc(IElement a, IElement b)
        {
            return LogicUtils.Or(a.Execute(), b.Execute()).ToElement();
        }
    }
    public class Xor_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "^^"; // "^" was taken by Exponentiation_op
        public override string Definition => "Performs logical Xor on the two elements and returns the results. The elements must be either 0 or 1, otherwise, null is returned.";

        protected override IElement calc(IElement a, IElement b)
        {
            return LogicUtils.Xor(a.Execute(), b.Execute()).ToElement();
        }
    }
    #region conditional
    public class ConditionalAnd_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "&&";
        public override string Definition => "Performs conditional and on the two elements and returns the results. If the first element evaluates to false, the second element is not evaluated. The elements must be either 0 or 1, otherwise, null is returned.";

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
                return false.ToElement();
            }
        }
    }
    public class ConditionalOr_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "||";
        public override string Definition => "Performs conditional or on the two elements and returns the results. If the first element evaluates to true, the second element is not evaluated. The elements must be either 0 or 1, otherwise, null is returned.";
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
        public override string Definition => "Returns true if the elements are equal.";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() == b.Execute()).ToElement();
        }
    }

    public class CheckNotEqual_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "!=";
        public override string Definition => "Returns true if the elements are not equal.";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() != b.Execute()).ToElement();
        }
    }

    public class GreaterThan_op : LeftToRightOperator
    {
        public override string Symbol { get; } = ">";
        public override string Definition => "Returns true if the left element is greater than the right element.";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() > b.Execute()).ToElement();
        }
    }
    public class LessThan_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "<";
        public override string Definition => "Returns true if the left element is less than the right element.";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() < b.Execute()).ToElement();
        }
    }
    public class GreaterThanEqualTo_op : LeftToRightOperator
    {
        public override string Symbol { get; } = ">=";
        public override string Definition => "Returns true if the left element is greater than or equal to the right element.";
        protected override IElement calc(IElement a, IElement b)
        {
            return (a.Execute() >= b.Execute()).ToElement();
        }
    }
    public class LessThanEqualTo_op : LeftToRightOperator
    {
        public override string Symbol { get; } = "<=";
        public override string Definition => "Returns true if the left element is less than or equal to the right element.";
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
        public override string Definition => "Sets the left element equal to the value of the right. Returns the value.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, other => other.Execute().ToElement());
        }
    }

    public class PlusEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "+=";
        public override string Definition => "Adds the value of the right element to the left element. Returns the result";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() + other.Execute()).ToElement());
        }
    }
    public class MinusEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "-=";
        public override string Definition => "Subtracts the value of the right element from the left element. Returns the result";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() - other.Execute()).ToElement());
        }
    }
    public class TimesEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "*=";
        public override string Definition => "Sets the left element to itself times the right element. Returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() * other.Execute()).ToElement());
        }
    }
    public class DivEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "/=";
        public override string Definition => "Sets the left element to itself divided by the right element. Returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => MathUtils.Div(self.Execute(), other.Execute()).ToElement());
        }
    }
    public class FloorDivsEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "//=";
        public override string Definition => "Sets the left element to the floor of itself divided by the right element. Returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => MathUtils.FloorDiv(self.Execute(), other.Execute()).ToElement());
        }
    }
    public class PowerEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "^=";
        public override string Definition => "Sets the left element to itself raises the the right. Returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => MathUtils.Power(self.Execute(), other.Execute()).ToElement());
        }
    }
    public class ModEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "%=";
        public override string Definition => "Sets the left element to the modulus of itself and the right element. Returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => (self.Execute() % other.Execute()).ToElement());
        }
    }
    #region logic equals
    public class AndEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "&=";
        public override string Definition => "Sets the left element to the logical and of itself and the right element. Returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => LogicUtils.And(a.Execute(), b.Execute()).ToElement());
        }
    }
    public class OrEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "|=";
        public override string Definition => "Sets the left element to the logical or of itself and the right element. Returns the result.";
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b, (self, other) => LogicUtils.Or(a.Execute(), b.Execute()).ToElement());
        }
    }
    public class XorEqual_op : RightToLeftOperator
    {
        public override string Symbol { get; } = "^^="; // ^= was taken by PowerEqual_op
        public override string Definition => "Sets the left element to the logical Xor of itself and the right element. Returns the result.";
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
        public override string Definition => "Sets the left element's definition to the right element (value or expression). Returns the resulting definition. Can be chained.";
        public override bool DontExecute_flag(IElement a, IElement b, Operation parent) => true;
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return calc(a, b);
        }
        protected override IElement calc(IElement a, IElement b)
        {
            return a.TrySetDefinition(b);
        }
        protected override IntFloatFrac? execute(IElement a, IElement b, Operation parent)
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
        public override string Definition => "Evaluates the left element and returns the result or the right element if the result was null.";
        protected override IElement calc(IElement a, IElement b)
        {
            IntFloatFrac? a_Execute = a.Execute();
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
        public override string Definition => "Evaluates the left element and returns the appropriate selection from the right, first for true, second for false.";
        protected override IElement pass(IElement a, IElement b, Operation parent)
        {
            return parent;
        }
        protected override IElement calc(IElement a, IElement b)
        {
            if (b is TernaryMessenger tm)
            {
                IntFloatFrac? a_Execute = a.Execute();
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
                IntFloatFrac? a_Execute = a.Execute();
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
        public override string Definition => "Used exclusively with ?. The element on the left is returned if the element before ? is true. Otherwise the element on the right is returned.";
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
        public override string Definition => "Evaluates the left element and throws away the result, then returns the right element.";
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
