﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public class Negative_mod : PreModifier
    {
        public override string Symbol { get { return "-"; } }
        public override IElement calc(IElement item)
        {
            return (-item.Execute()).ToElement();
        }
    }
    public class Positive_mod : PreModifier
    {
        public override string Symbol { get { return "+"; } }
        public override IElement calc(IElement item)
        {
            return item.Execute().ToElement();
        }
    }
    public class Not_mod : PreModifier
    {
        public override string Symbol { get { return "!"; } }
        public override IElement calc(IElement item)
        {
            return LogicUtils.Not(item.Execute()).ToElement();
        }
    }

    public class Dereference_mod : PreModifier
    {
        public override string Symbol { get { return "$"; } }
        public override IElement calc(IElement item)
        {
            return item.Definition;
        }
    }
    public class IncrementAfter_mod : PostModifier
    {
        public override string Symbol { get { return "++"; } }
        public override IElement calc(IElement item)
        {
            if (item is IDefinable)
            {
                double? item_Execute = item.Execute();
                ((IDefinable)item).Definition = (item_Execute + 1).ToElement();
                return item_Execute.ToElement();
            }
            else
            {
                throw new NotDefinableException(item);
            }
        }
    }
    public class IncrementBefore_mod : PreModifier
    {
        public override string Symbol { get { return "++"; } }
        public override IElement calc(IElement item)
        {
            return item.TryChangeDefinition(self => (self.Execute() + 1).ToElement());
        }
    }
    public class DecrementAfter_mod : PostModifier
    {
        public override string Symbol { get { return "--"; } }
        public override IElement calc(IElement item)
        {
            if (item is IDefinable)
            {
                double? item_Execute = item.Execute();
                ((IDefinable)item).Definition = (item_Execute - 1).ToElement();
                return item_Execute.ToElement();
            }
            else
            {
                throw new NotDefinableException(item);
            }
        }
    }
    public class DecrementBefore_mod : PreModifier
    {
        public override string Symbol { get { return "--"; } }
        public override IElement calc(IElement item)
        {
            return item.TryChangeDefinition(self => (self.Execute() - 1).ToElement());
        }
    }

    public class Factorial_mod : PostModifier
    {
        public override string Symbol { get { return "!"; } }
        public override IElement calc(IElement item)
        {
            return MathUtils.Factorial(item.Execute()).ToElement();
        }
    }

    public class EqualsNot_mod : PostModifier
    {
        public override string Symbol { get { return "=!"; } }
        public override IElement calc(IElement item)
        {
            item.TryChangeDefinition(self => LogicUtils.Not(self.Execute()).ToElement());
            return item;
        }
    }
}