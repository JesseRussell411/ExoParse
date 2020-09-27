using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExoParsev1
{
    public abstract class Command
    {
        public string Name
        {
            get
            {
                return name.ToLower();
            }
        }
        protected abstract string name { get; }
        public void Execute(string args, Environment env)
        {
            Exec(args, env);
        }
        protected abstract void Exec(string args, Environment env);
    }

    // NOTE: Command names are not caps-sensitive.

    class ListVars : Command
    {
        protected override string name { get; } = "listvars";
        //public Dictionary<string, Variable> VariableContainer { get; set; }
        protected override void Exec(string args, Environment env)
        {
            Func<string> read = env.ReadFunction;
            Action<string> print = env.PrintFunction;
            if (print != null)
            {
                if (env.VariableContainer == null)
                {
                    throw new MessageException("Something is very wrong, go get help!\nThe variable container doesn't exist!");
                }
                else
                {
                    // Get the symbol for the setDefinition operator (which is probably ":=").
                    string DEF = new SetDefinition().Symbol;
                    string EQ = new SetEqual().Symbol;
                    int count = 0;

                    foreach (KeyValuePair<string, Variable> kv in env.VariableContainer)
                    {
                        Variable v = kv.Value;

                        // Print the variable's name with the variable definition ("a := 2 + b").
                        string s;
                        if (v.Definition is constant)
                        {
                            s = new SetEqual().Symbol;
                        }
                        else
                        {
                            s = new SetDefinition().Symbol;
                        }
                        if (v.Definition != null)
                        {
                            print($"    {v.Name} {s} {v.Definition}\n\n");
                        }
                        else
                        {
                            print($"    {v.Name}\n\n");
                        }
                        count++;
                    }
                    print($"    {count} registered variable{(count == 1 ? "" : "s")}.\n");
                }
            }
        }
    }
    class DeleteAllVariables : Command
    {
        protected override string name { get; } = "deleteallvariables";

        protected override void Exec(string args, Environment env)
        {
            // If this were c++, this would be a memory leak because the contents were not deleted first.
            // I think I would create a wrapper class for the variable container dictionary, that deletes variables when removed.
            // What are you talking about? this isn't c++. Shut up.
            int count = env.VariableContainer.Count;
            env.VariableContainer.Clear();
            env.PrintLine($"    {count} variables deleted.\n");
        }
    }
    class DeleteVar : Command
    {
        protected override string name { get; } = "deletevar";

        protected override void Exec(string args, Environment env)
        {
            string name = args.Trim();
            if (env.VariableContainer.Remove(name))
            {
                env.PrintLine($"{name} has been deleted.\n");
            }
            else
            {
                throw new MessageException($"Variable not found: {name}\n");
            }
        }
    }
    class DefineFunc : Command
    {
        protected override string name { get; } = "def";

        protected override void Exec(string args, Environment env)
        {
            string func = "";
            string def = "";
            {
                StringBuilder funcBuilder = new StringBuilder();
                StringBuilder defBuilder = new StringBuilder();
                string EQ = "=";
                SymbolFinder finder = new SymbolFinder(EQ);
                bool preEq = true;

                char c;
                char? next;
                for (int i = 0; i < args.Length; i++)
                {
                    c = args[i];
                    next = i == args.Length - 1 ? null : (char?)args[i + 1];


                    if (preEq && finder.Found(c, next))
                    {
                        preEq = false;
                        if (EQ.Length > 1) { funcBuilder.Remove(funcBuilder.Length - (EQ.Length - 1), EQ.Length - 1); } // Remove the equality symbol from funcBuilder. of course the full symbol hasn't been appended yet so we only need to remove some of it. And that's only if the symbol is longer than 1 character.
                    }
                    else if (preEq)
                    {
                        funcBuilder.Append(c);
                    }
                    else
                    {
                        defBuilder.Append(c);
                    }
                }
                func = funcBuilder.ToString().Trim();
                def = defBuilder.ToString();
            }


            (string name, string[] args) partParsed;
            if (func.IsFunction(env.Parser.OpenParenthesis, env.Parser.CloseParenthesis, out partParsed))
            {
                Dictionary<string, Variable> localVarCan = new Dictionary<string, Variable>();
                foreach (string arg in partParsed.args)
                {
                    if (arg.IsVariable())
                    {
                        if (!localVarCan.TryAdd(arg, new Variable(arg)))
                        {
                            throw new MessageException($"Duplicate argument: {arg}");
                        }
                    }
                    else
                    {
                        throw new MessageException($"Not a valid argument name: {arg}\nMust be a valid variable name to be a valid argument name.");
                    }
                }

                List<Valuable> behavior = new List<Valuable>();
                foreach(string statement in def.Split(env.LineSplitOperator))
                {
                    behavior.Add(env.Parser.ParseExpression(statement, null, localVarCan));
                }

                Function function = new CustomFunction(partParsed.name, behavior.ToArray(), localVarCan.Values.ToArray());

                if (!env.TryAddFunction(function))
                {
                    throw new MessageException($"Function already exists.");
                }
            }
            else
            {
                throw new MessageException($"Invalid function: {func}");
            }
        }
    }

    class ReDefineFunc : Command
    {
        protected override string name { get; } = "redef";

        protected override void Exec(string args, Environment env)
        {
            string func = "";
            string def = "";
            {
                StringBuilder funcBuilder = new StringBuilder();
                StringBuilder defBuilder = new StringBuilder();
                string EQ = "=";
                SymbolFinder finder = new SymbolFinder(EQ);
                bool preEq = true;

                char c;
                char? next;
                for (int i = 0; i < args.Length; i++)
                {
                    c = args[i];
                    next = i == args.Length - 1 ? null : (char?)args[i + 1];


                    if (preEq && finder.Found(c, next))
                    {
                        preEq = false;
                        if (EQ.Length > 1) { funcBuilder.Remove(funcBuilder.Length - (EQ.Length - 1), EQ.Length - 1); } // Remove the equality symbol from funcBuilder. of course the full symbol hasn't been appended yet so we only need to remove some of it. And that's only if the symbol is longer than 1 character.
                    }
                    else if (preEq)
                    {
                        funcBuilder.Append(c);
                    }
                    else
                    {
                        defBuilder.Append(c);
                    }
                }
                func = funcBuilder.ToString().Trim();
                def = defBuilder.ToString();
            }


            (string name, string[] args) partParsed;
            if (func.IsFunction(env.Parser.OpenParenthesis, env.Parser.CloseParenthesis, out partParsed))
            {
                Dictionary<string, Variable> localVarCan = new Dictionary<string, Variable>();
                foreach (string arg in partParsed.args)
                {
                    if (arg.IsVariable())
                    {
                        if (!localVarCan.TryAdd(arg, new Variable(arg)))
                        {
                            throw new MessageException($"Duplicate argument: {arg}");
                        }
                    }
                    else
                    {
                        throw new MessageException($"Not a valid argument name: {arg}\nMust be a valid variable name to be a valid argument name.");
                    }
                }

                List<Valuable> behavior = new List<Valuable>();
                foreach (string statement in def.Split(env.LineSplitOperator))
                {
                    behavior.Add(env.Parser.ParseExpression(statement, null, localVarCan));
                }

                Function function = new CustomFunction(partParsed.name, behavior.ToArray(), localVarCan.Values.ToArray());

                //if (!env.TryAddFunction(function))
                //{
                //    throw new GenericMessageException($"Function already exists.");
                //}
                Function existingFunc;
                if (env.FunctionContainer.TryGetValue(function.Id, out existingFunc))
                {
                    if (existingFunc.NonReplaceable)
                    {
                        throw new MessageException("The existing function in non-removable.");
                    }
                    else
                    {
                        env.FunctionContainer.Remove(existingFunc.Id);
                        env.AddFunction(function);
                    }
                }
                else
                {
                    throw new MessageException("The function does not exist yet. To define it for the first time use the def command.");
                }
            }
            else
            {
                throw new MessageException($"Invalid function: {func}");
            }
        }
    }
    class DeleteFunc : Command
    {
        protected override string name { get; } = "deletefunc";

        protected override void Exec(string args, Environment env)
        {
            string[] args_split = args.Split(' ');
            if (args_split.Length != 2)
            {
                throw new MessageException("That is the wrong format for this command.\nplease enter it this way: \n{function name} {how many arguments the function takes}");
            }

            int argCount;
            try
            {
                argCount = int.Parse(args_split[1]);
            }
            catch(FormatException e)
            {
                throw new MessageException(e.Message);
            }

            string name = args_split[0];

            Function func;
            if (env.FunctionContainer.TryGetValue((name, argCount), out func))
            {
                if (func.NonReplaceable)
                {
                    throw new MessageException($"{func} cannot be deleted.");
                }
                else
                {
                    env.FunctionContainer.Remove((name, argCount));
                    env.PrintLine($"{func} has been deleted");
                }
            }
            else
            {
                throw new MessageException($"Function not found.");
            }
        }
    }
    public class ListFuncs : Command
    {
        protected override string name { get; } = "listfuncs";
        protected override void Exec(string args, Environment env)
        {
            int count = 0;
            int customCount = 0;
            foreach (Function func in env.FunctionContainer.Values)
            {
                env.PrintLine($"   {(func.NonReplaceable ? "*" : " ")}{func}{(func is CustomFunction ? $" = {((CustomFunction)func).Behavior.ToDelimString($"{env.LineSplitOperator} ")}" : "")}");

                count++;
                if (func is CustomFunction) { customCount++; }
            }
            env.PrintLine();
            env.PrintLine("    *Non-Removable");
            env.PrintLine();
            string s;

            s = (count - customCount == 1) ? "" : "s";
            env.PrintLine($"    {count - customCount} built in function{s}.");

            s = (customCount == 1) ? "" : "s";
            env.PrintLine($"    {customCount} user-defined function{s}.");


            s = (count == 1) ? "" : "s";
            env.PrintLine($"    {count} function{s} in total.");
        }
    }
    public class ToggleDebug : Command
    {
        private static string[] t = new string[] { "1", "t", "true", "y", "yes" };
        private static string[] f = new string[] { "0", "f", "false", "n", "no" };
        protected override string name { get; } = "debug";
        protected override void Exec(string args, Environment env)
        {
            args = args.Trim().ToLower();
            if (t.Contains(args))
            {
                env.Debug = true;
            }
            else if (f.Contains(args))
            {
                env.Debug = false;
            }
            else
            {
                env.Debug = !env.Debug;
            }

            env.PrintLine($"Debug mode is {(env.Debug ? "ON" : "OFF")}.\n");
        }
    }

}