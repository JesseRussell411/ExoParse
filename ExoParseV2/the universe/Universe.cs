using ExoParseV2.the_universe.Commands;
using System;
using System.Collections.Generic;
using ParsingTools;
using System.Text;
using System.Runtime.CompilerServices;
using ConvenienceTools;

namespace ExoParseV2.the_universe
{
    public class Universe
    {
        public Universe()
        {
            commentOpFinder = new SymbolFinder(CommentOperator);
            Commands = new Dictionary<string, Command>();
            Ans = new Constant("ans", ans_var);
        }

        public bool Debug { get; set; } = false;
        public Action<string> PrintFunction { get; set; }
        public Func<string> ReadFunction { get; set; }
        public SymbolizedIndex SymbolizedIndex { get; set; }
        public Parser Parser { get; set; }
        public string CommandOperator { get; set; } = ":"; // The symbol to go before commands.
        public string CommentOperator
        {
            get { return commentOperator; }
            set
            {
                commentOperator = value;
                commentOpFinder = new SymbolFinder(CommentOperator);
            }
        }
        private string commentOperator = "#";
        internal SymbolFinder commentOpFinder;
        public Dictionary<string, Command> Commands { get; }
        private Variable ans_var { get; } = new Variable("previousAnswer");
        public Constant Ans { get; }

        private void RunCommand(string statement)
        {
            int? nameEnd = null;
            int? argsBegin = null;
            if (statement.Trim().Length == 0)
            {
                throw new MessageException("A command was expected but no command was given.");
            }



            // Find name and args.
            {
                char c;
                for (int i = 0; i < statement.Length; i++)
                {
                    c = statement[i];
                    if (nameEnd == null)
                    {
                        if (c.IsWhiteSpace())
                        {
                            nameEnd = i;
                        }
                    }
                    else if (argsBegin == null && !c.IsWhiteSpace())
                    {
                        argsBegin = i;
                        break;
                    }
                }

                if (nameEnd == null && argsBegin == null)
                {
                    nameEnd = -1;
                }
            }
            //



            // Get name.
            string name = statement.rng(0, (int)nameEnd);
            //



            // Find command and execute.
            if (Commands.TryGetValue(name.ToLower(), out Command cmd))
            {
                cmd.Execute(argsBegin == null ? "" : statement.rng((int)argsBegin, -1), this);
            }
            else
            {
                throw new MessageException($"Command not found: {name}");
            }
            //
        }


        public bool AddCommand(Command cmd)
        {
            return Commands.TryAdd(cmd.Name, cmd);
        }

        public bool AddCommands(IEnumerable<Command> commands)
        {
            bool fullSuccessfull = true;
            foreach (var cmd in commands)
            {
                if (!AddCommand(cmd)) { fullSuccessfull = false; }
            }
            return fullSuccessfull;
        }

        public void TakeLine(string statement)
        {
            // Remove comments.
            #region remove comments
            {
                char c;
                char? next;
                for (int i = 0; i < statement.Length; i++)
                {
                    c = statement[i];
                    next = i == statement.Length - 1 ? null : (char?)statement[i + 1];
                    if (commentOpFinder.Found(c, next))
                    {
                        //statement = statement.Substring(0, i - commentOperator.Length);// remove comment from statement.
                        statement = statement.rng(0, i);
                        commentOpFinder.Reset();// don't forget to reset the opfinder.
                        break;// no point in finnishing the loop.
                    }
                }
            }
            #endregion

            // Ignore if blank
            if (statement.Trim().Length == 0) { return; } // ignore blank lines and pure comments

            // Is this a command or an expression?
            if (statement.Length >= CommandOperator.Length && statement.Substring(0, CommandOperator.Length) == CommandOperator)
            {
                // This is a command.
                RunCommand(statement.rng(CommandOperator.Length, -1));
            }
            else
            {
                // Nothing else this can be, must be an expression.

                // Parse the expression
                IElement e = Parser.ParseElement(statement);

                // If the expression was null...
                if (e == null)
                {
                    PrintFunction(ParsingProps.VoidLabel + "\n");
                    return;
                }

                // Run the expression but don't execute it yet.
                IElement p = e.Pass(out bool dontExecute);
                double? ex = null;

                // Print the parsed expression to show the user what was inturpreted by the parser.
                PrintFunction(e.ToString(SymbolizedIndex, null) + "\n");

                // Print the expression after it has been ran.
                PrintFunction(p.NullableToString(ParsingProps.VoidLabel) + "\n");

                // Execute the expression unless the don't execuse flag was true.
                if (!dontExecute)
                {
                    // Execute
                    ex = p?.Execute();

                    // Print the value from the execution
                    PrintFunction($"\t{ex}\n");

                    // Set the previos answer variable to the new previouse answer.
                    ans_var.Definition = ex.ToElement();
                }

                // Decorative line break.
                PrintFunction("\n");
            }
        }

        #region vars and functions
        public Dictionary<(string name, int paramCount), Function> Functions { get; } = new Dictionary<(string name, int paramCount), Function>();
        public Dictionary<string, ILabeled> NamedItems { get; } = new Dictionary<string, ILabeled>();


        public bool AddFunction(Function function)
        {
            return Functions.TryAdd(function.Id, function);
        }
        public bool AddFunctions(IEnumerable<Function> functions)
        {
            bool fullSuccess = true;
            foreach (Function f in functions)
            {
                if (!AddFunction(f)) { fullSuccess = false; }
            }
            return fullSuccess;
        }


        public bool AddLabeled(IEnumerable<ILabeled> labeled)
        {
            bool success = true;
            foreach (ILabeled l in labeled)
            {
                if (!NamedItems.TryAdd(l.Name, l)) { success = false; }
            }
            return success;
        }
        public bool AddLabeled(ILabeled labeled)
        {
            return NamedItems.TryAdd(labeled.Name, labeled);
        }
        #endregion
    }
}
