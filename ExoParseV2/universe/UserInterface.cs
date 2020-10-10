using ExoParseV2.theUniverse.Commands;
using System;
using System.Collections.Generic;
using ParsingTools;
using System.Text;
using System.Runtime.CompilerServices;
using ConvenienceTools;
using ExoParseV2.elements;
using ExoParseV2.theUniverse;

namespace ExoParseV2.universe
{
    class UserInterface
    {
        public UserInterface(Universe universe)
        {
            Universe = universe;
        }
        public Action<string> Print { get; set; } = Console.Write;
        private void print(object s) { Print(s.ToString()); }
        private void print() { Print(""); }
        private void println(object s) { Print(s.ToString() + "\n"); }
        private void println() { Print("" + "\n"); }

        public Func<string> Readln { get; set; } = Console.ReadLine;
        private string      readln() { return Readln(); }


        private Universe un;

        public Universe Universe { get => un; set => un = value; }
        private void RunCommand(string statement)
        {
            int? nameEnd = null;
            int? argsBegin = null;
            if (statement.Trim().Length == 0)
            {
                throw new MessageException("A command was expected but no command was given.");
            }



            // Find name and args:
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

                if (nameEnd == null)
                {
                    nameEnd = statement.Length;
                }

                if (argsBegin == null)
                {
                    argsBegin = nameEnd;
                }
            }
            //




            // Get name.
            string name = statement.rng(0, (int)nameEnd);
            //



            // Find command and execute.
            if (un.Commands.TryGetValue(name.ToLower(), out Command cmd))
            {
                cmd.Execute(statement.rng((int)argsBegin, statement.Length), un);
            }
            else
            {
                throw new MessageException($"Command not found: {name}");
            }
            //

            println();
        }
        public void RunLine(string statement)
        {
            // Ignore if null
            if (statement == null) { return; }

            // Remove comments.
            #region remove comments
            {
                char c;
                char? next;
                for (int i = 0; i < statement.Length; i++)
                {
                    c = statement[i];
                    next = i == statement.Length - 1 ? null : (char?)statement[i + 1];
                    if (un.commentOpFinder.Found(c, next))
                    {
                        //statement = statement.Substring(0, i - commentOperator.Length);// remove comment from statement.
                        statement = statement.rng(0, i);
                        un.commentOpFinder.Reset();// don't forget to reset the opfinder.
                        break;// no point in finnishing the loop.
                    }
                }
            }
            #endregion

            // Ignore if blank
            if (statement.Trim().Length == 0) { return; } // ignore blank lines and pure comments

            // Is this a command or an expression?
            if (statement.Length >= un.CommandOperator.Length && statement.Substring(0, un.CommandOperator.Length) == un.CommandOperator)
            {
                // This is a command.
                RunCommand(statement.rng(un.CommandOperator.Length, -1));
            }
            else
            {
                // Nothing else this can be, must be an expression.

                // Parse the expression
                IElement e = un.Parser.ParseElement(statement);

                // If the expression was null...
                if (e == null)
                {
                    println(StringProps.VoidLabel);
                    return;
                }

                // Run the expression but don't execute it yet.
                IElement p = e.Pass(out bool dontExecute);
                double? ex = null;

                // Print the parsed expression to show the user what was inturpreted by the parser.
                println(e.ToString(un.SymbolizedIndex));

                // Print the expression after it has been ran.
                println(p.ToString(un.SymbolizedIndex));

                // Execute the expression unless the don't-execuse flag was true.
                if (!dontExecute)
                {
                    // Execute
                    ex = p?.Execute();

                    // Print the value from the execution
                    println($"\t{ex.ElementExecuteToString()}");

                    // Set the previos answer variable to the new previouse answer.
                    un.PreviouseAnswer= ex;
                }

                // Decorative line break.
                println();
            }
        }
    }
}
