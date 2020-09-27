using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParsingTools;

namespace ExoParseV2
{
    public abstract class Function
    {
        public abstract string Name { get; }
        public abstract string[] Arguments { get; }
        public virtual int ArgCount { get { return Arguments.Length; } }
        public (string name, int argCount) Id { get { return (name: Name, argCount: ArgCount); } }

        public IElement Calculate(params double?[] args)
        {
            return Calculate(args.Select(a => a.ToElement()).ToArray());
        }
        public virtual IElement Calculate(params IElement[] args)
        {
            if (args.Length != ArgCount)
            {
                return IElement.Void;
            }
            foreach (IElement arg in args)
            {
                if (arg == IElement.Void)
                {
                    return IElement.Void;
                }
            }

            return calc(args);
        }

        protected abstract IElement calc(IElement[] args);


        public override string ToString()
        {
            return ToString();
        }
        public string ToString(string delim = null, string openingBracket = null, string closingBracket = null)
        {
            if (delim == null) { delim = ParsingProps.Delims[0]; }
            if (openingBracket == null) { openingBracket = ParsingProps.OpenBrackets[0]; }
            if (closingBracket == null) { closingBracket = ParsingProps.CloseBrackets[0]; }
            return $"{Name}{Arguments.ToDelimString($"{delim} ").Wrap(openingBracket, closingBracket)}";
        }
    }

    public class CustomFunction : Function
    {
        public override string Name { get; }
        public override String[] Arguments { get; }
        public IElement Definition { get; }
        public Variable[] ArgVars { get; }
        
        public CustomFunction(String name, IElement definition, Variable[] arguments)
        {
            Name = name;
            Definition = definition;
            ArgVars = arguments;
            Arguments = ArgVars.Select(v => v.Name).ToArray();
        }

        protected override IElement calc(IElement[] args)
        {
            for(int i = 0; i < ArgVars.Length; i++)
            {
                ArgVars[i].Definition = args[i];
            }
            return Definition.Pass();
        }
    }

    public abstract class BuiltInFunction : Function
    {
        // Mostly for the sake of it for now
    }
}
