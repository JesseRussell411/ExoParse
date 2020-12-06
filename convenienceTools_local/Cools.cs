//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.PortableExecutable;
//using System.Text;

//namespace ConvenienceTools
//{
//    public static class Cools
//    {
//        /// <summary>
//        /// Convert a single item to an new array containing only that item.
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public static T[] Mkarr<T>(this T item) => new T[] { item };
//        /// <summary>
//        /// Convert a single item to a new list containing only that item.
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public static List<T> Mklst<T>(this T item) => new List<T> { item };

//        public static T[] Mkarr<T>(params T[] items) => items;

//        public static List<T> Mklst<T>(params T[] items) => items.ToList();

//        public static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);

//        public static int ToInt(this bool self) => self ? 1 : 0;
//        public static bool ToBool(this int self) => self != 0;
//        public static bool ToBool(this uint self) => self != 0;
//        public static bool ToBool(this long self) => self != 0;
//        public static bool ToBool(this ulong self) => self != 0;
        
//        public static string[] Ems { get { return new string[0]; } }
//        public static int[] Emi { get { return new int[0]; } }
//        public static bool[] Emb { get { return new bool[0]; } }
//        public static double[] Emd { get { return new double[0]; } }
//        public static List<string> Lems { get { return new List<string>(); } }
//        public static List<int> Lemi { get { return new List<int>(); } }
//        public static List<bool> Lemb { get { return new List<bool>(); } }
//        public static List<double> Lemd { get { return new List<double>(); } }

//        public static bool InBounds<T>(this T[] arr, int index) => index < arr.Length && index >= 0;
//        public static bool InBounds<T>(this List<T> arr, int index) => index < arr.Count && index >= 0;
//        public static bool InBounds(this string arr, int index) => index < arr.Length && index >= 0;

//        /// <summary>
//        /// Returns a substring contained by start inclusively and end exclusively. Entering -1 for end represents the string's length.
//        /// </summary>
//        /// <param name="s"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        public static string rng(this string s, int start, int end)
//        {
//            if (end < 0 || end > s.Length) { end = s.Length; }
//            if (start < 0) { start = 0; }


//            // Panic and give up.
//            if (s.Length == 0) { return ""; }
//            if (start > end) { return ""; }
//            //


//            return s.Substring(start, end - start);
//        }
//    }
//}
