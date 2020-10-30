using System;
using System.Numerics;

namespace IntegerFloatingPoint
{
    class Program
    {
        static void Main(string[] args)
        {
            IntFloat i = IntFloat.Parse("5325436262445243532454325243524352345234525234543365234598989898989898098098098098090903245435435");
            IntFloat f = IntFloat.Parse("-4e-9");
            IntFloat? n = null;

            Console.WriteLine(i);
            Console.WriteLine(f);
            Console.WriteLine(i.Add(9));
            Console.WriteLine(i.Add(2.5));
            Console.WriteLine(i);

            Console.WriteLine(i - ulong.MaxValue);
            Console.WriteLine(8 * 2.5);
            Console.WriteLine(new IntFloat(2).Raise(-2));
        }
    }
}
