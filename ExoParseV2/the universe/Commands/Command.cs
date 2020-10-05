using ConvenienceTools;
using ParsingTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.the_universe.Commands
{
    public abstract class Command
    {
        private string name;
        public string Name
        {
            get { return name; }
            protected set
            {
                name = value.ToLower();
            }
        }
        public abstract string Definition { get; }
        public virtual string Manual
        {
            get { return Definition; }
        }
        public void Execute(string args, Universe universe)
        {
            exec(args, universe);
        }
        protected abstract void exec(string args, Universe universe);
        protected Tokenizer argSplitter { get; } = new Tokenizer(new string[0], ParsingProps.Barriers, new string[0], new string[0], new string[0], new string[0], ParsingProps.Barriers, new string[0]) { BreakOnWhiteSpace = true, IncludeEmpty = false };
    }
}
