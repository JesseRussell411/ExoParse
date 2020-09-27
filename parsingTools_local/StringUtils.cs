using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ParsingTools
{
    public static class StringUtils
    {
        public static readonly string[] Em = new String[0];
        public static String ToDelimString<T>(this IEnumerable<T> self)
        {
            return self.ToDelimString(ParsingUtils.Delims_stan.FirstOrDefault() ?? ", ");
        }
        public static String ToDelimString<T>(this IEnumerable<T> self, String delim)
        {
            StringBuilder builder = new StringBuilder();
            var enumer = self.Select(o => o.ToString()).GetEnumerator();
            enumer.MoveNext();
            builder.Append(enumer.Current);

            while (enumer.MoveNext())
            {
                builder.Append(delim);
                builder.Append(enumer.Current);
            }

            return builder.ToString();
        }
        public static String NullableToString<T>(this T self, string nullReplacement)
        {
            return self == null ? nullReplacement : self.ToString();
        }

        public static String Wrap(this String text, String openingBracket, String closingBracket)
        {
            return $"{openingBracket}{text}{closingBracket}";
        }

        public static String Wrap(this String text)
        {
            return text.Wrap(ParsingUtils.OpenBrackets_stan[0], ParsingUtils.CloseBrackets_stan[0]);
        }
    }
}
