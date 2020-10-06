using System;
using ParsingTools;
using System.Linq;
using ExoParseV2.Functions;
using System.Diagnostics;
using ExoParseV2.the_universe;
using ExoParseV2.the_universe.Commands;
using ExoParseV2.utilities;




namespace ExoParseV2
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            UniverseFactory uf = new UniverseFactory();


            the_universe.Universe universe = uf.CreateUniverse();

            Stopwatch s = new Stopwatch();
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (universe.Debug)
                {
                    s.Reset();
                    s.Start();
                }

                try
                {
                    universe.TakeLine(input);
                }
                catch (MessageException me)
                {
                    Console.WriteLine(me.Message);
                }

                if (universe.Debug)
                {
                    s.Stop();
                    Console.WriteLine($"                   Total time taken: {s.ElapsedMilliseconds}");
                }
            }
        }
    }
}
