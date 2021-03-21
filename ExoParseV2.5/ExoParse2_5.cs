using System;

namespace ExoParseV2._5
{
    public class ExoParse2_5
    {
        static UniverseFactory uf;
        Universe un;
        UserInterface ui = new UserInterface(un);




        bool stop = false;
        Stopwatch s = new Stopwatch();
            while (!stop)
            {
                ui.ReadAndRunLine();
            }
}
}
