using ConvenienceTools;
using ParsingTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.theUniverse.commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Definition { get; }
        public virtual string Manual { get => Definition; }
        public string Execute(string args, Universe universe)
        {
            return exec(args, universe);
        }
        protected abstract string exec(string args, Universe universe);
        protected Tokenizer argSplitter { get; } = new Tokenizer(new string[0], StringProps.Barriers, new string[0], new string[0], new string[0], new string[0], StringProps.Barriers, new string[0]) { BreakOnWhiteSpace = true, IncludeEmpty = false };

        public static readonly string defName = "def";
    }
}
