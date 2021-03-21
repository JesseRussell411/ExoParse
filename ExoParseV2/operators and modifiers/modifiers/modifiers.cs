using ExoParseV2.elements;
using JesseRussell.Numerics;

namespace ExoParseV2
{
    public class Negative_mod : PreModifier
    {
        public override string Symbol { get { return "-"; } }
        public override string Definition => "Negates the element";
        protected override IElement calc(IElement item)
        {
            return (-item.Execute()).ToElement();
        }
    }
    public class Positive_mod : PreModifier
    {
        public override string Symbol { get { return "+"; } }
        public override string Definition => "Returns the element's value unmodified.";
        protected override IElement calc(IElement item)
        {
            return item.Execute().ToElement();
        }
    }
    public class Not_mod : PreModifier
    {
        public override string Symbol { get { return "!"; } }
        public override string Definition => "Returns the logical inverse of the element if the element is either 0 or 1, null otherwise.";
        protected override IElement calc(IElement item)
        {
            return LogicUtils.Not(item.Execute()).ToElement();
        }
    }
    public class Dereference_mod : PreModifier
    {
        public override string Symbol { get { return "*"; } }
        public override string Definition => "Returns the definition of the element. If the element is a reference variable, then the definition is whatever the variable is referencing.";
        public override bool DontExecute_flag(IElement item, Modification parent)
        {
            return true;
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return calc(item);
        }
        protected override IElement calc(IElement item)
        {
            return item.Definition;
        }
        protected override IntFloatFrac? execute(IElement item, Modification parent)
        {
            return null;
        }
    }
    public class Increment_post_mod : PostModifier
    {
        public override string Symbol { get { return "++"; } }
        public override string Definition => "Increments the element by 1 and returns the value of the element from before it was incremented.";
        protected override IElement calc(IElement item)
        {
            if (item is IRedefinable)
            {
                IntFloatFrac? item_Execute = item.Execute();
                ((IRedefinable)item).Definition = (item_Execute + 1).ToElement();
                return item_Execute.ToElement();
            }
            else
            {
                throw new NotRedefinableException(item);
            }
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return parent;
        }
    }
    public class Increment_pre_mod : PreModifier
    {
        public override string Symbol { get { return "++"; } }
        public override string Definition => "Increments the element by 1 and returns the result.";
        protected override IElement calc(IElement item)
        {
            return item.TryChangeDefinition(self => (self.Execute() + 1).ToElement());
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return parent;
        }
    }
    public class Decrement_post_mod : PostModifier
    {
        public override string Symbol { get { return "--"; } }
        public override string Definition => "Decrements the element by 1 and returns the value of the element from before it was incremented.";
        protected override IElement calc(IElement item)
        {
            if (item is IRedefinable)
            {
                IntFloatFrac? item_Execute = item.Execute();
                ((IRedefinable)item).Definition = (item_Execute - 1).ToElement();
                return item_Execute.ToElement();
            }
            else
            {
                throw new NotRedefinableException(item);
            }
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return parent;
        }
    }
    public class Decrement_pre_mod : PreModifier
    {
        public override string Symbol { get { return "--"; } }
        public override string Definition => "Decrements the element by 1 and returns the result.";
        protected override IElement calc(IElement item)
        {
            return item.TryChangeDefinition(self => (self.Execute() - 1).ToElement());
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return parent;
        }
    }

    public class Factorial_mod : PostModifier
    {
        public override string Symbol { get { return "!"; } }
        public override string Definition { get; } = "returns the factorial of the element";
        protected override IElement calc(IElement item)
        {
            return MathUtils.Factorial(item.Execute()).ToElement();
        }
    }
    public class Semicolon_post_mod : PostModifier
    {
        public override string Symbol { get { return ";"; } }
        public override string Definition { get; } = "Does nothing.";
        protected override IElement calc(IElement item)
        {
            return item.Pass();
        }
    }
    public class Semicolon_pre_mod : PreModifier
    {
        public override string Symbol { get { return ";"; } }
        public override string Definition { get; } = "Does nothing.";
        protected override IElement calc(IElement item)
        {
            return item;
        }
        public override string ToString()
        {
            return $"{Symbol} ";
        }
    }
    public class ForceExecute_mod : PreModifier
    {
        public override string Symbol { get { return "$"; } }
        public override string Definition => "Executes the element and returns the value from the execution.";
        protected override IElement calc(IElement item)
        {
            return item.Execute().ToElement();
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return calc(item);
        }

    }
    public class ForceCalc_mod : PreModifier
    {
        public override string Symbol { get { return "$$"; } }
        public override string Definition => "Calculates the element and returns the value from the execution.";
        protected override IElement calc(IElement item)
        {
            return item.Calc();
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return calc(item);
        }
    }
    public class ForcePass_mod : PreModifier
    {
        public override string Definition => "Passes the element and returns the value from the execution.";
        public override string Symbol { get { return "$$$"; } }
        protected override IElement calc(IElement item)
        {
            return item.Pass();
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return calc(item);
        }
    }
}
