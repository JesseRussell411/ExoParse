using System;
using System.Numerics;
using ExoParseV2.elements;
using ExoParseV2.theUniverse;
using JesseRussell.Numerics;

namespace ExoParseV2.Functions
{
    #region Trigonometry
    public class Sin_func : BuiltInFunction
    {
        public override string Name { get; } = "sin";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return ((IntFloatFrac)Math.Sin((double)arg_execute)).ToElement();
            }
        }
    }

    public class Cos_func : BuiltInFunction
    {
        public override string Name { get; } = "cos";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return Math.Cos((double)arg_execute).ToElement();
            }
        }
    }

    public class Tan_func : BuiltInFunction
    {
        public override string Name { get; } = "tan";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return Math.Tan((double)arg_execute).ToElement();
            }
        }
    }

    public class ArcSin_func : BuiltInFunction
    {
        public override string Name { get; } = "asin";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return Math.Asin((double)arg_execute).ToElement();
            }
        }
    }

    public class ArcCos_func : BuiltInFunction
    {
        public override string Name { get; } = "acos";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return Math.Acos((double)arg_execute).ToElement();
            }
        }
    }

    public class ArcTan_func : BuiltInFunction
    {
        public override string Name { get; } = "atan";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return Math.Atan((double)arg_execute).ToElement();
            }
        }
    }

    public class ToDegrees_func : BuiltInFunction
    {
        public override string Name { get; } = "deg";
        public override string[] Parameters { get; } = { "r" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.ToDegrees(args[0].Execute()).ToElement();
        }
    }

    public class ToRadians_func : BuiltInFunction
    {
        public override string Name { get; } = "rad";
        public override string[] Parameters { get; } = { "d" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.ToRadians(args[0].Execute()).ToElement();
        }
    }
    #endregion

    #region NumberMods
    public class Sign_func : BuiltInFunction
    {
        public override string Name { get; } = "sign";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Sign(args[0].Execute()).ToElement();
        }
    }

    public class AbsoluteValue_func : BuiltInFunction
    {
        public override string Name { get; } = "abs";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Abs(args[0].Execute()).ToElement();
        }
    }

    public class Floor_func : BuiltInFunction
    {
        public override string Name { get; } = "floor";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Floor(args[0].Execute()).ToElement();
        }
    }

    public class Ceiling_func : BuiltInFunction
    {
        public override string Name { get; } = "ceil";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Ceiling(args[0].Execute()).ToElement();
        }
    }

    public class Truncate_func : BuiltInFunction
    {
        public override string Name { get; } = "truncate";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Truncate(args[0].Execute()).ToElement();
        }
    }

    public class Round_func : BuiltInFunction
    {
        public override string Name { get; } = "round";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Round(args[0].Execute()).ToElement();
        }
    }

    public class Round2_func : BuiltInFunction
    {
        public override string Name { get; } = "round";
        public override string[] Parameters { get; } = { "x", "decimals" };
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? args1_execute = args[1].Execute();
            if (args1_execute != MathUtils.Floor(args1_execute))
            {
                throw new MessageException($"Incorrect ussage of {this}. Decimals must be an integer");
            }
            return MathUtils.Round(args[0].Execute(), args[1].Execute()).ToElement();
        }
    }
    #endregion

    #region misc
    public class Min_func : BuiltInFunction
    {
        public override string Name { get; } = "min";
        public override string[] Parameters { get; } = { "a", "b" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Min(args[0].Execute(), args[1].Execute()).ToElement();
        }
    }
    public class Max_func : BuiltInFunction
    {
        public override string Name { get; } = "max";
        public override string[] Parameters { get; } = { "a", "b" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Max(args[0].Execute(), args[1].Execute()).ToElement();
        }
    }
    public class Log_func : BuiltInFunction
    {
        public override string Name { get; } = "log";
        public override string[] Parameters { get; } = { "base", "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Log(args[1].Execute(), args[0].Execute()).ToElement();
        }
    }
    public class Log10_func : BuiltInFunction
    {
        public override string Name { get; } = "log";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Log10(args[0].Execute()).ToElement();
        }
    }
    public class NaturalLog_func : BuiltInFunction
    {
        public override string Name { get; } = "ln";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Ln(args[0].Execute()).ToElement();
        }
    }
    public class While_func : BuiltInFunction
    {
        public override string Name { get; } = "while";
        public override string[] Parameters { get; } = { "condition", "expression" };
        protected override IElement calc(IElement[] args)
        {
            IElement condition = args[0].Definition;
            IElement expression = args[1].Definition;
            IntFloatFrac? result = null;

            while (condition.Execute().ToBool() == true)
            {
                result = expression.Execute();
            }

            return result.ToElement();
        }
    }
    public class DoWhile_func : BuiltInFunction
    {
        public override string Name { get; } = "doWhile";
        public override string[] Parameters { get; } = { "expression", "condition" };
        protected override IElement calc(IElement[] args)
        {
            IElement expression = args[0].Definition;
            IElement condition = args[1].Definition;
            IntFloatFrac? result;

            do
            {
                result = expression.Execute();
            } while (condition.Execute().ToBool() == true);

            return result.ToElement();
        }
    }
    public class For_func : BuiltInFunction
    {
        public override string Name { get; } = "for";
        public override string[] Parameters { get; } = { "assignment", "condition", "iteration", "expression" };
        protected override IElement calc(IElement[] args)
        {
            IElement assignment = args[0].Definition;
            IElement condition  = args[1].Definition;
            IElement iteration  = args[2].Definition;
            IElement expression = args[3].Definition;

            IntFloatFrac? result = null;

            for (assignment.Execute(); condition.Execute().ToBool() == true; iteration.Execute())
            {
                result = expression.Execute();
            }

            return result.ToElement();
        }
    }

    public class If_func : BuiltInFunction
    {
        public override string Name { get; } = "if";
        public override string[] Parameters { get; } = { "condition", "expression" };
        protected override IElement calc(IElement[] args)
        {
            IElement condition = args[0].Definition;
            IElement expression = args[1].Definition;

            IntFloatFrac? result = null;
            if (condition.Execute().ToBool() == true)
            {
                result = expression.Execute();
            }
            return result.ToElement();
        }
    }
    public class IfElse_func : BuiltInFunction
    {
        public override string Name { get; } = "if";
        public override string[] Parameters { get; } = { "condition", "expression", "else_expression" };
        protected override IElement calc(IElement[] args)
        {
            IElement condition = args[0].Definition;
            IElement expression = args[1].Definition;
            IElement else_expression = args[2].Definition;

            IntFloatFrac? result = null;

            if (condition.Execute().ToBool() == true)
            {
                result = expression.Execute();
            }
            else
            {
                result = else_expression.Execute();
            }
            return result.ToElement();
        }
    }

    public class Print_func : BuiltInFunction
    {
        public override string Name { get; } = "print";
        public override string[] Parameters { get; } = { "n" };
        public Universe Universe { get; set; }
        protected override IElement calc(IElement[] args)
        {
            IElement arg0_def = args[0].Definition;
            Universe.PrintFunction($"{arg0_def}\n");
            return arg0_def;
        }
    }

    public class PrintLine_func : BuiltInFunction
    {
        public override string Name { get; } = "print";
        public override string[] Parameters { get; } = { };
        public Universe Universe { get; set; }
        protected override IElement calc(IElement[] args)
        {
            Universe.PrintFunction("\n");
            return IElement.Void;
        }
    }
    public class PrintValue_func : BuiltInFunction
    {
        public override string Name { get; } = "printValue";
        public override string[] Parameters { get; } = { "n" };
        public Universe Universe { get; set; }
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg0_ex = args[0].Execute();
            Universe.PrintFunction($"{arg0_ex}\n");
            return arg0_ex.ToElement();
        }
    }

    public class Exit_func : BuiltInFunction
    {
        public override string Name { get; } = "exit";
        public override string[] Parameters { get; } = { };
        public Universe Universe { get; set; }
        protected override IElement calc(IElement[] args)
        {
            Environment.Exit(0);
            return IElement.Void;
        }
    }

    public class ExitWithCode_func : BuiltInFunction
    {
        public override string Name { get; } = "exit";
        public override string[] Parameters { get; } = { "exitCode" };
        public Universe Universe { get; set; }
        protected override IElement calc(IElement[] args)
        {
            int? exitCode = (int?)args[0].Execute();
            Environment.Exit(exitCode ?? 0);
            return IElement.Void;
        }
    }

    public class RandomRange_float_func : BuiltInFunction
    {
        public override string Name { get; } = "randomRange_float";
        public override string[] Parameters { get; } = { "minValue", "maxValue" };
        public Universe Universe { get; set; }
        private static Random rand = new Random();
        protected override IElement calc(IElement[] args)
        {
            double? minValue = (double?)args[0].Execute();
            double? maxValue = (double?)args[1].Execute();
            if (minValue != null && maxValue != null)
            {
                if (minValue > maxValue) { throw new ExecutionException("minValue cannot be greater than maxValue."); }
                return ((rand.NextDouble() * (maxValue - minValue)) + minValue).ToElement();
            }
            else
            {
                return IElement.Null;
            }
        }
    }

    public class GCD_func: BuiltInFunction
    {
        public override string Name { get; } = "gcd";
        public override string[] Parameters { get; } = { "a", "b" };
        public Universe Universe { get; set; }
        private static Random rand = new Random();
        protected override IElement calc(IElement[] args)
        {
            var a_nullable = args[0].Execute();
            var b_nullable = args[1].Execute();

            if (a_nullable == null) throw new ExecutionException("a was null in the gcd function.");
            if (b_nullable == null) throw new ExecutionException("b was null in the gcd function.");

            var a = a_nullable.Value;
            var b = b_nullable.Value;

            if (!a.IsInt) throw new ExecutionException("a was not an int in the gcd function.");
            if (!b.IsInt) throw new ExecutionException("b was not an int in the gcd function.");

            return BigInteger.GreatestCommonDivisor(a.Int, b.Int).ToElement();
        }
    }
    #endregion

    #region IntFloatFrac
    public class ToFloat_func : BuiltInFunction
    {
        public override string Name { get; } = "float";
        public override string[] Parameters { get; } = { "value" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calculate(args);
        }
        protected override IElement calc(IElement[] args)
        {
            return new IntFloatFrac(new IntFloat(args[0].Execute()?.Float ?? default)).ToElement();
        }
    }
    public class ToInt_func : BuiltInFunction
    {
        public override string Name { get; } = "int";
        public override string[] Parameters { get; } = { "value" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calculate(args);
        }
        protected override IElement calc(IElement[] args)
        {
            return args[0].Execute()?.Int.ToElement();
        }
    }
    public class ToFraction_func : BuiltInFunction
    {
        public override string Name => "fraction";
        public override string[] Parameters { get; } = { "value" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calculate(args);
        }
        protected override IElement calc(IElement[] args)
        {
            return new IntFloatFrac(args[0].Execute()?.Fraction ?? default).ToElement();
        }
    }
        public class IsFloat_func : BuiltInFunction
    {
        public override string Name { get; } = "isFloat";
        public override string[] Parameters { get; } = { "value" };
        protected override IElement calc(IElement[] args)
        {
            return (args[0].Execute()?.IsFloat ?? false).ToElement();
        }
    }
    public class IsInt_func : BuiltInFunction
    {
        public override string Name { get; } = "isInt";
        public override string[] Parameters { get; } = { "value" };
        protected override IElement calc(IElement[] args)
        {
            return (args[0].Execute()?.IsInt ?? false).ToElement();
        }
    }
    public class IsFraction_func : BuiltInFunction
    {
        public override string Name { get; } = "isFraction";
        public override string[] Parameters { get; } = { "value" };
        protected override IElement calc(IElement[] args)
        {
            return (args[0].Execute()?.IsFraction ?? false).ToElement();
        }
    }
    #endregion

    #region Fraction
    public class Simplify_func : BuiltInFunction
    {
        public override string Name { get; } = "simp";
        public override string[] Parameters { get; } = { "fraction" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calc(parent, args);
        }
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg0_ex = args[0].Execute();
            if (arg0_ex == null) return null;

            return arg0_ex?.Fraction.Simplify().ToElement();
        }
    }

    public class GetNumerator_func : BuiltInFunction
    {
        public override string Name { get; } = "getNumerator";
        public override string[] Parameters { get; } = { "fraction" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calc(parent, args);
        }
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg0_ex = args[0].Execute();
            if (arg0_ex == null) return null;

            return arg0_ex?.Fraction.Numerator.ToElement();
        }
    }

    public class GetDenominator_func : BuiltInFunction
    {
        public override string Name { get; } = "getDenominator";
        public override string[] Parameters { get; } = { "fraction" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calc(parent, args);
        }
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg0_ex = args[0].Execute();
            if (arg0_ex == null) return null;

            return arg0_ex?.Fraction.Denominator.ToElement();
        }
    }

    public class NaiveSum_func : BuiltInFunction
    {
        public override string Name { get; } = "naiveSum";
        public override string[] Parameters { get; } = { "fraction_A", "fraction_B" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calc(parent, args);
        }
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? arg0_ex = args[0].Execute();
            if (arg0_ex == null) return null;
            IntFloatFrac? arg1_ex = args[1].Execute();
            if (arg1_ex == null) return null;
            Fraction a = arg0_ex.Value.Fraction;
            Fraction b = arg1_ex.Value.Fraction;

            return a.NaiveSum(b).ToElement();
        }
    }
    #endregion

    #region Doudec
    public class IsDouble : BuiltInFunction
    {
        public override string Name { get; } = "isDouble";
        public override string[] Parameters { get; } = { "n" };
        protected override IElement calc(IElement[] args) => 
            (args[0].Execute() is IntFloatFrac iff && iff.IsFloat && iff.Float.IsDouble).ToElement();
    }
    public class IsDecimal : BuiltInFunction
    {
        public override string Name { get; } = "isDecimal";
        public override string[] Parameters { get; } = { "n" };
        protected override IElement calc(IElement[] args) => 
            (args[0].Execute() is IntFloatFrac iff && iff.IsFloat && iff.Float.IsDecimal).ToElement();
    }
    public class ToDouble : BuiltInFunction
    {
        public override string Name { get; } = "double";
        public override string[] Parameters { get; } = { "n" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calculate(args);
        }
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? a0ex = args[0].Execute();
            if (a0ex == null) return new Literal(null);

            // *It's like a Russian nesting doll.
            return new Literal(new IntFloatFrac(new IntFloat(new Doudec(((IntFloatFrac)a0ex).Float.Double))));
        }
    }
    public class ToDecimal : BuiltInFunction
    {
        public override string Name { get; } = "decimal";
        public override string[] Parameters { get; } = { "n" };
        public override IElement Pass(Execution parent, IElement[] args)
        {
            return Calculate(args);
        }
        protected override IElement calc(IElement[] args)
        {
            IntFloatFrac? a0ex = args[0].Execute();
            if (a0ex == null) return new Literal(null);

            try
            {
                return new Literal(new IntFloatFrac(new IntFloat(new Doudec(((IntFloatFrac)a0ex).Float.Decimal))));
            }
            catch (OverflowException oe)
            {
                throw new ExecutionException($"The number could not be converted to decimal because: {oe.Message}");
            }
        }
    }
    #endregion
}
