using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.elements;
using ExoParseV2.theUniverse;
using IntegerFloatingPoint;

namespace ExoParseV2.Functions
{
    #region Trigonometry
    public class Sin_func : BuiltInFunction
    {
        public override string Name { get; } = "sin";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloat? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return ((IntFloat)Math.Sin((double)arg_execute)).ToElement();
            }
        }
    }

    public class Cos_func : BuiltInFunction
    {
        public override string Name { get; } = "cos";
        public override string[] Parameters { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            IntFloat? arg_execute = args[0].Execute();
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
            IntFloat? arg_execute = args[0].Execute();
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
            IntFloat? arg_execute = args[0].Execute();
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
            IntFloat? arg_execute = args[0].Execute();
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
            IntFloat? arg_execute = args[0].Execute();
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
            IntFloat? args1_execute = args[1].Execute();
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
            IElement result = IElement.Void;

            while (condition.Execute().ToBool() == true)
            {
                result = expression.Calc();
            }

            return result;
        }
    }
    public class DoWhile_func : BuiltInFunction
    {
        public override string Name { get; } = "doWhile";
        public override string[] Parameters { get; } = { "condition", "expression" };
        protected override IElement calc(IElement[] args)
        {
            IElement condition = args[0].Definition;
            IElement expression = args[1].Definition;
            IElement result = IElement.Void;

            do
            {
                result = expression.Calc();
            } while (condition.Execute().ToBool() == true);

            return result;
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

            IElement result = IElement.Void;

            for (assignment.Execute(); condition.Execute().ToBool() == true; iteration.Execute())
            {
                result = expression.Calc();
            }

            return result;
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
            double? minValue = args[0].Execute()?.Float;
            double? maxValue = args[1].Execute()?.Float;
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
    #endregion

    #region IntFloat
    public class ToFloat_func : BuiltInFunction
    {
        public override string Name { get; } = "float";
        public override string[] Parameters { get; } = { "value" };
        protected override IElement calc(IElement[] args)
        {
            return args[0].Execute()?.Float.ToElement();
        }
    }
    public class ToInt_func : BuiltInFunction
    {
        public override string Name { get; } = "int";
        public override string[] Parameters { get; } = { "value" };
        protected override IElement calc(IElement[] args)
        {
            return args[0].Execute()?.Int.ToElement();
        }
    }
    public class IsFloat_func : BuiltInFunction
    {
        public override string Name { get; } = "isFloat";
        public override string[] Parameters { get; } = { "value" };
        protected override IElement calc(IElement[] args)
        {
            return args[0].Execute()?.IsFloat.ToElement();
        }
    }
    public class IsInt_func : BuiltInFunction
    {
        public override string Name { get; } = "isInt";
        public override string[] Parameters { get; } = { "value" };
        protected override IElement calc(IElement[] args)
        {
            return args[0].Execute()?.IsInt.ToElement();
        }
    }
    #endregion

}
