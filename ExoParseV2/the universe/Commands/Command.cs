using ParsingTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.the_universe.Commands
{
    public abstract class Command
    {
        public string Name { get { return name.ToLower(); } }
        protected abstract string name { get; }
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
        protected Tokenizer argSplitter { get; } = new Tokenizer() { BreakOnWhiteSpace = true, IncludeEmpty = false };
    }
}
