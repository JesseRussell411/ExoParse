using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    class UniverseFactory
    {
        public List<Constant> CreateConstants()
        {
            List<Constant> constants = new List<Constant>();

            constants.Add(new Constant("pi", Math.PI));
            constants.Add(new Constant("e", Math.E));
            constants.Add(new Constant("true", LogicUtils.True_double));
            constants.Add(new Constant("false", LogicUtils.False_double));
            constants.Add(new Constant(ParsingProps.NullLabel, ElementUtils.NullElement));
            constants.Add(new Constant("void", ElementUtils.VoidElement));
            constants.Add(new Constant("theMeaningOfLife", new Literal(42)));

            return constants;
        }
    }
}
