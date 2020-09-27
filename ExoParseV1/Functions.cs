using System;
using System.Linq;

namespace ExoParsev1
{
    public abstract class Function : Element
    {
        public virtual bool NonReplaceable { get; set; } = false;
        public abstract string Name { get; }
        public virtual string[] Arguments { get; } = new string[0];
        public virtual int ArgCount
        {
            get
            {
                return Arguments.Length;
            }
        }
        public (string name, int argCount) Id
        {
            get
            {
                return ElementUtils.FuncId(Name, Arguments.Length);
            }
        }
        public Valuable Calculate(params Valuable[] arguments)
        {
            foreach (Valuable arg in arguments)
            {
                if (arg == null) { return null; } // Cancel and return null if any of the arguments are null (void).
            }
            if (arguments.Length != Arguments.Length) { return null; } // Cancel and return null if the number of arguemnts is wrong.
            return Calc(arguments);
        }
        //#region convenience calculate methods
        //public Valuable Calculate() { return Calculate(new Valuable[] { }); }
        //public Valuable Calculate(Valuable arg1) { return Calculate(new Valuable[] { arg1 }); }
        //public Valuable Calculate(Valuable arg1, Valuable arg2) { return Calculate(new Valuable[] { arg1, arg2 }); }
        //public Valuable Calculate(Valuable arg1, Valuable arg2, Valuable arg3) { return Calculate(new Valuable[] { arg1, arg2, arg3 }); }
        //public Valuable Calculate(Valuable arg1, Valuable arg2, Valuable arg3, Valuable arg4) { return Calculate(new Valuable[] { arg1, arg2, arg3, arg4 }); }
        //public Valuable Calculate(Valuable arg1, Valuable arg2, Valuable arg3, Valuable arg4, Valuable arg5) { return Calculate(new Valuable[] { arg1, arg2, arg3, arg4, arg5 }); }
        //#endregion
        protected abstract Valuable Calc(Valuable[] arguments);
        public override string ToString()
        {
            return $"{Name}({Arguments.ToDelimString(", ")})";
        }
    }

    #region functions
    public class CustomFunction : Function
    {
        public new bool NonReplaceable { get; private set; } = false;
        public override string Name { get; }
        public override string[] Arguments { get; }
        //public Valuable Behavior { get; }
        public Valuable[] Behavior { get; private set; }
        public CustomFunction(string name, Valuable behavior, Variable[] argLinks) : this(name, behavior.Ta(), argLinks) { }
        public CustomFunction(string name, Valuable[] behavior, Variable[] argLinks)
        {
            Name = name;
            Behavior = behavior;
            this.argLinks = argLinks;
            Arguments = argLinks.Select(al => al.Name).ToArray();
        }
        public CustomFunction(string name, Valuable behavior) : this(name, behavior, new Variable[0]) { }
        public CustomFunction(string name, Valuable[] behavior) : this(name, behavior, new Variable[0]) { }
        protected override Valuable Calc(Valuable[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                argLinks[i].Definition = arguments[i];
            }

            Valuable ret = null;
            foreach(Valuable val in Behavior)
            {
                ret = val.Execute(); 
            }

            return ret;
        }
        private Variable[] argLinks;
    }

