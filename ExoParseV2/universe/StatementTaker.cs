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
    public class StatementTaker
    {
        public StatementTaker(Universe universe)
        {
            Universe = universe;
        }

        private StringBuilder escapedLine = null;


        public string LineBreakEscapeSymbol { get; set; } = "\\";

        private Universe un;
        public Universe Universe { get => un; set => un = value; }

        private string RunCommand(string statement)
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
                // Return the result string from execution.
                return cmd.Execute(statement.rng((int)argsBegin, statement.Length), un);
            }
            else
            {
                throw new MessageException($"Command not found: {name}");
            }
            //
        }

        public string RunStatement(string statement)
        {
            // Ignore if null
            if (statement == null) { return ""; }

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
                        statement = statement.rng(0, (i - un.CommentOperator.Length) + 1);
                        un.commentOpFinder.Reset();// don't forget to reset the opfinder.
                        break;// no point in finishing the loop.
                    }
                }
            }
            #endregion

            // Ignore if blank.
            if (statement.Trim().Length == 0) { return ""; } // ignore blank lines and pure comments

            #region escaped line
            // Escape line:
            if (statement.Length >= LineBreakEscapeSymbol.Length && statement.Substring(statement.Length - LineBreakEscapeSymbol.Length, LineBreakEscapeSymbol.Length) == LineBreakEscapeSymbol)
            {
                // Create new escaped line if necessary.
                if (escapedLine == null) { escapedLine = new StringBuilder(); }

                // Append to escaped line.
                escapedLine.Append(statement.Substring(0, statement.Length - LineBreakEscapeSymbol.Length));

                // Return decorative symbol.
                return $"{LineBreakEscapeSymbol} ";
            }
            //

            // Assemble escaped line:
            if (escapedLine != null)
            {
                statement = escapedLine.Append(statement).ToString();

                // Reset escaped line.
                escapedLine = null;
            }
            //
            #endregion

            // Is this a command or an expression?
            if (statement.Length >= un.CommandOperator.Length && statement.Substring(0, un.CommandOperator.Length) == un.CommandOperator)
            {
                // This is a command.
                return RunCommand(statement.rng(un.CommandOperator.Length, -1));
            }
            else
            {
                // Nothing else this can be, must be an expression.

                // Parse the expression
                IElement e = un.Parser.ParseElement(statement);

                // If the expression was null:
                if (e == null)
                {
                    // Return void for result string.
                    return $"{StringProps.VoidLabel}\n";
                }
                //

                // Pass the expression but don't execute it yet.
                IElement p = e.Pass(out bool dontExecute);
                double? ex = null;


                // Start building result string;
                StringBuilder result = new StringBuilder();

                // Append the parsed expression to show the user what was interpreted by the parser.
                result.Append($"{e.ToString(un.SymbolizedIndex)}\n");

                // Append the expression after it has been ran.
                result.Append($"{p.ToString(un.SymbolizedIndex)}\n");

                // Execute the expression unless the don't-execute flag was true.
                if (!dontExecute)
                {
                    // Execute
                    ex = p?.Execute();

                    // Append the value from the execution
                    result.Append($"\t{ex.ElementExecuteToString()}\n");

                    // Set the previous answer variable to the new previous answer.
                    un.PreviousAnswer= ex;
                }

                // Return the result string.
                return result.ToString();
            }

            return "";
        }
    }
}
