using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
//#define TOKENIZE_UNDER_CONSTRUCTION


namespace ParsingTools
{
    public class Tokenizer
    {
        // Symbol types (different things to break on).
        private List<String> separators; // "a|b" -> "a", "|", "b"
        private List<String> delims;     // "a|b" -> "a", "b"
        private List<String> preBreaks;  // "a|b" -> "a", "|b"
        private List<String> postBreaks; // "a|b" -> "a|", "b"
        //


        
        // Categorizers(brackets(...) and barriers"..."), These make the tokenizer skip symbols that are found in their domain: 2 + (1 + 3) -> "2 ","+", " (1 + 3), notice the + inside the parens was ignored.
        private List<String> openBrackets; // (
        private List<String> closeBrackets;// )
        private List<String> barriers;   // ""
        private List<String> barriersToBreak; // break on barriers?
        //

        private SymbolFinder finder;


        // Switch defaults
        public bool BreakOnWhiteSpace { get; set; } = false;
        public bool TrimTokens { get; set; } = false;
        public bool IncludeEmpty { get; set; } = false;
        //

        public Tokenizer(bool includeEmpty = true)
            : this(new String[0], new String[0], new String[0], new String[0], new String[0], new String[0], new String[0], new String[0])
        {
            BreakOnWhiteSpace = true;
            TrimTokens = false;
            IncludeEmpty = includeEmpty;
        }
        public Tokenizer(
            IEnumerable<string> separators,
            IEnumerable<string> delims,
            IEnumerable<string> preBreaks,
            IEnumerable<string> postBreaks,
            IEnumerable<string> openBrackets,
            IEnumerable<string> closeBrackets,
            IEnumerable<string> barriers,
            IEnumerable<string> barriersToBreak,
            bool breakOnBrackets = false
            )
        {
            this.separators =    separators.ToList();
            this.delims =        delims.ToList();

            this.preBreaks =     breakOnBrackets ?  preBreaks.Union(openBrackets).ToList( ) : preBreaks.ToList();
            this.postBreaks =    breakOnBrackets ? postBreaks.Union(closeBrackets).ToList() : preBreaks.ToList();

            this.openBrackets =  openBrackets.ToList();
            this.closeBrackets = closeBrackets.ToList();

            if (this.openBrackets.Count != this.closeBrackets.Count)
            {
                throw new BracketDisparityException();
            }

            this.barriers =      barriers.ToList();
            this.barriersToBreak = barriersToBreak.ToList();

            finder = new SymbolFinder(
                separators
                .Union(delims)
                .Union(preBreaks)
                .Union(postBreaks)
                .Union(openBrackets)
                .Union(closeBrackets)
                .Union(barriers)
                );
        }

        public List<string> Tokenize(String text)
        {
#if TOKENIZE_UNDER_CONSTRUCTION
            throw new NotImplementedException();
#endif
            List<string> tokens = new List<string>();

            int[] depth = new int[openBrackets.Count];
            bool[] inBarrier = new bool[barriers.Count];
            StringBuilder token = new StringBuilder();

            #region function definitions
            Func<String, String> MaybeTrim;

            if (TrimTokens)
            {
                MaybeTrim = s => s.Trim();
            }
            else
            {
                MaybeTrim = s => s;
            }

            void BreakToken_KeepEmpty()
            {
                tokens.Add(MaybeTrim(token.ToString()));
                token.Clear();
            }

            void BreakToken_DiscardEmpty()
            {
                String tokenString = MaybeTrim(token.ToString());
                
                if (tokenString.Length > 0)
                {
                    tokens.Add(tokenString);
                }
                token.Clear();
            }

            Action BreakToken = IncludeEmpty ? (Action)BreakToken_KeepEmpty : (Action)BreakToken_DiscardEmpty;
            #endregion

            finder.Reset();
            char c;
            char? next;
            int text_lastIndex = text.Length - 1;
            string symb;
            for (int i = 0; i < text.Length; i++)
            {
                c = text[i];
                next = i == text_lastIndex ? null : (char?)text[i + 1];
                symb = finder.Find(c, next);

                token.Append(c);

                #region handle brackets(...) and barriers"..."
                bool? barrier_opening_closing = null;
                bool engaged = true;
                for(int j = 0; j < openBrackets.Count; j++)
                {

                    if (closeBrackets[j] == symb) { depth[j]--; }
                    if (depth[j] != 0) { engaged = false; }
                    if (openBrackets[j] == symb) { depth[j]++; }
                }

                for (int j = 0; j < barriers.Count; j++)
                {
                    bool openingBarrier = false;
                    if (barriers[j] == symb)
                    {
                        if (inBarrier[j])
                        {
                            inBarrier[j] = false;
                            barrier_opening_closing = false;
                        }
                        else
                        {
                            inBarrier[j] = true;
                            openingBarrier = true;
                            barrier_opening_closing = true;
                        }
                    }
                    if (inBarrier[j] && !openingBarrier) { engaged = false; }
                }
                #endregion


                if (engaged)
                {
                    if (symb.Length > 0)
                    {
                        if (separators.Contains(symb))
                        {
                            // Remove the separator from the text to be added to the array.
                            token.Remove(token.Length - symb.Length, symb.Length);

                            // Add to array.
                            BreakToken();

                            // Add the separator.
                            tokens.Add(symb);

                            // Clear the stringBuilder for the next cycle.
                            token.Clear();
                        }
                        else if (preBreaks.Contains(symb))
                        {
                            // Remove the separator from the text to be added to the array.
                            token.Remove(token.Length - symb.Length, symb.Length);

                            // Add to array.
                            BreakToken();

                            // Clear the stringBuilder for the next cycle.
                            token.Clear();

                            // Add the preBreak seperator to the next cycle.
                            token.Append(symb);
                        }
                        else if (postBreaks.Contains(symb))
                        {
                            // Add to array.
                            BreakToken();

                            // Clear the stringBuilder for the next cycle.
                            token.Clear();
                        }
                        else if (delims.Contains(symb))
                        {
                            // Remove the separator from the text to be added to the array.
                            token.Remove(token.Length - symb.Length, symb.Length);

                            // Add to array.
                            BreakToken();

                            // Clear the stringBuilder for the next cycle.
                            token.Clear();
                        }
                        else if (barrier_opening_closing == true && barriersToBreak.Contains(symb))
                        {
                            // Remove the barrier from the text to be added to the array.
                            token.Remove(token.Length - symb.Length, symb.Length);

                            // Add to array.
                            BreakToken();

                            // Clear the stringBuilder for the next cycle.
                            token.Clear();

                            // Add the preBreak seperator to the next cycle.
                            token.Append(symb);
                        }
                        else if (barrier_opening_closing == false && barriersToBreak.Contains(symb))
                        {
                            // Add to array.
                            BreakToken();

                            // Clear the stringBuilder for the next cycle.
                            token.Clear();
                        }
                    }
                    else if (BreakOnWhiteSpace && char.IsWhiteSpace(c))
                    {
                        // Remove the single whitespace character from the stringBuilder.
                        token.Remove(token.Length - 1, 1);

                        // Skip the reset of the contiguous whitespace.
                        i++;
                        while (i < text.Length && char.IsWhiteSpace(text[i])) { i++; }
                        i--;
                        finder.Reset();


                        // Add to array.
                        BreakToken();

                        // Clear the stringBuilder for the next cycle.
                        token.Clear();
                    }
                }
            }
            BreakToken();
            return tokens;
        }
    }
}
