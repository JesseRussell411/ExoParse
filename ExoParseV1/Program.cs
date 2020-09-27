//#define diagnostic

using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Collections.Specialized;

namespace ExoParsev1
{
    class Program
    {
        static void Main(string[] args)
        {
            EnvironmentFactory envFactory = new EnvironmentFactory(
                new Function[]
                {
                        new Pi(),
                        new E(),
                        new Sin(),
                        new Cos(),
                        new Tan(),
                        new ASin(),
                        new ACos(),
                        new ATan(),
                        new Abs(),
                        new Sign(),
                        new Floor(),
                        new Ceiling(),
                        new Round(),
                        new Round2(),
                        new Log(),
                        new Min(),
                        new True(),
                        new False(),
                        new Null(),
                        new Void(),
                        new Log2(),
                        new NaturalLog(),
                        new ToRadians(),
                        new ToDegrees(),
                        new GravitationalConstant()
                },
                new Command[] {
                        new ListVars(),
                        new DeleteAllVariables(),
                        new DeleteVar(),
                        new DefineFunc(),
                        new ReDefineFunc(),
                        new DeleteFunc(),
                        new ListFuncs(),
                        new ToggleDebug()
                },
                    new Operator[] {
                        new Addition(),
                        new Subtraction(),
                        new Multiplication(),
                        new Division(),
                        new Modulus(),
                        new FloorDivision(),
                        new Exponentiation(),

                        new SetEqual(),
                        new SetDefinition(),
                        new SetAsDefinition(),

                        new PlusEquals(),
                        new MinusEquals(),
                        new TimesEquals(),
                        new DivEquals(),
                        new FloorDivEquals(),
                        new ExponentiateEquals(),
                        new ModulusEquals(),

                        new CheckEqual(),
                        new CheckNotEqual(),
                        new CheckDef(),
                        new CheckNotDef(),

                        new GreaterThen(),
                        new Lessthen(),
                        new LessthenEqualto(),
                        new GreaterThenEqualTo(),

                        new And(),
                        new ConditionalAnd(),
                        new Or(),
                        new ConditionalOr(),
                        new Xor(),

                        new NullCoalesecer(),
                        new NullcoalescingSetEquals(),
                        new NullCoalescingSetDefinition(),
                        new NullCoalescingSetAsDefinition(),

                        // statement ? option 1 : option 2
                        new TernaryStatement(),//?
                        new TernarySeperator()// :
                    },
                new Modifier[]
                {
                        new Deref(),
                        new GetValue(),

                        new Not(),
                        new Neg(),
                        new Pos(),
                        new DecrementAfter(),
                        new DecrementBefore(),
                        new IncrementAfter(),
                        new IncrementBefore(),
                        new Factorial()
                }
            );

            

            Environment env = envFactory.BuildEnv();

            Console.WriteLine("ExoParse V1    ExoParse V1    ExoParse V1");
            Console.WriteLine("    ExoParse V1        ExoParse V1        ExoParse V1");
            Console.WriteLine("        ExoParse V1            ExoParse V1            ExoParse V1");
            Console.WriteLine("    ExoParse V1        ExoParse V1        ExoParse V1");
            Console.WriteLine("ExoParse V1    ExoParse V1    ExoParse V1");
            Console.WriteLine();
            Console.WriteLine();

            env.Run();


        }
    }
}
