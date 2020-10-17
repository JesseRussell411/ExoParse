using ExoParseV2.theUniverse.Commands;
using System;
using System.Collections.Generic;
using ParsingTools;
using System.Text;
using System.Runtime.CompilerServices;
using ConvenienceTools;
using ExoParseV2.elements;

namespace ExoParseV2.theUniverse
{
    public class Universe
    {
        public Universe()
        {
            CommentOperator = StringProps.CommentOperator;
            CommandOperator = StringProps.CommandOperator;
            Commands = new Dictionary<string, Command>();
            Ans = new Constant("ans", ans_var);
        }

        #region flags
        public bool Debug { get; set; } = false;
        #endregion
        #region io
        public Action<string> PrintFunction { get; set; }
        public Func<string> ReadFunction { get; set; }
        #endregion
        #region symbols
        public SymbolizedIndex SymbolizedIndex { get; set; }
        public Parser Parser { get; set; }
        /// <summary>
        /// The symbol to go before commands.
        /// </summary>
        public string CommandOperator { get; set; }
        /// <summary>
        /// The symbol to go before comments.
        /// </summary>
        public string CommentOperator
        {
            get { return commentOperator; }
            set
            {
                commentOperator = value;
                commentOpFinder = new SymbolFinder(CommentOperator);
            }
        }
        private string commentOperator;
        internal SymbolFinder commentOpFinder;
        #endregion
        #region environment
        #region vars and functions
        public Dictionary<(string name, int paramCount), Function> Functions { get; } = new Dictionary<(string name, int paramCount), Function>();
        public Dictionary<string, IReference> NamedItems { get; } = new Dictionary<string, IReference>();


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


        public bool AddLabeled(IEnumerable<IReference> labeled)
        {
            bool success = true;
            foreach (IReference l in labeled)
            {
                if (!NamedItems.TryAdd(l.Name, l)) { success = false; }
            }
            return success;
        }
        public bool AddLabeled(IReference labeled)
        {
            return NamedItems.TryAdd(labeled.Name, labeled);
        }
        #endregion
        public Dictionary<string, Command> Commands { get; }
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
        #endregion
        #region ans
        public Variable ans_var { get; } = new Variable("previousAnswer");
        public Constant Ans { get; }
        public double? PreviousAnswer
        {
            get
            {
                return ans_var.Execute();
            }
            set
            {
                ans_var.Definition = value.ToElement();
            }
        }
        #endregion

        


        public IElement ParseLine(string statement)
        {
            // Ignore if null
            if (statement == null) { return null; }

            // Ignore if blank
            if (statement.Trim().Length == 0) { return null; }

            // Parse the expression
            return Parser.ParseElement(statement);
        }

        //public IElement ParseLine(string statement)
        //{
        //    // Ignore if null
        //    if (statement == null) { return; }

        //    // Remove comments.
        //    #region remove comments
        //    {
        //        char c;
        //        char? next;
        //        for (int i = 0; i < statement.Length; i++)
        //        {
        //            c = statement[i];
        //            next = i == statement.Length - 1 ? null : (char?)statement[i + 1];
        //            if (commentOpFinder.Found(c, next))
        //            {
        //                //statement = statement.Substring(0, i - commentOperator.Length);// remove comment from statement.
        //                statement = statement.rng(0, i);
        //                commentOpFinder.Reset();// don't forget to reset the opfinder.
        //                break;// no point in finnishing the loop.
        //            }
        //        }
        //    }
        //    #endregion

        //    // Ignore if blank
        //    if (statement.Trim().Length == 0) { return; } // ignore blank lines and pure comments

        //    // Is this a command or an expression?
        //    if (statement.Length >= CommandOperator.Length && statement.Substring(0, CommandOperator.Length) == CommandOperator)
        //    {
        //        // This is a command.
        //        RunCommand(statement.rng(CommandOperator.Length, -1));
        //    }
        //    else
        //    {
        //        // Nothing else this can be, must be an expression.

        //        // Parse the expression
        //        IElement e = Parser.ParseElement(statement);

        //        // If the expression was null...
        //        if (e == null)
        //        {
        //            PrintFunction(StringProps.VoidLabel + "\n");
        //            return;
        //        }

        //        // Run the expression but don't execute it yet.
        //        IElement p = e.Pass(out bool dontExecute);
        //        double? ex = null;

        //        // Print the parsed expression to show the user what was inturpreted by the parser.
        //        PrintFunction(e.ToString(SymbolizedIndex, null) + "\n");

        //        // Print the expression after it has been ran.
        //        PrintFunction(p.NullableToString(StringProps.VoidLabel) + "\n");

        //        // Execute the expression unless the don't execute flag was true.
        //        if (!dontExecute)
        //        {
        //            // Execute
        //            ex = p?.Execute();

        //            // Print the value from the execution
        //            PrintFunction($"\t{ex}\n");

        //            // Set the previos answer variable to the new previouse answer.
        //            ans_var.Definition = ex.ToElement();
        //        }

        //        // Decorative line break.
        //        PrintFunction("\n");
        //    }
        //}

    }
}
