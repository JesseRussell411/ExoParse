using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExoParseV2.elements;

namespace ExoParseV2.theUniverse.commands
{
    public class Exit_cmd : Command
    {
        public override string Name { get; } = "exit";
        public override string Definition { get; } = "Terminates the session.";
        
        protected override string exec(string args, Universe universe)
        {
            if (int.TryParse(args, out int i))
            {
                System.Environment.Exit(i);
            }
            else
            {
                System.Environment.Exit(0);
            }
            return "";
        }
    }
    public class Help_cmd : Command
    {
        public override string Name { get; } = "help";
        public override string Definition { get; } = "Lists all commands and their definition. If a specific command is given as an argument, " +
            "then that command's manual is displayed unless the command has no manual, then its definition will be displayed.";
        protected override string exec(string args, Universe universe)
        {
            // Create result builder.
            StringBuilder result = new StringBuilder();

            // trim args
            args = args.Trim();
            //


            if (args.Length > 0) // Is there a command specified?
            {
                // If so, find that command and list it's manual entry.
                if (universe.Commands.TryGetValue(args, out Command cmd))
                {
                    result.Append($"{cmd.Name}:");

                    for (int i = 0; i < cmd.Name.Length + 1; i++) { result.Append("-"); }
                    result.Append("\n");
                    result.Append($"{cmd.Manual}\n\n");

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
                    result.Append($"{cmd.Name}:");
                    for (int i = 0; i < cmd.Name.Length + 1; i++) { result.Append("-"); }
                    result.Append($"\n{cmd.Definition}\n\n");
                }
                //
            }

            // Return the result string.
            return result.ToString();
        }
    }
    public class ListVars_cmd : Command
    {
        public override string Name { get; } = "listVars";
        public override string Definition { get; } = "Lists all of the constants and variables defined in the environment.";
        protected override string exec(string args, Universe universe)
        {
            StringBuilder result = new StringBuilder();

            Action<object> print = o => result.Append(o.ToString());
            Action<object> println = o => print($"{o} \n");


            foreach (var g in universe.References.Select(p => p.Value).GroupBy(l => l is Variable))
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

            return result.ToString();
        }

    }
    public class Debug_cmd : Command
    {
        public override string Name { get; } = "debug";
        public override string Definition { get; } = "Toggles debug mode.";
            
        protected override string exec(string args, Universe universe)
        {
            universe.Debug = !universe.Debug;
            return $"Debug mode is {(universe.Debug ? "on" : "off")}.\n\n";
        }
    }
    public class ListFuncs_cmd : Command
    {
        public override string Name { get; } = "listFuncs";
        public override string Definition { get; } = "Lists all registered functions.";

