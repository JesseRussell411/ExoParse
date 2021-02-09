using System;

namespace ParsingTools
{
    public class BracketDisparityException : Exception
    {
        public BracketDisparityException()
            : base("The number of openBrackets and closeBrackets is not equal. Every opening bracket needs to have a corresponding closing bracket stored at the same index.")
        {
            
        }
    }


    public class UnClosedBracketException : Exception
    {
        public string Statement { get; set; }
        public string ExpectedBracket { get; set; }
        public UnClosedBracketException(String statement, String expectedBracket)
            : base($"Missing closing bracket: {expectedBracket} expected in \"{statement}\".")
        {
            Statement = statement;
            ExpectedBracket = expectedBracket;
        }
    }

    public class UnOpenedBracketException : Exception
    {
        public string Statement { get; set; }
        public string ExpectedBracket { get; set; }
        public UnOpenedBracketException(String statement, String expectedBracket)
            : base($"Missing opening bracket: {expectedBracket} expected in \"{statement}\".")
        {
            Statement = statement;
            ExpectedBracket = expectedBracket;
        }
    }
}
