using ParsingTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2.utilities
{
    public static class ParsingStringUtils
    {
        public static string NullableToString(this object self)
        {
            return self?.ToString() ?? ParsingProps.VoidLabel;
        }

        public static string VoidableToString(this IElement self, SymbolizedIndex si, IExpressionComponent parent)
        {
            return self?.ToString(si, parent) ?? ParsingProps.VoidLabel;
        }

        public static string VoidableToString(this object self)
        {
            return self?.ToString() ?? ParsingProps.VoidLabel;
        }
    }
}
