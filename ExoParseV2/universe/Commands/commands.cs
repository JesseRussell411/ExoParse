using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public override string Definition { get; } = "Lists all of the constants and variables defined in the environment.";
        public ListVars_cmd()
        {
            Name = "listvars";
        }
        protected override void exec(string args, Universe universe)
        {
            Action<object> print = o => universe.PrintFunction(o.ToString());
            Action<object> println = o => print($"{o} \n");

            foreach (var g in universe.NamedItems.Select(p => p.Value).GroupBy(l => l is Variable))
            {
                if (g.Key)
                {
                    println("Variables:");
                    println("----------");
                }
                else
                {
                    println("Constant:");
                    println("---------");
                }

                foreach(var l in g)
                {
                    println($"{l.ToString(universe.SymbolizedIndex, null)} := {l.Definition?.ToString(universe.SymbolizedIndex, null) ?? StringProps.VoidLabel}");
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
    public class ListFuncs_cmd : Command
    {
        public override string Definition { get; } = "Lists all registered functions.";
        public ListFuncs_cmd()
        {
            Name = "listfuncs";
        }

        protected override void exec(string args, Universe universe)
        {
            var print = universe.PrintFunction;
            Action<string> println = s => print(s + "\n");
            foreach(var g in universe.Functions.Select(p => p.Value).GroupBy(f => f is CustomFunction))
            {
                if (g.Key)
                {
                    println("custom functions:");
                    println("-----------------");
                }
                else
                {
                    println("built-in functions:");
                    println("-------------------");
                }

                foreach (var f in g)
                {
                    print($"{f}");
                    if (g.Key)
                    {
                        print($" = { ((CustomFunction)f).Behavior.ToString(universe.SymbolizedIndex, null) }");
                    }
                    println("");
                }

                println("");
            }
        }

    }
    public class Def_cmd : Command
    {
        public override string Definition { get; } = "Defines a function or constant.";

        public Def_cmd()
        {
            Name = "def";
        }

        protected override void exec(string args, Universe universe)
        {
            Action thro = () => throw new GenericCommandException("Invalid arguments.");
            string[] args_split = args.Split("=", 2).Select(s => s.Trim()).ToArray();
            if (args_split.Length != 2) { thro(); }

            if (universe.Parser.IsFunction(args_split[0].Trim(), out (string name, List<string> @params) partParsed))
            {
                var @params = new Dictionary<string, ILabeled>();
                foreach (var param in partParsed.@params)
                {
                    string param_trim = param.Trim();
                    @params.Add(param_trim, new Variable(param_trim));
                }

                CustomFunction f = new CustomFunction(partParsed.name, universe.Parser.ParseElement(args_split[1], @params), @params.Values.Select(p => (Variable)p).ToArray());

                if (universe.Functions.ContainsKey(f.Id))
                {
                    throw new GenericCommandException($"{f} already exists.");
                }
                else
                {
                    universe.AddFunction(f);
                    return;
                }
            }
            else if (universe.Parser.IsLabel(args_split[0]))
            {
                Constant con = new Constant(args_split[0], universe.Parser.ParseElement(args_split[1]));
                if (universe.NamedItems.ContainsKey(con.Name))
                {
                    throw new GenericCommandException($"{con} already exists.");
                }
                else
                {
                    universe.AddLabeled(con);
                    return;
                }
            }
            else
            {
                thro();
            }
        }
    }
    public class Delete_cmd : Command
    {
        public override string Definition { get; } = "Deletes a function or constant.";

        public Delete_cmd()
        {
            Name = "delete";
        }

        protected override void exec(string args, Universe universe)
        {
            List<string> argList = argSplitter.Tokenize(args);
            if (argList.Count == 2)
            {
                // Function
                if (int.TryParse(argList[1], out int i))
                {
                    if (universe.Functions.TryGetValue((argList[0], i), out Function f))
                    {
                        universe.Functions.Remove(f.Id);
                        universe.PrintFunction($"{f} has been deleted.\n");
                    }
                    else
                    {
                        throw new GenericCommandException($"Function not found.");
                    }
                }
                else
                {
                    throw new GenericCommandException($"{argList[1]} is not a vallid integer. This argument should represent the functions parameter count.");
                }
            }
            else if (argList.Count == 1)
            {
                // Constant
                if (universe.NamedItems.TryGetValue(argList[0], out ILabeled l) && l is Constant)
                {
                    universe.NamedItems.Remove(l.Name);
                    universe.PrintFunction($"{l} has been deleted.\n");
                }
                else
                {
                    throw new GenericCommandException($"{argList[0]} not found.");
                }
            }
            else
            {
                throw new GenericCommandException("Invalid input.");
            }
        }
    }
}
