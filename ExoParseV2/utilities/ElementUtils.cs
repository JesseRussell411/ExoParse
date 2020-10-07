using ParsingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoParseV2
{
    public static class ElementUtils
    {
        public static IElement NullElement { get { return IElement.Null; } }
        public static IElement VoidElement { get { return IElement.Void; } }

        #region conversion
        public static IElement ToElement(this double? self)
        {
            return new Literal(self);
        }
        public static IElement ToElement(this double self)
        {
            return new Literal(self);
        }

        public static IElement ToElement(this int self)
        {
            return new Literal(self);
        }

        public static IElement ToElement(this int? self)
        {
            return new Literal(self);
        }

        public static IElement ToElement(this long self)
        {
            return new Literal(self);
        }

        public static IElement ToElement(this long? self)
        {
            return new Literal(self);
        }

        public static IElement ToElement(this bool? self)
        {
            return new Literal(
                self.ToDouble());
        }
        public static IElement ToElement(this bool self)
        {
            return new Literal(
                self.ToDouble());
        }
        
        #endregion

        public static IElement TrySetDefinition(this IElement self, IElement other)
        {
            return self.TrySetDefinition(other, d => d);
        }
        public static IElement TrySetDefinition(this IElement self, IElement other, Func<IElement, IElement> mod)
        {
            return self.TrySetDefinition(other, (s, d) => mod(d));
        }
        public static IElement TryChangeDefinition(this IElement self, Func<IElement, IElement> change)
        {
            return self.TrySetDefinition(null, (self, other) => change(self));
        }
        public static IElement TrySetDefinition(this IElement self, IElement other, Func<IElement, IElement, IElement> mod)
        {
            if (self is IRedefinable)
            {

                return ((IRedefinable)self).Definition = mod(self, other);
            }
            else
            {
                throw new NotRedefinableException(self);
            }
        }
        public static string ToDelimString(this IEnumerable<IElement> self, SymbolizedIndex si)
        {
            return self.ToDelimString(si, (IExpressionComponent)null);
        }
        public static string ToDelimString(this IEnumerable<IElement> self, SymbolizedIndex si, IExpressionComponent parent)
        {
            return self.ToDelimString(si, parent, StringProps.Delims[0]);
        }
        public static string ToDelimString(this IEnumerable<IElement> self, SymbolizedIndex si, string delim)
        {
            return self.ToDelimString(si, null, delim);
        }
        public static string ToDelimString(this IEnumerable<IElement> self, SymbolizedIndex si, IExpressionComponent parent, string delim)
        {
            return self.Select(s => s.ToString(si, parent)).ToDelimString(delim);
        }
    }
}
