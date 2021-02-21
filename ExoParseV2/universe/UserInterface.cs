using System;
using System.Threading;
using System.Threading.Tasks;
using ExoParseV2.theUniverse;

namespace ExoParseV2.universe
{
    class UserInterface
    {
        public UserInterface(Universe universe)
        {
            Universe = universe;
        }
        // print:
        public Action<string> Print { get; set; } = Console.Write;
        private void print(object s) { Print(s.ToString()); }
        private void print() { Print(""); }
        private void println(object s) { Print(s.ToString() + "\n"); }
        private void println() { Print("" + "\n"); }
        //

        // read:
        public Func<string> ReadLine { get; set; } = Console.ReadLine;
        private string readln() { return ReadLine(); }
        //

        public Universe Universe { get; set; }

        public string RunLine(string statment)
        {
            return Universe.RunStatement(statment);
        }

        public void ReadAndRunLine()
        {
            print("> ");
            string input = readln();
            try
            {
                string result = RunLine(input);
                println(result);
            }
            catch (MessageException me)
            {
                println(" *" + me.Message);
                println();
            }
            catch (StackOverflowException so) //* sadly, this doesn't work, but hey, maybe someday.
            {
                println(" *Stack Overflow Error.");
                println();
            }
            catch (AggregateException e)
            {
                Console.WriteLine("fsajd");
            }
            catch (Exception e)
            {
                Console.WriteLine("some e");
            }






        }
    }
}
