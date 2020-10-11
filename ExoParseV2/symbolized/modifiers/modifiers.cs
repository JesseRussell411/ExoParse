﻿using System;
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
        protected override IElement calc(IElement item)
        {
            return item.Definition;
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

    // kinda just for fun, it toggles a boolean. a is true; a=!; a is now false
    public class EqualsNot_mod : PostModifier
    {
        public override string Symbol { get { return "=!"; } }
        protected override IElement calc(IElement item)
        {
            item.TryChangeDefinition(self => LogicUtils.Not(self.Execute()).ToElement());
            return item;
        }
        protected override IElement pass(IElement item, Modification parent)
        {
            return parent;
        }
    }

}
