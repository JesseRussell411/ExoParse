using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ExoParseV2.elements;

namespace ExoParseV2
{
    public class Negative_mod : PreModifier
    {
        public override string Symbol { get { return "-"; } }
        protected override IElement calc(IElement item)
        {
            return (-item.Execute()).ToElement();
        }
    }
    public class Positive_mod : PreModifier
    {
        public override string Symbol { get { return "+"; } }
        protected override IElement calc(IElement item)
        {
            return item.Execute().ToElement();
        }
    }
    public class Not_mod : PreModifier
    {
        public override string Symbol { get { return "!"; } }
        protected override IElement calc(IElement item)
        {
            return LogicUtils.Not(item.Execute()).ToElement();
        }
    }
    public class Dereference_mod : PreModifier
    {
        public override string Symbol { get { return "$"; } }
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
        protected override double? execute(IElement item, Modification parent)
        {
            return null;
        }
    }
    public class IncrementAfter_mod : PostModifier
    {
        public override string Symbol { get { return "++"; } }
        protected override IElement calc(IElement item)
        {
            if (item is IRedefinable)
            {
                double? item_Execute = item.Execute();
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
    public class IncrementBefore_mod : PreModifier
    {
        public override string Symbol { get { return "++"; } }
        protected override IElement calc(IElement item)
        {
            return item.TryChangeDefinition(self => (self.Execute() + 1).ToElement());
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return parent;
        }
    }
    public class DecrementAfter_mod : PostModifier
    {
        public override string Symbol { get { return "--"; } }
        protected override IElement calc(IElement item)
        {
            if (item is IRedefinable)
            {
                double? item_Execute = item.Execute();
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
    public class DecrementBefore_mod : PreModifier
    {
        public override string Symbol { get { return "--"; } }
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
        protected override IElement calc(IElement item)
        {
            return MathUtils.Factorial(item.Execute()).ToElement();
        }
    }
    public class Semicolon_post_mod : PostModifier
    {
        public override string Symbol { get { return ";"; } }
        protected override IElement calc(IElement item)
        {
            return item.Pass();
        }
    }
    public class Semicolon_pre_mod : PreModifier
    {
        public override string Symbol { get { return ";"; } }
        protected override IElement calc(IElement item)
        {
            return item;
        }
        public override string ToString()
        {
            return $"{Symbol} ";
        }
    }
}