        protected override string exec(string args, Universe universe)
        {
            StringBuilder result = new StringBuilder();

            Action<object> print = o => result.Append(o.ToString());
            Action<object> println = o => print($"{o} \n");

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
            return result.ToString();
        }

    }
    public class ListOps_cmd : Command
    {
        public override string Name { get; } = "listOps";
        public override string Definition { get; } = "List all operators and modifiers";
        protected override string exec(string args, Universe universe)
        {
            StringBuilder result = new StringBuilder();


            foreach (var g in universe.SymbolizedIndex.ItemsFlat)
            {
                if (g is Operator)
                {
                    result.Append($"A{g}B\n");
                }
                else if (g is PreModifier)
                {
                    result.Append($"{g}A\n");
                }
                else if (g is PostModifier)
                {
                    result.Append($"A{g}\n");
                }
                
            }

            return result.ToString();
        }
    }
    public class Def_cmd : Command
    {
        public override string Name { get; } = "def";
        public override string Definition { get; } = "Defines a function or constant.";
        public override string Manual { get; } =
            "Defines a new function or constant.\n" +
            "\n" +
            "The item to be defined is placed to the left of the \"=\" symbol.\n" +
            "Whether the item is constant or function is based on the syntax for both.\n" +
            "Constants follow the same naming rules as variables, while functions always end in parenthesis like this: func().\n" +
            "The function's parameters are included inside these parenthesis like this: fun(param1, param2).\n" +
            "This function and it's parameters can be named anything, of course. Like this: sum(a, b).\n" +
            "\n" +
            "The value of the item being defined is placed to the right of the \"=\" symbol like this: sum(a, b) = a + b. Or: nine = 9.\n" +
            "\n" +
            "Examples:\n" +
            ":def nine = 9\n" +
            ":def sum(a, b) = a + b\n" +
            ":def sinOfPi = sin(pi)\n";
        protected string define(string args, Universe universe, bool redefine = false)
        {
            StringBuilder result = new StringBuilder();

            Action<object> print = o => result.Append(o.ToString());
            Action<object> println = o => print($"{o} \n");

            Action thro = () => throw new GenericCommandException("Invalid arguments.");

            string[] args_split = args.Split("=", 2).Select(s => s.Trim()).ToArray();// split the args around the "=" symbol

            if (args_split.Length != 2) { thro(); }// there wasn't a "=", invalid arguments.

            // If this is a functions that's been entered do this
            if (universe.Parser.IsFunction(args_split[0].Trim(), out (string name, List<string> @params) partParsed))
            {
                var @params = new Dictionary<string, IReference>();


                foreach (var param in partParsed.@params)
                {
                    string param_trim = param.Trim();
                    @params.Add(param_trim, new Variable(param_trim));
                }

                // go copy the arguments out of params before it gets filled with other local variables.
                var params_items = @params.Values.Select(p => (Variable)p).ToArray();

                // what is this functions called? what does it look like?
                CustomFunction f = new CustomFunction(partParsed.name, null, params_items, null);


                if (universe.Functions.TryGetValue(f.Id, out Function existingFunction))
                {
                    if (redefine)
                    {
                        if (existingFunction is BuiltInFunction)
                        {
                            throw new GenericCommandException($"{f} is a built in function and cannot be redefined.");
                        }
                    }
                    else
                    {
                        throw new GenericCommandException($"{existingFunction} already exists.{(!(existingFunction is BuiltInFunction) ? " Use redef to redefine it." : "")}");
                    }
                }

                // good to go, add the function
                if (existingFunction != null)
                {
                    universe.Functions.Remove(existingFunction.Id);
                    universe.AddFunction(f);
                    println($"{f} has been redefined.");
                }
                else
                {
                    universe.AddFunction(f);
                    println($"{f} has been created.");
                }




                // define the behavior of the new function (and fill params with any locally created variables other than the function's arguments)
                f.Behavior = universe.Parser.ParseElement(args_split[1], @params);

                // set the functions localVars field to these local variables.
                f.LocalVars = @params.Select(p => p.Value)
                                     .Where(l => l is Variable)
                                     .Select(l => (Variable)l)
                                     .ToArray();



                return result.ToString();// done
            }
            else if (universe.Parser.IsLabel(args_split[0]))
            {
                // this isn't a function, but it could be a constant
                Constant con = new Constant(args_split[0], universe.Parser.ParseElement(args_split[1]));

                // is this constant name already taken?
                if (universe.References.TryGetValue(con.Name, out IReference existing))
                {
                    if (redefine)
                    {
                        if (existing is BuiltInConstant)
                        {
                            throw new GenericCommandException($"{existing} is a built in constant and cannot be redefined");
                        }
                    }
                    else
                    {
                        throw new GenericCommandException($"{existing} already exist.{(!(existing is BuiltInConstant) ? " Use redef to redefine it." : "")}");
                    }
                }

                // good to go
                if (existing!= null)
                {
                    universe.References.Remove(existing.Name);
                    universe.AddLabeled(con);
                    println($"{con} has been redefined.");
                }
                else
                {
                    universe.AddLabeled(con);
                    println($"{con} has been created.");
                }

            }
            else
            {
                thro();// yo, thro, somethin's wrong.
            }

            return result.ToString();
        }
        protected override string exec(string args, Universe universe)
        {
            return define(args, universe);
        }
    }
    public class Redef_cmd : Def_cmd
    {
        public override string Name { get; } = "redef";
        public override string Definition { get; } = "Defines a function or constant. If it already exists, it is redefined unless it's built in.";
        protected override string exec(string args, Universe universe)
        {
            return define(args, universe, true);
        }
    }
    public class Delete_cmd : Command
    {
        public override string Name { get; } = "delete";
        public override string Definition { get; } = "Deletes a function, constant, or variable.";
        public override string Manual { get; } =
            "Deletes a function, constant, or variable.\n" +
            "\n" +
            "The functions, constant, or variable to be deleted is specified as an argument.\n" +
            "If a constant or variable is to be deleted, the name should be provided.\n" +
            "If a functions is to be deleted, the functions name should be given as well as the number of parameters the functions takes.\n" +
            "If the functions takes zero parameters enter 0" +
            "For example, To delete the round(x) function: :delete round 1\n" +
            "To delete the true constant, enter: :delete true\n" +
            "\n" +
            "Examples:\n" +
            ":delete nine\n" +
            ":delete sum 2\n" +
            ":delete true\n" +
            ":delete pi\n" +
            ":delete sin 1\n" +
            ":delete func 0\n";

