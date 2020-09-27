using System;
using ParsingTools;
using System.Linq;
using ExoParseV2.Functions;
using System.Diagnostics;

namespace ExoParseV2
{
    class Program
    {
        static void Main(string[] args)
        {

            //String[] em = new String[0];
            //Tokenizer whiteSpacer = new Tokenizer(em, em, em, em, ParsingProps.OpenBrackets, ParsingProps.CloseBrackets, em, em) { BreakOnWhiteSpace = true};

            //Tokenizer[] delimer = new Tokenizer[si.priorityCount];

            //for(int i = 0; i < delimer.Length; i++)
            //{
            //    delimer[i] = new Tokenizer(si.GetSymbols(i), em, em, em, ParsingProps.OpenBrackets, ParsingProps.CloseBrackets, em, em) { IncludeEmpty = false };
            //};


            //Tokenizer parenBreaker = new Tokenizer(em, em, ParsingProps.OpenBrackets, ParsingProps.CloseBrackets, ParsingProps.OpenBrackets, ParsingProps.CloseBrackets, em, em) { IncludeEmpty = false } ;

            //string statement = "4 +  3E-2 + bob(4 + 1) + 8*2";

            //Console.WriteLine(statement);

            //string[] whiteBroke = whiteSpacer.Tokenize(statement).ToArray();

            //Console.WriteLine(whiteBroke.ToDelimString());

            //for(int i = 0; i < delimer.Length; i++)
            //{
            //    whiteBroke = whiteBroke.Select(s => delimer[i].Tokenize(s)).SelectMany(m => m).ToArray();
            //    Console.WriteLine(whiteBroke.ToDelimString());
            //}

            //whiteBroke = whiteBroke.Select(s => parenBreaker.Tokenize(s)).SelectMany(m => m).ToArray();
            //Console.WriteLine(whiteBroke.ToDelimString());



            //IExpressionComponent _5p4 = new Operation(addition, 5.ToElement(), 4.ToElement());
            //IExpressionComponent ft_5p4 = new Operation(multiplication, new Execution(new Floor_func(), 5.6.ToElement()), _5p4);
            //IExpressionComponent topExp = new Operation(multiplication, ft_5p4, 2.ToElement());

            //Console.WriteLine($"{topExp.ToString(si)} = {topExp.Execute()}");

            Addition_op addition = new Addition_op();
            Multiplication_op multiplication = new Multiplication_op();

            SymbolizedIndex si = new SymbolizedIndex(
                new ISymbolized[][]
                {
                    new ISymbolized[] {new Dereference_mod()},
                    new ISymbolized[] {new NullCoalescing_op()},
                    new ISymbolized[] {new SetDefinition_op(), new SetAsDefinition_op()},
                    new ISymbolized[] {new CheckEqual_op(),new CheckNotEqual_op(), new GreaterThan_op(), new GreaterThanEqualTo_op(), new LessThan_op(), new LessThanEqualTo_op()},
                    new ISymbolized[] {new SetEqual_op()},
                    new ISymbolized[] {new PlusEqual_op(), new MinusEqual_op()},
                    new ISymbolized[] {new TimesEqual_op(), new DivEqual_op(), new FloorDivsEqual_op(), new ModEqual_op()},
                    new ISymbolized[] {new TimesEqual_op(), new PowerEqual_op()},
                    new ISymbolized[] {new ConditionalAnd_op(), new ConditionalOr_op()},
                    new ISymbolized[] {new And_op(), new Or_op(), new Xor_op()},
                    new ISymbolized[] {addition, new Subtraction_op() },
                    new ISymbolized[] {multiplication, new Division_op(), new FloorDivision_op(), new Modulus_op()},
                    new ISymbolized[] {new Negative_mod(), new Positive_mod()},
                    new ISymbolized[] {new Exponentiation_op()},
                    new ISymbolized[] {new Factorial_mod()},
                    new ISymbolized[] {new Not_mod() },
                    new ISymbolized[] {new IncrementAfter_mod(), new DecrementAfter_mod()},
                    new ISymbolized[] {new IncrementBefore_mod(), new DecrementBefore_mod()},
                    new ISymbolized[] {new Factorial_mod() },
                    new ISymbolized[] {new EqualsNot_mod()},
                }
                );

            Environment env = new Environment();
            env.AddFunction(new Sin_func());
            env.AddFunction(new Cos_func());
            env.AddFunction(new Tan_func());
            env.AddFunction(new ArcSin_func());
            env.AddFunction(new ArcCos_func());
            env.AddFunction(new ArcTan_func());
            env.AddFunction(new ToDegrees_func());
            env.AddFunction(new ToRadians_func());
            env.AddFunction(new Sign_func());
            env.AddFunction(new AbsoluteValue_func());
            env.AddFunction(new Floor_func());
            env.AddFunction(new Ceiling_func());
            env.AddFunction(new Min_func());
            env.AddFunction(new Max_func());
            env.AddFunction(new Log_func());
            env.AddFunction(new Log10_func());
            env.AddFunction(new NaturalLog_func());



            Parser parser = new Parser(si, env);
            parser.DefaultOperator = multiplication;

            //IElement el = parser.ParseElement("log(log(4, 16), log(2, 16))^-2");

            //string str = el.ToString(si, null);

            //double? dob = el.Execute();

            //Console.WriteLine($"{str} = {dob}");

            UniverseFactory uf = new UniverseFactory();
            env.AddLabeled(uf.CreateConstants());
            Variable ans_var = new Variable("system_Ans_var");
            FinalVariable ans = new FinalVariable("ans", ans_var);
            env.AddLabeled(ans);
            parser.Starter = ans;
            Stopwatch s = new Stopwatch();
            IElement el;
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                try
                {
                    s.Reset();
                    s.Start();
                    el = parser.InternalParseElement(input);
                    s.Stop();

                    IElement p = el.Pass();
                    double? ex = el?.Execute();
                    ans_var.Definition = ex.ToElement();

                    Console.WriteLine($"{el?.ToString(si, null)} := {p.NullableToString(ParsingProps.NullLabel) }\n");
                    Console.WriteLine($"{el?.ToString(si, null)} = {ex.NullableToString(ParsingProps.NullLabel) }\n");
                    Console.WriteLine($"Parse time (milliseconds): {(s.ElapsedMilliseconds)}");
                }
                catch(MessageException me)
                {
                    Console.WriteLine(me.Message);
                }
            }
        }
    }
}
