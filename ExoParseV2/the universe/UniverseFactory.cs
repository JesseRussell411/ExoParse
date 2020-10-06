using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.Functions;
using ExoParseV2.the_universe;
using ExoParseV2.the_universe.Commands;

namespace ExoParseV2
{
    class UniverseFactory
    {
        public Universe CreateUniverse()
        {
            Universe uni = new Universe();
            multiplication = new Multiplication_op();
            starter = uni.Ans;
            defaultElement = ElementUtils.VoidElement;

            SymbolizedIndex si = CreateSymbolizedIndex();

            uni.AddFunctions(CreateBuiltInFunctions());
            uni.AddLabeled(CreateConstants());

            Parser pars = new Parser(si, uni);

            pars.Starter = starter;
            pars.DefaultOperator = multiplication;
            pars.DefaultElement = defaultElement;

            uni.SymbolizedIndex = si;
            uni.Parser = pars;
            uni.AddCommands(CreateCommands());
            uni.PrintFunction = Console.Write;
            uni.ReadFunction = Console.ReadLine;

            uni.AddLabeled(uni.Ans);


            return uni;
        }
        private Operator multiplication = null;
        private IElement starter = null;
        private IElement defaultElement = ElementUtils.VoidElement;
        private List<Command> CreateCommands()
        {
            var commands = new List<Command>();
            commands.Add(new ListVars_cmd());
            commands.Add(new Help_cmd());
            commands.Add(new Exit_cmd());
            commands.Add(new Debug_cmd());
            commands.Add(new ListFuncs_cmd());
            return commands;
        }

        private List<Constant> CreateConstants()
        {
            List<Constant> constants = new List<Constant>();

            constants.Add(new Constant("pi", Math.PI));
            constants.Add(new Constant("e", Math.E));
            constants.Add(new Constant("true", LogicUtils.True_double));
            constants.Add(new Constant("false", LogicUtils.False_double));
            constants.Add(new Constant(StringProps.NullLabel, ElementUtils.NullElement));
            constants.Add(new Constant("void", ElementUtils.VoidElement));
            constants.Add(new Constant("theMeaningOfLife", new Literal(42)));

            return constants;
        }

        private SymbolizedIndex CreateSymbolizedIndex()
        {

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
                    new ISymbolized[] {new Addition_op(), new Subtraction_op() },
                    new ISymbolized[] {multiplication, new Division_op(), new FloorDivision_op(), new Modulus_op()},
                    new ISymbolized[] {new Negative_mod(), new Positive_mod()},
                    new ISymbolized[] {new Exponentiation_op()},
                    new ISymbolized[] {new Factorial_mod()},
                    new ISymbolized[] {new Not_mod()},
                    new ISymbolized[] {new IncrementAfter_mod(), new DecrementAfter_mod()},
                    new ISymbolized[] {new IncrementBefore_mod(), new DecrementBefore_mod()},
                    new ISymbolized[] {new Factorial_mod() },
                    new ISymbolized[] {new EqualsNot_mod()},
                }
                );

            return si;
        }
        private List<BuiltInFunction> CreateBuiltInFunctions()
        {
            var funcs = new List<BuiltInFunction>();
            funcs.Add(new Sin_func());
            funcs.Add(new Cos_func());
            funcs.Add(new Tan_func());
            funcs.Add(new ArcSin_func());
            funcs.Add(new ArcCos_func());
            funcs.Add(new ArcTan_func());
            funcs.Add(new ToDegrees_func());
            funcs.Add(new ToRadians_func());
            funcs.Add(new Sign_func());
            funcs.Add(new AbsoluteValue_func());
            funcs.Add(new Floor_func());
            funcs.Add(new Ceiling_func());
            funcs.Add(new Min_func());
            funcs.Add(new Max_func());
            funcs.Add(new Log_func());
            funcs.Add(new Log10_func());
            funcs.Add(new NaturalLog_func());
            funcs.Add(new Round_func());
            funcs.Add(new Round2_func());
            return funcs;
        }
    }
}
