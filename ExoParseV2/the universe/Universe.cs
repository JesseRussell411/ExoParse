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

        public Action<string> PrintFunction { get; set; }
        public Func<string> ReadFunction { get; set; }
        public Environment Environment { get; set; }
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
            int? nameEnd   = null;
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
            string name = statement.rng(0, (int) nameEnd);
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

        public Universe()
        {
            commentOpFinder = new SymbolFinder(CommentOperator);
            Commands = new Dictionary<string, Command>();
            Ans = new Constant("ans", ans_var);
        }

        public void TakeLine(string statement)
        {
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
            
            if (statement.Trim().Length == 0) { return; } // ignore blank lines and pure comments
            if (statement.Length >= CommandOperator.Length && statement.Substring(0, CommandOperator.Length) == CommandOperator)
            {
                // This is a command.
                RunCommand(statement.rng(CommandOperator.Length, -1));
            }
            else
            {
                // Nothing else this can be, must be an expression.
                IElement e = Parser.ParseElement(statement);
                if (e == null)
                {
                    PrintFunction(ParsingProps.VoidLabel + "\n");
                    return;
                }

                IElement p = e.Pass(out bool dontExecute);
                double? ex = null;
                PrintFunction(e.ToString(SymbolizedIndex, null) + "\n");
                PrintFunction(p.NullableToString(ParsingProps.VoidLabel) + "\n");
                if (!dontExecute) 
                {
                    ex = p?.Execute();
                    PrintFunction($"\t{ex}\n");
                    ans_var.Definition = ex.ToElement();
                }
                PrintFunction("\n");
            }
        }
    }
}
