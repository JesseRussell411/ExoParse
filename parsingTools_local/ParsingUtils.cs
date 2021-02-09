using System;
using System.Collections.Generic;
using System.Linq;


namespace ParsingTools
{
    public static class ParsingUtils
    {
        // Standard Symbols
        public static readonly string[] OpenBrackets_stan =  { "(", "[", "{" };
        public static readonly string[] CloseBrackets_stan = { ")", "]", "}" };
        public static readonly string[] Barriers_stan =      { "\"" };
        public static readonly string[] Delims_stan =        { "," };
        public static readonly string[] Separators_stan =    { };
        public static readonly string[] PreBreaks_stan =     { };
        public static readonly string[] PostBreaks_stan =    { };
        //
        

        #region wrapping
        #region IsWrapped
        public static bool IsWrapped(this String text)
        {
            return text.IsWrapped(OpenBrackets_stan, CloseBrackets_stan);
        }

        public static bool IsWrapped(this String text, out String openingBracket, out String closingBracket)
        {
            return text.IsWrapped(OpenBrackets_stan, CloseBrackets_stan, out openingBracket, out closingBracket);
        }

        public static bool IsWrapped(this String text, String openBracket, String closeBracket)
        {
            return text.IsWrapped(new String[] { openBracket }, new String[] { closeBracket });
        }

        public static bool IsWrapped(this String text, String openBracket, String closeBracket, out String openingBracket, out String closingBracket)
        {
            return text.IsWrapped(new String[] { openBracket }, new String[] { closeBracket }, out openingBracket, out closingBracket);
        }

        public static bool IsWrapped(this String text, IEnumerable<String> openBrackets, IEnumerable<String> closeBrackets)
        {
            String trash0;
            String trash1;
            return text.IsWrapped(openBrackets, closeBrackets, out trash0, out trash1);
        }

        public static bool IsWrapped(this String text, IEnumerable<String> openBrackets, IEnumerable<String> closeBrackets, out String openingBracket, out String closingBracket)
        {
            if (text.Length < 2) 
            {
                openingBracket = null;
                closingBracket = null;
                return false;
            }
            
            int stage = 1;

            openingBracket = null;
                             
            closingBracket = null;


            HashSet<string> openSet = openBrackets.ToHashSet();



            int depth = 0;

            SymbolFinder finder = new SymbolFinder(openBrackets.Union(closeBrackets));

            string symb = "";
            int text_lastIndex = text.Length - 1;


            char c;
            char? next;
            for(int i = 0; i < text.Length; i++)
            {
                c = text[i];
                next = i == text_lastIndex ? null : (char?) text[i + 1];
                symb = finder.Find(c, next);

                if (stage == 1)
                {
                    if (openSet.Contains(symb))
                    {
                        if (symb.Length - 1 != i) { return false; }

                        // what this block does:
                        //  - sets openingBracket and closingBracket to their apropriate values.
                        //  - increments stage to 2
                        //  - increments depth


                        // get index of openBracket and set openingBracket
                        int index = 0;
                        foreach (String bracket in openBrackets)
                        {
                            if (bracket == symb)
                            {
                                openingBracket = bracket;
                                break;
                            }
                            index++;
                        }
                        //


                        // set closing bracket
                        try
                        {
                            closingBracket = closeBrackets.ElementAt(index);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            throw new BracketDisparityException();
                        }


                        depth++;
                        stage = 2;
                    }
                }
                else
                {
                    if (openingBracket == symb) { depth++; }
                    if (closingBracket == symb) { depth--; }

                    if (i == text_lastIndex)
                    {
                        return depth == 0;
                    }
                    else if (depth == 0)
                    {
                        openingBracket = "";
                        closingBracket = "";
                        return false;
                    }
                }
            }

            openingBracket = "";
            closingBracket = "";
            return false;
        }
        #endregion

        #region UnWrap
        public static String UnWrap(this String text)
        {
            return text.UnWrap(OpenBrackets_stan, CloseBrackets_stan);
        }

        public static String UnWrap(this String text, String openBracket, String closeBracket)
        {
            return text.UnWrap(new String[] {openBracket}, new String[] { closeBracket });
        }

        public static String UnWrap(this String text, IEnumerable<String> openBrackets, IEnumerable<String> closeBrackets)
        {
            String openingBracket;
            String closingBracket;
            if (text.IsWrapped(openBrackets, closeBrackets, out openingBracket, out closingBracket))
            {
                int startIndex = openingBracket.Length;
                int endIndex = text.Length - closingBracket.Length;
                int length = (endIndex - startIndex);

                return text.Substring(startIndex, length);
            }
            else
            {
                return text.Substring(0, text.Length);
            }
        }
        #endregion

        #endregion
    }
}
