using System;
using System.Linq;
using ConvenienceTools;

namespace Cutls
{
    class Test
    {
        public static void Main(string[] args)
        {
            var arr = Cools.MakeArray(1,32,4,3,5,6);
            var bob = 1.MakeArray();
            Console.WriteLine(
                arr.Select(i => i.ToString()).Aggregate((a, b) => $"{a}, {b}")
                );
        }
    }
}
