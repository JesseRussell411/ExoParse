using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Reflection.Metadata;

namespace ExoParsev1
{
    public static class EnvUtils
    {
        // Nothing here yet.

    }
    public class EnvironmentFactory
    {
        public Dictionary<(string name, int argcount), Function>               BuiltInFunctions { get; set; }
        public Dictionary<string, Command>                                     Commands         { get; set; }
        public Dictionary<string, Operator>                                    Operators        { get; set; }
        public Dictionary<(string symbol, Orientation orientation), Modifier>  Modifiers        { get; set; }

        public void BuiltInFunctions_Add(Function function)
        {
            BuiltInFunctions.Add((function.Name, function.ArgCount), function);
        }
        public void Commands_Add(Command command)
        {
            Commands.Add(command.Name, command);
        }
        public void Operators_Add(Operator op)
        {
            Operators.Add(op.Symbol, op);
        }
        public void Modifiers_Add(Modifier mod)
        {
            Modifiers.Add((mod.Symbol, mod.Orientation), mod);
        }
        public EnvironmentFactory(Function[] builtIntFunction, Command[] commands, Operator[] ops, Modifier[] mods)
        {
            BuiltInFunctions = builtIntFunction.ToDictionary(bif => (bif.Name, bif.ArgCount));
            Commands = commands.ToDictionary(c => c.Name);
            Operators = ops.ToDictionary(o => o.Symbol);
            Modifiers = mods.ToDictionary(m => (m.Symbol, m.Orientation));
        }
        public Environment BuildEnv()
        {
            Environment env = new Environment(BuiltInFunctions.Values.ToDictionary(v => (v.Name, v.ArgCount)), Commands.Values.ToDictionary(v => v.Name), Operators.Values.ToDictionary(v => v.Symbol), Modifiers.Values.ToDictionary(v => (v.Symbol, v.Orientation)));
            return env;
        }
    }
    public class Environment
    {
        public bool Debug = false;
        public Valuable StarterElement { get; set; } = null;
        public void Run()
        {
            while (!stop)
            {
                try
                {
                    RunStatement();
                }
                catch(MessageException gme)
                {
                    PrintLine(gme.Message);
                    PrintLine();
                }
            }
            stop = true;
        }

