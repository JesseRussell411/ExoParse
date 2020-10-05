using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoParseV2.the_universe.Commands
{
    public class Exit_cmd : Command
    {
        public override string Definition { get; } = "Terminates the session.";
        public Exit_cmd()
        {
            Name = "exit";
        }

        protected override void exec(string args, Universe universe)
        {
            if (int.TryParse(args, out int i))
            {
                System.Environment.Exit(i);
            }
            else
            {
                System.Environment.Exit(0);
            }
        }
    }
    public class Help_cmd : Command
    {
        public override string Definition { get; } = "Lists all commands and their deffinition. If a specific command is given as an argument, then that commands manual is displayed.";
        public Help_cmd()
        {
            Name = "help";
        }
        protected override void exec(string args, Universe universe)
        {
            // get print function
            var print = universe.PrintFunction;
            Action<string> println = s => print($"{s}\n");
            //

            // trim args
            args = args.Trim();
            //


            if (args.Length > 0) // Is there a command specified?
            {
                // If so, find that command and list it's manual entry.
                if (universe.Commands.TryGetValue(args, out Command cmd))
                {
                    println(cmd.Name + ":");
                    for (int i = 0; i < cmd.Name.Length + 1; i++) { print("-"); }
                    println("");
                    println(cmd.Manual);
                    println("");

                }
                else
                {
                    throw new GenericCommandException($"Command not found: {args}");
                }
                //
            }
            else
            {
                // If not, list all of them
                foreach (Command cmd in universe.Commands.Select(p => p.Value))
                {
                    println(cmd.Name + ":");
                    for (int i = 0; i < cmd.Name.Length + 1; i++) { print("-"); }
                    println("");
                    println(cmd.Definition);
                    println("");
                }
                //
            }
        }
    }
    
    public class ListVars_cmd : Command
    {
        public override string Definition { get; } = "Lists all of the variables defined in the environment.";
        public ListVars_cmd()
        {
            Name = "listvars";
        }
        protected override void exec(string args, Universe universe)
        {
            Action<object> print = o => universe.PrintFunction(o.ToString());
            Action<object> println = o => print($"{o} \n");

            foreach (var g in universe.Environment.NamedItems.Select(p => p.Value).GroupBy(l => l is Constant))
            {
                if (g.Key)
                {
                    println("Constant:");
                    println("---------");
                }
                else
                {
                    println("Variables:");
                    println("----------");
                }

                foreach(var l in g)
                {
                    println($"{l.ToString(universe.SymbolizedIndex, null)} := {l.Definition?.ToString(universe.SymbolizedIndex, null) ?? ParsingProps.VoidLabel}");
                }
                
                println("");
            }
        }

    }
    public class Debug_cmd : Command
    {
        public override string Definition { get; } = "Toggles debug mode.";
        public Debug_cmd()
        {
            Name = "debug";
        }
            
        protected override void exec(string args, Universe universe)
        {
            universe.Debug = !universe.Debug;
            universe.PrintFunction($"Debug mode is {(universe.Debug ? "on" : "off" )}.\n\n");
        }
    }
}
