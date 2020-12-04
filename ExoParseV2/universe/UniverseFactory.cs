using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.Functions;
using ExoParseV2.theUniverse;
using ExoParseV2.theUniverse.commands;
using ExoParseV2.elements;
using MathTypes;

namespace ExoParseV2
{
    public class UniverseFactory
    {
        public Universe CreateUniverse()
        {
            Universe uni = new Universe();
            uni.PrintFunction = Console.Write;
            uni.ReadFunction = Console.ReadLine;
            multiplication = new Multiplication_op();
            starter = uni.Ans;
            defaultElement = ElementUtils.VoidElement;

            SymbolizedIndex si = CreateSymbolizedIndex();

            uni.AddFunctions(CreateBuiltInFunctions(uni));
            uni.AddLabeled(CreateConstants());

            Parser pars = new Parser(si, uni);

            pars.Starter = starter;
            pars.DefaultOperator = multiplication;
            pars.DefaultElement = defaultElement;

            uni.SymbolizedIndex = si;
            uni.Parser = pars;
            uni.AddCommands(CreateCommands());

            uni.AddLabeled(uni.Ans);


            return uni;
        }
        private Operator multiplication = null;
        private IElement starter = null;
        private IElement defaultElement = ElementUtils.VoidElement;
        private List<Command> CreateCommands()
        {
            var commands = new List<Command>();
            commands.Add(new Help_cmd());
            commands.Add(new Exit_cmd());
            commands.Add(new Debug_cmd());
            commands.Add(new ListVars_cmd());
            commands.Add(new ListFuncs_cmd());
            commands.Add(new ListOps_cmd());
            commands.Add(new Def_cmd());
            commands.Add(new Redef_cmd());
            commands.Add(new Delete_cmd());
            commands.Add(new Echo_cmd());
            commands.Add(new Run_cmd());
            commands.Add(new GenerateScript_cmd());
            commands.Add(new ReGenerateScript_cmd());
            return commands;
        }

        private List<Constant> CreateConstants()
        {
            List<Constant> constants = new List<Constant>();

            constants.Add(new BuiltInConstant("pi", Math.PI));
            constants.Add(new BuiltInConstant("e", Math.E));
            constants.Add(new BuiltInConstant("true", LogicUtils.True_IntFloat));
            constants.Add(new BuiltInConstant("false", LogicUtils.False_IntFloat));
            constants.Add(new BuiltInConstant(StringProps.NullLabel, ElementUtils.NullElement));
            constants.Add(new BuiltInConstant(StringProps.VoidLabel, ElementUtils.VoidElement));
            //constants.Add(new BuiltInConstant("theMeaningOfLife", new Literal(42)));
            constants.Add(new BuiltInConstant("inf", IntFloatFrac.PositiveInfinity));

            return constants;
        }

        private SymbolizedIndex CreateSymbolizedIndex()
        {

            // This is were the operators and modifiers are added to the universe.
            // The order in which they are added is what defines their priority.
            // Some are added at the same time; hence, they have the same priority.
            SymbolizedIndex si = new SymbolizedIndex(
                new ISymbolized[][]
                {
                    new ISymbolized[] {new Semicolon_post_mod()},
                    new ISymbolized[] {new Semicolon_op()},
                    new ISymbolized[] {new SetDefinition_op()},
                    new ISymbolized[] {new Ternary_op(), new TernarySeperator_op()},
                    new ISymbolized[] {new NullCoalescing_op()},
                    new ISymbolized[] {new ConditionalAnd_op(), new ConditionalOr_op()},
                    new ISymbolized[] {new And_op(), new Or_op(), new Xor_op()},
                    new ISymbolized[] {new CheckEqual_op(),new CheckNotEqual_op(), new GreaterThan_op(), new GreaterThanEqualTo_op(), new LessThan_op(), new LessThanEqualTo_op()},
                    new ISymbolized[] {new SetEqual_op()},
                    new ISymbolized[] {new PlusEqual_op(), new MinusEqual_op()},
                    new ISymbolized[] {new TimesEqual_op(), new DivEqual_op(), new FloorDivsEqual_op(), new ModEqual_op(), new PowerEqual_op(), new XorEqual_op()},
                    new ISymbolized[] {new Addition_op(), new Subtraction_op() },
                    new ISymbolized[] {multiplication, new Division_op(), new FloorDivision_op(), new FloatDivision_op(), new Modulus_op()},
                    new ISymbolized[] {new Negative_mod(), new Positive_mod()},
                    new ISymbolized[] {new Exponentiation_op()},
                    new ISymbolized[] {new Factorial_mod()},
                    new ISymbolized[] {new Not_mod()},
                    new ISymbolized[] {new Increment_post_mod(), new Decrement_post_mod()},
                    new ISymbolized[] {new Increment_pre_mod(), new Decrement_pre_mod()},
                    new ISymbolized[] {new Factorial_mod() },
                    new ISymbolized[] {new Dereference_mod(), new ForceExecute_mod(), new ForceCalc_mod(), new ForcePass_mod() },
                }
                );

            return si;
        }
        private List<BuiltInFunction> CreateBuiltInFunctions( Universe universe)
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
            funcs.Add(new Truncate_func());
            funcs.Add(new Min_func());
            funcs.Add(new Max_func());
            funcs.Add(new Log_func());
            funcs.Add(new Log10_func());
            funcs.Add(new NaturalLog_func());
            funcs.Add(new Round_func());
            funcs.Add(new Round2_func());
            funcs.Add(new While_func());
            funcs.Add(new DoWhile_func());
            funcs.Add(new For_func());
            funcs.Add(new RandomRange_float_func());
            funcs.Add(new ToFloat_func());
            funcs.Add(new ToInt_func());
            funcs.Add(new ToFraction_func());
            funcs.Add(new IsFloat_func());
            funcs.Add(new IsInt_func());
            funcs.Add(new IsFraction_func());
            funcs.Add(new Print_func() { Universe = universe});
            funcs.Add(new PrintLine_func() { Universe = universe});
            funcs.Add(new PrintValue_func() { Universe = universe});
            funcs.Add(new Exit_func());
            funcs.Add(new ExitWithCode_func());
            //funcs.Add(new Frac_func());
            funcs.Add(new Simplify_func());
            return funcs;
        }
    }
}
