using System;
using System.Numerics;

namespace MathTypes
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
            Console.WriteLine(i + 9);
            Console.WriteLine(i + 2.5);
            Console.WriteLine(i);

            Console.WriteLine(i - ulong.MaxValue);
            Console.WriteLine(8 * 2.5);
            Console.WriteLine(IntFloat.Pow(2,-2));

            Console.WriteLine(BigFraction.Parse("-1/5") > BigFraction.Parse("-1/5"));
            Console.WriteLine((uBigInteger)(-5));

            BigInteger big = -40;
            uBigInteger ubig = (uBigInteger)big;
            Console.WriteLine(ubig);
        }
    }
}
