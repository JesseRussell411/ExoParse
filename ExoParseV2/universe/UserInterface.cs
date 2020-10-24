using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ExoParseV2.theUniverse;
using ExoParseV2.universe;

namespace ExoParseV2.universe
{
    class UserInterface
    {
        public UserInterface(StatementTaker statementTaker)
        {
            StatementTaker = statementTaker;
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

        public StatementTaker StatementTaker;

        public string RunLine(string statment)
        {
            return StatementTaker.RunStatement(statment);
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
        }
    }
}
