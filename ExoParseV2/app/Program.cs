using System.Diagnostics;
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




            bool stop = false;
            Stopwatch s = new Stopwatch();
            while (!stop)
            {
                ui.ReadAndRunLine();
            }
        }
    }
}
