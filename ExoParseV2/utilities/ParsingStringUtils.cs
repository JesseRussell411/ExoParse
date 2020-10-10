using ParsingTools;
using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.elements;

namespace ExoParseV2.utilities
{
    public static class ParsingStringUtils
    {
        public static string NullableToString(this object self)
        {
            return self?.ToString() ?? StringProps.VoidLabel;
        }

        public static string VoidableToString(this IElement self, SymbolizedIndex si, IExpressionComponent parent)
        {
            return self?.ToString(si, parent) ?? StringProps.VoidLabel;
        }

        public static string VoidableToString(this object self)
        {
            return self?.ToString() ?? StringProps.VoidLabel;
        }
    }
}
