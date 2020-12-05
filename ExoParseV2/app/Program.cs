using System;
using ParsingTools;
using System.Linq;
using ExoParseV2.Functions;
using System.Diagnostics;
using ExoParseV2.theUniverse;
using ExoParseV2.theUniverse.commands;
using ExoParseV2.utilities;
using ExoParseV2.universe;
using System.Text;
using JesseRussell.Numerics;

namespace ExoParseV2
{
    class Program
    {
        static void Main(string[] args)
        {
            UniverseFactory uf = new UniverseFactory();
            theUniverse.Universe un = uf.CreateUniverse();
            UserInterface ui = new UserInterface(un);




            bool stop = false;
            Stopwatch s = new Stopwatch();
            while (!stop)
            {
                ui.ReadAndRunLine();
            }
        }
    }
}