    #region built in functions
    public abstract class BuiltInFunction : Function
    {
        public override bool NonReplaceable { get; set; } = true;
    }
    public class Ans : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "ans"; } }
        public Valuable Value { get; set; }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            return Value;
        }
        #endregion
    }
    public class Pi : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "pi"; } }
        #endregion

        #region hidden
        protected override Valuable Calc(Valuable[] arguments)
        {
            return Math.PI.ToValuable();
        }
        #endregion
    }
    public class E : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "e"; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            return Math.E.ToValuable();
        }
        #endregion
    }
    public class Sin : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "sin"; } }
        public override string[] Arguments { get { return new string[] { "x" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            if (arguments[0] == null) { return null; }
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null)
            {
                return ElementUtils.GetValuableNull();
            }
            else
            {
                return Math.Sin((double)arg0_value).ToValuable();
            }
        }
        #endregion
    }
    public class Cos : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "cos"; } }
        public override string[] Arguments { get { return new string[] { "x" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            if (arguments[0] == null) { return null; }
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null)
            {
                return ElementUtils.GetValuableNull();
            }
            else
            {
                return Math.Cos((double)arg0_value).ToValuable();
            }
        }
        #endregion
    }
    public class Tan : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "tan"; } }
        public override string[] Arguments { get { return new string[] { "x" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            if (arguments[0] == null) { return null; }
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null)
            {
                return ElementUtils.GetValuableNull();
            }
            else
            {
                return Math.Tan((double)arg0_value).ToValuable();
            }
        }
        #endregion
    }
    public class ASin : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "asin"; } }
        public override string[] Arguments { get { return new string[] { "x" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            if (arguments[0] == null) { return null; }
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null)
            {
                return ElementUtils.GetValuableNull();
            }
            else
            {
                return Math.Asin((double)arg0_value).ToValuable();
            }
        }
        #endregion
    }
    public class ACos : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "acos"; } }
        public override string[] Arguments { get { return new string[] { "x" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            if (arguments[0] == null) { return null; }
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null)
            {
                return ElementUtils.GetValuableNull();
            }
            else
            {
                return Math.Acos((double)arg0_value).ToValuable();
            }
        }
        #endregion
    }
    public class ATan : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "atan"; } }
        public override string[] Arguments { get { return new string[] { "x" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            if (arguments[0] == null) { return null; }
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null)
            {
                return ElementUtils.GetValuableNull();
            }
            else
            {
                return Math.Atan((double)arg0_value).ToValuable();
            }
        }
        #endregion
    }
    public class Abs : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "abs"; } }
        public override string[] Arguments { get { return new string[] { "n" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null) { return ElementUtils.GetValuableNull(); }
            return Math.Abs((double)arg0_value).ToValuable();
        }
        #endregion
    }
    public class Sign : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "sign"; } }
        public override string[] Arguments { get { return new string[] { "n" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {

            double? arg0_value = arguments[0].Value;
            if (arg0_value == null) { return ElementUtils.GetValuableNull(); }
            return Math.Sign((double)arg0_value).ToValuable();
        }
        #endregion
    }
    public class Floor : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "floor"; } }
        public override string[] Arguments { get { return new string[] { "n" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {

            double? arg0_value = arguments[0].Value;
            if (arg0_value == null) { return ElementUtils.GetValuableNull(); }
            return Math.Floor((double)arg0_value).ToValuable();
        }
        #endregion
    }
    public class Ceiling : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "ceiling"; } }
        public override string[] Arguments { get { return new string[] { "n" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {

            double? arg0_value = arguments[0].Value;
            if (arg0_value == null) { return ElementUtils.GetValuableNull(); }
            return Math.Ceiling((double)arg0_value).ToValuable();
        }
        #endregion
    }
    public class Round : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "round"; } }
        public override string[] Arguments { get { return new string[] { "n" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_value = arguments[0].Value;
            if (arg0_value == null) { return ElementUtils.GetValuableNull(); }
            return Math.Round((double)arg0_value).ToValuable();
        }
        #endregion
    }
    public class Round2 : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "round"; } }
        public override string[] Arguments { get { return new string[] { "n", "d" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_val = arguments[0].Value;
            double? arg1_val = arguments[1].Value;
            if (arg0_val == null) { return ElementUtils.GetValuableNull(); }
            if (arg1_val == null) { return ElementUtils.GetValuableNull(); }

            int args1_toInt = (int)Math.Round((double)arg1_val);
            args1_toInt = Math.Min(15, args1_toInt);

            return Math.Round((double)arg0_val, args1_toInt).ToValuable();
        }
        #endregion
    }
    public class Log : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "log"; } }
        public override string[] Arguments { get { return new string[] { "n" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_val = arguments[0].Value;
            if (arg0_val == null) { return ElementUtils.GetValuableNull(); }
            return Math.Log10((double)arg0_val).ToValuable();
        }
        #endregion
    }
    public class Min : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "min"; } }
        public override string[] Arguments { get { return new string[] { "a", "b" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            return ElementUtils.Min(arguments[0], arguments[1]);
        }
        #endregion
    }
    public class Max : BuiltInFunction
    {
        #region properties
        public override string Name { get { return "max"; } }
        public override string[] Arguments { get { return new string[] { "a", "b" }; } }
        #endregion

        #region methods
        protected override Valuable Calc(Valuable[] arguments)
        {
            return ElementUtils.Max(arguments[0], arguments[1]);
        }
        #endregion
    }
    public class True : BuiltInFunction
    {
        public override string Name { get { return "true"; } }
        protected override Valuable Calc(Valuable[] arguments)
        {
            return true.ToValuable();
        }
    }
    public class False : BuiltInFunction
    {
        public override string Name { get { return "false"; } }
        protected override Valuable Calc(Valuable[] arguments)
        {
            return false.ToValuable();
        }
    }
    public class Null : BuiltInFunction
    {
        public override string Name { get { return "null"; } }
        protected override Valuable Calc(Valuable[] arguments)
        {
            return ElementUtils.GetValuableNull();
        }
    }
    public class Void : BuiltInFunction
    {
        public override string Name { get { return "void"; } }
        protected override Valuable Calc(Valuable[] arguments)
        {
            return null;
        }
    }
    public class Log2 : BuiltInFunction
    {
        public override string Name { get { return "log"; } }
        public override string[] Arguments { get; } = new string[] { "n", "b" };
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_val = arguments[0].Value;
            double? arg1_val = arguments[1].Value;

            if (arg0_val == null || arg1_val == null) { return ElementUtils.GetValuableNull(); }

            return Math.Log((double)arg0_val, (double)arg1_val).ToValuable();

        }
    }
    public class NaturalLog : BuiltInFunction
    {
        public override string Name { get { return "ln"; } }
        public override string[] Arguments { get; } = new string[] { "n" };
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_val = arguments[0].Value;

            if (arg0_val == null) { return ElementUtils.GetValuableNull(); }

            return Math.Log((double)arg0_val, Math.E).ToValuable();

        }
    }
    public class ToRadians : BuiltInFunction
    {
        public override string Name { get { return "rad"; } }
        public override string[] Arguments { get; } = new string[] { "d" };
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_val = arguments[0].Value;

            if (arg0_val == null) { return ElementUtils.GetValuableNull(); }

            return (arg0_val * (Math.PI / 180)).ToValuable();
        }
    }
    public class ToDegrees : BuiltInFunction
    {
        public override string Name { get { return "deg"; } }
        public override string[] Arguments { get; } = new string[] { "r" };
        protected override Valuable Calc(Valuable[] arguments)
        {
            double? arg0_val = arguments[0].Value;

            if (arg0_val == null) { return ElementUtils.GetValuableNull(); }

            return (arg0_val * (180 / Math.PI)).ToValuable();
        }
    }
    public class GravitationalConstant : BuiltInFunction
    {
        public override string Name { get { return "G"; } }
        protected override Valuable Calc(Valuable[] arguments)
        {
            return 0.000000000067408.ToValuable();
        }
    }
    #endregion

    #endregion
}