        protected override string exec(string args, Universe universe)
        {
            List<string> argList = argSplitter.Tokenize(args);
            if (argList.Count == 2)
            {
                // Function
                if (int.TryParse(argList[1], out int i))
                {
                    if (universe.Functions.TryGetValue((argList[0], i), out Function f))
                    {
                        if (f is BuiltInFunction)
                        {
                            throw new GenericCommandException($"{f} is a built in function and cannot be deleted.");
                        }
                        else
                        {
                            universe.Functions.Remove(f.Id);
                            return $"{f} has been deleted.\n";
                        }
                    }
                    else
                    {
                        throw new GenericCommandException($"Function not found.");
                    }
                }
                else
                {
                    throw new GenericCommandException($"{argList[1]} is not a valid integer. This argument should represent the functions parameter count.");
                }
            }
            else if (argList.Count == 1)
            {
                // Constant
                if (universe.References.TryGetValue(argList[0], out IReference r))
                {
                    if (r is BuiltInConstant)
                    {
                        throw new GenericCommandException($"{r} is a built in constant and cannot be deleted.");
                    }
                    else
                    {
                        universe.References.Remove(r.Name);
                        return $"{r} has been deleted.\n";
                    }
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
    public class Echo_cmd : Command
    {
        public override string Name { get; } = "echo";
        public override string Definition { get; } = "Prints the argument.";

        protected override string exec(string args, Universe universe)
        {
            return args;
        }
    }
    public class Run_cmd : Command
    {
        public override string Name { get; } = "run";
        public override string Definition { get; } = "Runs the script at the path specified.";

        protected override string exec(string args, Universe universe)
        {
            string path = args.Trim();
            if (File.Exists(path))
            {
                StringBuilder result = new StringBuilder();
                using (StreamReader sr = File.OpenText(path))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            result.Append($"{universe.RunStatement(line)}\n");
                        }
                        catch(MessageException e)
                        {
                            result.Append($" *{e.Message}\n\n");
                            
                        }
                    }
                }
                return result.ToString();
            }
            else
            {
                throw new GenericCommandException($"{path} could not be found.");
            }
        }
    }
    public class GenerateScript_cmd : Command
    {
        public override string Name { get; } = "generateScript";
        public override string Definition { get; } = "At the path specified, generates a script that re-creates the current state of the program when run.";

        protected bool generate(StreamWriter sw, Universe universe)
        {
            #region constants and variables
            sw.WriteLine($"{universe.CommentOperator} Constants and variables:");
            sw.WriteLine($"{universe.CommentOperator} ------------------------");
            foreach (var item in universe.References.Values)
            {
                if (!(item is BuiltInConstant))
                {
                    sw.WriteLine($"{(item is Constant ? $"{universe.CommandOperator}{Command.defName} {item.Name}" : item.Name)} {(item.Definition is Literal ? "=" : ":=")} {item.Definition.ToString(universe.SymbolizedIndex)}");
                }
            }
            #endregion

            #region functions
            sw.WriteLine($"{universe.CommentOperator} Functions:");
            sw.WriteLine($"{universe.CommentOperator} ----------");
            foreach (var func in universe.Functions.Values)
            {
                if ((func is CustomFunction cf))
                {
                    sw.WriteLine($"{universe.CommandOperator}{Command.defName} {cf} = {cf.Behavior.ToString(universe.SymbolizedIndex)}");
                }
            }
            #endregion
            return true;
        }

        protected StreamWriter OpenFileForWriting(string path)
        {
            try
            {
                return new StreamWriter(path);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new GenericCommandException(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new GenericCommandException(e.Message);
            }
            catch (IOException e)
            {
                throw new GenericCommandException(e.Message);
            }
        }

        protected override string exec(string args, Universe universe)
        {
            string path = args.Trim();
            if (File.Exists(path))
            {
                throw new GenericCommandException($"{path} already exists. use reGenerateScript to replace it.");
            }
            else
            {
                using (StreamWriter sw = OpenFileForWriting(path))
                {
                    generate(sw, universe);
                }
                return "Script generated at {path}\n";
            }
        }
    }
    public class ReGenerateScript_cmd : GenerateScript_cmd
    {
        public override string Name { get; } = "regenerateScript";
        public override string Definition { get; } =
            "At the path specified, generates a script that re-creates the current state of the program when run." +
            " If the script already exists, it is replaced.";

        protected override string exec(string args, Universe universe)
        {
            string path = args.Trim();
            using (StreamWriter sw = OpenFileForWriting(path))
            {
                generate(sw, universe);
            }
            return "Script generated at {path}\n";
        }
    }
}