        private bool stop = false;
        public void Stop() { stop = true; }
        public void RunStatement()
        {
            PrintFunction("> ");
            string rawInput = ReadFunction();
            string input;

            // Remove comments and trim.
            {
                SymbolFinder commentFinder = new SymbolFinder(CommentOperator);
                StringBuilder inputBuilder = new StringBuilder();
                char c;
                char? next;
                int rawInput_LastIndex = rawInput.Length - 1;
                for (int i = 0; i < rawInput.Length; i++)
                {
                    c = rawInput[i];
                    next = i == rawInput_LastIndex ? null : (char?)rawInput[i + 1];

                    inputBuilder.Append(c);

                    if (commentFinder.Found(c, next))
                    {
                        inputBuilder.Remove(inputBuilder.Length - CommentOperator.Length, CommentOperator.Length);
                        break; // No point in going through the rest of the string.
                    }
                }

                input = inputBuilder.ToString();
            }

            if (input.Length >= CommandOperator.Length ? input.Substring(0, CommandOperator.Length) == CommandOperator : false)
            {
                RunStatement(input);
            }
            else
            {
                foreach (string statement in input.Split(LineSplitOperator).Select(s => s).Where(s => s.Length > 0))
                {
                    RunStatement(statement);
                }
            }
        }
        public void RunStatement(string statement)
        {
            // Just stop everything if the statement is blank.
            if (statement.Trim().Length == 0)
            {
                return;
            }
            else if (statement.Length >= CommandOperator.Length && statement.Substring(0, CommandOperator.Length) == CommandOperator)
            {
                int index = 0;
                bool prespace = true;
                StringBuilder commandNameBuilder = new StringBuilder();
                StringBuilder argumentStringBuilder = new StringBuilder();
                foreach (char c in statement)
                {
                    if (index > 0)
                    {
                        if (prespace && c == ' ')
                        {
                            prespace = false;
                        }
                        else if (prespace)
                        {
                            commandNameBuilder.Append(c);
                        }
                        else
                        {
                            argumentStringBuilder.Append(c);
                        }
                    }
                    index++;
                }
                string commandName = commandNameBuilder.ToString().ToLower();
                string argumentString = argumentStringBuilder.ToString();
                Command command;
                //string commandName = input.Substring(CommandOperator.Length, input.IndexOf(' ') - CommandOperator.Length);
                if (Commands.TryGetValue(commandName, out command))
                {
                    command.Execute(argumentString, this);
                }
                else
                {
                    throw new MessageException($"command not found: {commandName}");
                }
            }
            else
            {
                long parseTime = 0;
                long exTime = 0;
                Stopwatch s = null;
                if (Debug) 
                { 
                    s = new Stopwatch(); 
                    s.Start();
                }

                Valuable ex = Parser.ParseExpression(statement, StarterElement);

                if (Debug)
                {
                    s.Stop();
                    parseTime = s.ElapsedMilliseconds;
                    s.Reset();
                    s.Start();
                }

                Valuable ans = ex?.Execute();

                if (Debug)
                {
                    s.Stop();
                    exTime = s.ElapsedMilliseconds;
                }

                Ans.Value = ans;

                PrintLine($" {ex.NullableToString("void")}");
                PrintLine($"     {ans.NullableToString("void")}");
                PrintLine();

                if (Debug)
                {
                    PrintLine($"time to parse:   {parseTime} milliseconds.");
                    PrintLine($"time to execute: {exTime} milliseconds.");
                    PrintLine();
                }
            }
        }
        public Parser Parser
        {
            get
            {
                return parser;
            }
            set
            {
                parser = value;
                parser.VariableContainer = VariableContainer;
                parser.FunctionContainer = FunctionContainer;
            }
        }
        public Ans Ans { get; set; } = new Ans();
        private Parser parser;
        public Action<string> PrintFunction { get; set; } = Console.Write;
        public void PrintLine() { PrintFunction("\n"); }
        public void PrintLine(string text)
        {
            PrintFunction(text);
            PrintFunction("\n");
        }
        public Func<string> ReadFunction { get; set; } = Console.ReadLine;
        public Dictionary<string, Variable> VariableContainer { get; set; }
        public Dictionary<(string name, int argCount), Function> FunctionContainer { get; set; }
        public Dictionary<string, Command> Commands { get; private set; }
        public void Commands_Add(Command command)
        {
            Commands.Add(command.Name.ToLower(), command);
        }
        public string CommandOperator { get; set; } = ":";
        public string CommentOperator { get; set; } = "#";
        public string LineSplitOperator { get; set; } = ";";
        public Environment(Dictionary<(string name, int argcount), Function> functions, Dictionary<string, Command> commands, Dictionary<string, Operator> ops, Dictionary<(string symbol, Orientation orientation), Modifier> mods)
        {
            FunctionContainer = functions;
            VariableContainer = new Dictionary<string, Variable>();
            Commands = commands;
            Parser = new Parser();
            Parser.Operators = ops.Values.ToArray();  // yeah I know, this is really ineficient, but it'll have to do for now, I might fix it later.
            Parser.Modifiers = mods.Values.ToArray(); // yeah I know, this is really ineficient, but it'll have to do for now, I might fix it later.
            Ans ans = new Ans();
            FunctionContainer.Add(ans.Id, ans);
            StarterElement = new FunctionHolder(ans);
            Ans = ans;

        }
        public bool TryAddFunction(Function func)
        {
            return FunctionContainer.TryAdd(func.Id, func);
        }
        public void AddFunction(Function func)
        {
            FunctionContainer.Add(func.Id, func);
        }
        public bool TryAddCommand(Command command)
        {
            return Commands.TryAdd(command.Name, command);
        }
        public void AddCommand(Command command)
        {
            Commands.Add(command.Name, command);
        }
        public Valuable Execute(string statement)
        {
            return Parser.ParseExpression(statement).Execute();
        }
    }
}