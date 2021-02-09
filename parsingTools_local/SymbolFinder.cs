using System.Collections.Generic;
using System.Linq;

namespace ParsingTools
{
    /// <summary>
    /// This class is capable of finding multi-character symbols in strings. example: "a := x + 7" -> ":=", "+".
    /// This is done using the find method. The standard use is in a for loop, looping through a string such that the currect character and the next character are provided.
    /// Like this:
    /// 
    /// SymbolFinder finder = new SymbolFinder(ARRAY OF SYMBOLS TO FIND);
    /// 
    /// char c;
    /// char? next;
    /// int text_lastIndex = text.Length - 1
    /// string symb;
    /// for(int i = 0; i < text.Length; i++)
    /// {
    ///     c = text[i];
    ///     next = i == text_lastIndex ? null : (char?) text[i + 1];
    ///     
    ///     symb = finder.Find(c, next);
    /// }
    /// </summary>
    public class SymbolFinder
    {
        public SymbolFinder(IEnumerable<string> symbols)
        {
            this.symbols = symbols.ToArray();
            //first build of possible
            for (int i = 0; i < this.symbols.Length; i++) { possible.Add(i); }
        }
        public SymbolFinder(string symbol)
            : this(new string[] { symbol })
        { }
        public SymbolFinder()
            : this(new string[0])
        { }
        public void Reset()
        {
            opIndex = 0;
            //rebuild possible
            possible.Clear();
            for (int i = 0; i < symbols.Length; i++) { possible.Add(i); }
        }

        /// <summary>
        /// Returns the found symbol or an empty string if no symbol was found.
        /// </summary>
        /// <returns></returns>
        public string Find(char c, char? next)
        {
            possiblyFound = -1;
            for (int i = possible.Count - 1; i >= 0; i--)
            {
                string op = symbols[possible[i]];
                if (op.Length <= opIndex
                    || op[opIndex] != c)
                {
                    possible.RemoveAt(i);
                }
                else if (op.Length == opIndex + 1)
                {
                    possiblyFound = possible[i];
                }
            }

            if (!possible.Any())
            {
                //reset
                opIndex = 0;
                //rebuild possible
                for (int i = 0; i < symbols.Length; i++) { possible.Add(i); }
                //
                //

                //skip the rest
                return "";
            }

            nextPossible = false;
            foreach (int p in possible)
            {
                string op = symbols[p];

                // Will this operator still be possible with the next character?
                if (op.Length > (opIndex + 1)
                    && op[opIndex + 1] == next)
                {
                    nextPossible = true;
                    break;
                }
            }

            if (!nextPossible)
            {
                //reset
                opIndex = 0;
                possible.Clear();
                //rebuild possible
                for (int i = 0; i < symbols.Length; i++) { possible.Add(i); }
                //
                //
                if (possiblyFound == -1)
                {
                    return "";
                }
                else
                {
                    return symbols[possiblyFound];
                }
            }
            else
            {
                opIndex++;
                return "";
            }
        }
        /// <summary>
        /// Returns true if a symbol was found.
        /// </summary>
        /// <param name="c">Current character.</param>
        /// <param name="next">Last character.</param>
        /// <param name="symbol">Returns the found symbol or an empty string if no symbol was found.</param>
        public bool Found(char c, char? next, out string symbol)
        {
            symbol = Find(c, next);
            return symbol.Length != 0;
        }
        /// <summary>
        /// Returns true if a symbol was found.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public bool Found(char c, char? next)
        {
            return Find(c, next).Length != 0;
        }

        #region hidden
        private List<int> possible = new List<int>();
        private int possiblyFound = -1;
        private bool nextPossible = false;
        private int opIndex = 0;
        private string[] symbols;
        #endregion
    }
}
