using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoParseV2.the_universe.Commands
{
    public class Exit_cmd : Command
    {
        public override string Definition { get; } = "Terminates the session.";
        protected override string name { get; } = "exit";

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
        protected override string name { get; } = "help";
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
        protected override string name { get; } = "listvars";
        protected override void exec(string args, Universe universe)
        {
            Func<string> read = universe.ReadFunction;
            Action<string> print = universe.PrintFunction;
        }
    }
}
