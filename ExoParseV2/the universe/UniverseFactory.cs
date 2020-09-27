using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    class UniverseFactory
    {
        public List<FinalVariable> CreateConstants()
        {
            List<FinalVariable> constants = new List<FinalVariable>();

            constants.Add(new FinalVariable("pi", Math.PI));
            constants.Add(new FinalVariable("e", Math.E));
            constants.Add(new FinalVariable("true", LogicUtils.True_double));
            constants.Add(new FinalVariable("false", LogicUtils.False_double));
            constants.Add(new FinalVariable(ParsingProps.NullLabel, ElementUtils.NullElement));
            constants.Add(new FinalVariable("void", ElementUtils.VoidElement));
            constants.Add(new FinalVariable("theMeaningOfLife", new Constant(42)));

            return constants;
        }
    }
}
