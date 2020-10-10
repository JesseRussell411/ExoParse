using System;
using ParsingTools;
using System.Linq;
using ExoParseV2.Functions;
using System.Diagnostics;
using ExoParseV2.theUniverse;
using ExoParseV2.theUniverse.Commands;
using ExoParseV2.utilities;
using ExoParseV2.universe;

namespace ExoParseV2
{
    class Program
    {
        static void Main(string[] args)
        {
            UniverseFactory uf = new UniverseFactory();
            theUniverse.Universe un = uf.CreateUniverse();
            UserInterface ui = new UserInterface(un);




            Stopwatch s = new Stopwatch();
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (un.Debug)
                {
                    s.Reset();
                    s.Start();
                }

                try
                {
                    ui.RunLine(input);
                    //un.TakeLine(input);
                }
                catch (MessageException me)
                {
                    Console.WriteLine(me.Message);
                }

                if (un.Debug)
                {
                    s.Stop();
                    Console.WriteLine($"                   Total time taken: {s.ElapsedMilliseconds}");
                }
            }
        }
    }
}
