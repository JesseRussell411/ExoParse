using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.the_universe.Commands
{
    public class ListVars : Command
    {
        protected override string name { get; } = "listvars";
        protected override void exec(string args, Universe universe)
        {
            Func<string> read = universe.ReadFunction;
            Action<string> print = universe.PrintFunction;
        }
    }
}
