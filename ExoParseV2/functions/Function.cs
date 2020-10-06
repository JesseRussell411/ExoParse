﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParsingTools;

namespace ExoParseV2
{
    /// <summary>
    /// Abstract class that represents a mathimematical function with an arbitrary number of inputs and one output.
    /// </summary>
    public abstract class Function
    {
        public abstract string Name { get; }
        /// <summary>
        /// Inputs
        /// </summary>
        public abstract string[] Parameters { get; }
        /// <summary>
        /// Number of Inputs
        /// </summary>
        public virtual int ParamCount { get { return Parameters.Length; } }
        /// <summary>
        /// Unique identifier for this function using it's name and parameter count.
        /// </summary>
        public (string name, int paramCount) Id { get { return (name: Name, paramCount: ParamCount); } }

        public IElement Calculate(params double?[] args)
        {
            return Calculate(args.Select(a => a.ToElement()).ToArray());
        }

        /// <summary>
        /// Executes the function with the given input arguments. IF the number of arguments given does not match the number of parametes, null will be returned.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IElement Calculate(params IElement[] args)
        {
            if (args.Length != ParamCount)
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
            return ToString(null,null,null);
        }
        public string ToString(string delim = null, string openingBracket = null, string closingBracket = null)
        {
            if (delim == null) { delim = StringProps.Delims[0]; }
            if (openingBracket == null) { openingBracket = StringProps.OpenBrackets[0]; }
            if (closingBracket == null) { closingBracket = StringProps.CloseBrackets[0]; }
            return $"{Name}{Parameters.ToDelimString($"{delim} ").Wrap(openingBracket, closingBracket)}";
        }
    }


    /// <summary>
    /// A function who's behavior is defined by a given IElement object. Links to the functions parameters are given as Variable objects that are assumed to exist in the given IElement behavior object.
    /// </summary>
    public class CustomFunction : Function
    {
        public override string Name { get; }
        /// <summary>
        /// Inputs
        /// </summary>
        public override string[] Parameters { get; }
        /// <summary>
        /// Defines the function's behavior.
        /// </summary>
        public IElement Behavior { get; }

        /// <summary>
        /// Links to Variable objects assumed to exist in the IElement object defining the function's behavior.
        /// </summary>
        public Variable[] ArgVars { get; }
        
        public CustomFunction(String name, IElement definition, Variable[] arguments)
        {
            Name = name;
            Behavior = definition;
            ArgVars = arguments;
            Parameters = ArgVars.Select(v => v.Name).ToArray(); // Set Parameters acordingly.
        }

        protected override IElement calc(IElement[] args)
        {
            // Set the value for each parameter to the given value.
            //Note that args is guaranteed to have the same length as ArgVars thanks to the actual interface method: Function.Calculate which calls this method.
            for (int i = 0; i < ArgVars.Length; i++)
            {
                ArgVars[i].Definition = args[i];
            }
            //


            return Behavior.Pass();
        }
    }

    public abstract class BuiltInFunction : Function
    {
        // Mostly for the sake of it for now
    }
}
