﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.Functions
{
    #region Trigonometry
    public class Sin_func : BuiltInFunction
    {
        public override string Name { get; } = "sin";
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            double? arg_execute = args[0].Execute();
            if (arg_execute == null)
            {
                return ElementUtils.NullElement;
            }
            else
            {
                return Math.Sin((double)arg_execute).ToElement();
            }
        }
    }

    public class Cos_func : BuiltInFunction
    {
        public override string Name { get; } = "cos";
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            double? arg_execute = args[0].Execute();
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
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            double? arg_execute = args[0].Execute();
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
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            double? arg_execute = args[0].Execute();
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
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            double? arg_execute = args[0].Execute();
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
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            double? arg_execute = args[0].Execute();
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
        public override string[] Arguments { get; } = { "r" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.ToDegrees(args[0].Execute()).ToElement();
        }
    }

    public class ToRadians_func : BuiltInFunction
    {
        public override string Name { get; } = "rad";
        public override string[] Arguments { get; } = { "d" };
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
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Sign(args[0].Execute()).ToElement();
        }
    }

    public class AbsoluteValue_func : BuiltInFunction
    {
        public override string Name { get; } = "abs";
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Abs(args[0].Execute()).ToElement();
        }
    }

    public class Floor_func : BuiltInFunction
    {
        public override string Name { get; } = "floor";
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Floor(args[0].Execute()).ToElement();
        }
    }

    public class Ceiling_func : BuiltInFunction
    {
        public override string Name { get; } = "ceil";
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Ceiling(args[0].Execute()).ToElement();
        }
    }
    #endregion
    #region misc
    public class Min_func : BuiltInFunction
    {
        public override string Name { get; } = "min";
        public override string[] Arguments { get; } = { "a", "b" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Min(args[0].Execute(), args[1].Execute()).ToElement();
        }
    }
    public class Max_func : BuiltInFunction
    {
        public override string Name { get; } = "max";
        public override string[] Arguments { get; } = { "a", "b" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Max(args[0].Execute(), args[1].Execute()).ToElement();
        }
    }
    public class Log_func : BuiltInFunction
    {
        public override string Name { get; } = "log";
        public override string[] Arguments { get; } = { "base", "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Log(args[1].Execute(), args[0].Execute()).ToElement();
        }
    }
    public class Log10_func : BuiltInFunction
    {
        public override string Name { get; } = "log";
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Log10(args[0].Execute()).ToElement();
        }
    }
    public class NaturalLog_func : BuiltInFunction
    {
        public override string Name { get; } = "ln";
        public override string[] Arguments { get; } = { "x" };
        protected override IElement calc(IElement[] args)
        {
            return MathUtils.Ln(args[0].Execute()).ToElement();
        }
    }
    #endregion

}