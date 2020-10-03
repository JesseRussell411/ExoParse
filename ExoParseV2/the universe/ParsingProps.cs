using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public static class ParsingProps
    {
        public static readonly string[] OpenBrackets = { "(", "[", "{" };
        public static readonly string[] CloseBrackets = { ")", "]", "}" };
        public static readonly string[] Barriers = { "\"" };
        public static readonly string[] Delims = { "," };
        public static readonly string   NullLabel = "null";
        public static readonly string   VoidLabel = "void";
    }
}
