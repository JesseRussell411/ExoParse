using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvenienceTools
{
    public static class Cools
    {
        public static T[] MakeArray<T>(this T item) => new T[] { item };
        public static List<T> MakeList<T>(this T item) => new List<T> { item };

        public static T[] MakeArray<T>(params T[] items) => items;

        public static List<T> MakeList<T>(params T[] items) => items.ToList();

        public static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);

        public static int ToInt(this bool self) => self ? 1 : 0;
        public static bool ToBool(this int self) => self != 0;
        public static bool ToBool(this uint self) => self != 0;
        public static bool ToBool(this long self) => self != 0;
        public static bool ToBool(this ulong self) => self != 0;
        


        public static readonly string[] Ems = new string[0];
        public static readonly int[] Emi = new int[0];
        public static readonly bool[] Emb = new bool[0];
        public static readonly double[] Emd = new double[0];
        public static readonly List<string> Lems = new List<string>();
        public static readonly List<int> Lemi = new List<int>();
        public static readonly List<bool> Lemb = new List<bool>();
        public static readonly List<double> Lemd = new List<double>();
        public static bool InBounds<T>(this T[] arr, int index) => index < arr.Length && index >= 0;
        public static bool InBounds<T>(this List<T> arr, int index) => index < arr.Count && index >= 0;
        public static bool InBounds(this string arr, int index) => index < arr.Length && index >= 0;
    }
}
