using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoParsev1
{

    public static class ToStringUtils
    {
        public static string ToDelimString<T>(this T enumerable, string delim) where T : IEnumerable
        {
            StringBuilder builder = new StringBuilder();

            var enu = enumerable.GetEnumerator();
            if (enu.MoveNext()) { builder.Append(enu.Current?.ToString()); } // "bob
            while (enu.MoveNext())
            {
                builder.Append(delim); // "bob, 
                builder.Append(enu.Current.ToString()); // "bob, bob
            }
            // "bob, bob, bob, bob... bob"

            return builder.ToString();
        }
        public static string NullableToString(this object self)
        {
            return self.NullableToString("null");
        }
        public static string NullableToString(this object self, string nullReplacementString)
        {
            if (self == null)
            {
                return nullReplacementString;
            }
            else
            {
                string self_ToString = self.ToString();
                return self_ToString.Length == 0 ? nullReplacementString : self_ToString;
            }
        }
        public static string FuncIdToString(this (string name, int argCount) self)
        {
            return GetFunctionIdString(self);
        }
        public static string GetFunctionIdString((string name, int argCount) functionId)
        {
            return $"{functionId.name}[{functionId.argCount}]";
        }
    }

    public static class MiscUtils
    {
        /// <summary>
        /// Returns an array of length 1 with the item inside.
        /// 
        /// Shorthand for: new T[] { item }
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns> Array of length 1 with the item inside. </returns>
        public static T[] Ta<T>(this T item)
        {
            return new T[1] { item };
        }

        /// <summary>
        /// Returns a list of length 1 with the item inside.
        /// 
        /// Shorthand for: new List<T>() { item }
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns> Array of length 1 with the item inside. </returns>
        public static List<T> Tl<T>(this T item)
        {
            return new List<T>() { item };
        }
    }


}
