using System;
using System.Linq;

namespace ParsingTools
{
    class Test
    {
        public static void Main(string[] args)
        {
            //// From: old c++ project
            ////	string text = "9+bool = var == con:bob := 4 + 9====bob==================:=======+=+=: =: ::= =::::++_-_=-+_=+++_-=+_++_+_++=====++_=_=-=_-:::++;;;;:;+_;-=-;:_+;-+=;=;;;-+=:_;-;-=-;-=:-__-----=++++;-:::-_=:_+++__:;:__:;====-_::==::-";
            ////	vector<string> operators = { "==", "=", ":=", "+=", "-=", "+", "-", "===", "::", "=:" };
            //String testString = "9+bool = var == con:bob := 4 + 9====bob==================:=======+=+=: =: ::= =::::++_-_=-+_=+++_-=+_++_+_++=====++_=_=-=_-:::++;;;;:;+_;-=-;:_+;-+=;=;;;-+=:_;-;-=-;-=:-__-----=++++;-:::-_=:_+++__:;:__:;====-_::==::-";
            //String[] symbols = { "==", "=", ":=", "+=", "-=", "+", "-", "===", "::", "=:" };
            //String text = testString;
            //SymbolFinder finder = new SymbolFinder(symbols);
            //char c;
            //char? next;
            //int text_lastIndex = text.Length - 1;
            //string symb;
            //for(int i = 0; i < text.Length; i++)
            //{
            //    c = text[i];
            //    next = i == text_lastIndex ? null : (char?)text[i + 1];

            //    symb = finder.Find(c, next);

            //    if (symb.Length != 0) { Console.WriteLine(symb); }


            //}


            //// Testing isWrapped:
            //String statement = "{(-<[(a[-       ]s)]>-)}";
            //String openingBracket;
            //String closingBracket;
            //Console.WriteLine(statement.UnWrap());

            Tokenizer tok = new Tokenizer(new String[] { "+", "+=" }, new String[] { "," }, ParsingUtils.OpenBrackets_stan, ParsingUtils.CloseBrackets_stan, ParsingUtils.OpenBrackets_stan, ParsingUtils.CloseBrackets_stan, ParsingUtils.Barriers_stan, new string[0]);

            Console.WriteLine(tok.Tokenize("a+b+c    b+=a(d+(e+8))b+a\"u+9\"a").Aggregate((a, b) => $"{a},{b}"));
            tok.BreakOnWhiteSpace = true;
            Console.WriteLine(tok.Tokenize("1+2 = 4").Aggregate((a, b) => $"{a},{b}"));

            Tokenizer argSplitter = new Tokenizer(new string[0], new string[] { "\"" }, new string[0], new string[0], new string[0], new string[0], new string[] { "\"" }, new string[0]) { BreakOnWhiteSpace = true, IncludeEmpty = false };

            Console.WriteLine(argSplitter.Tokenize("abc \"a b    c\" abc 123   \"b o b\".").Aggregate((a, b) => $"{a}, {b}"));
        }
    }
}
