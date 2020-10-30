using System;
using System.Collections.Generic;
using System.Text;
using IntegerFloatingPoint;

namespace ExoParseV2
{
    public static class LogicUtils
    {
        public static readonly IntFloat True_IntFloat  = 1;
        public static readonly IntFloat False_IntFloat = 0;
        public static IntFloat? Not(IntFloat? num)
        {
            return (!num.ToBool()).ToIntFloat();
        }
        public static IntFloat? And(IntFloat? a, IntFloat? b)
        {
            return (a.ToBool() & b.ToBool()).ToIntFloat();
        }
        public static IntFloat? Or(IntFloat? a, IntFloat? b)
        {
            return (a.ToBool() | b.ToBool()).ToIntFloat();
        }
        public static IntFloat? Xor(IntFloat? a, IntFloat? b)
        {
            return (a.ToBool() ^ b.ToBool()).ToIntFloat();
        }
        public static IntFloat? Buffer(IntFloat? num)
        {
            return num.ToBool().ToIntFloat();
        }
        public static bool? ToBool(this IntFloat? self)
        {
            if (self == True_IntFloat)
            {
                return true;
            }
            else if (self == False_IntFloat)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
        public static bool? ToBool(this IntFloat self)
        {
            return ((IntFloat?)self).ToBool();
        }
        public static IntFloat? ToIntFloat(this bool? self)
        {
            if (self == true)
            {
                return True_IntFloat;
            }
            else if (self == false)
            {
                return False_IntFloat;
            }
            else
            {
                return null;
            }
        }
        public static IntFloat ToIntFloat(this bool self)
        {
            return (IntFloat)((bool?)self).ToIntFloat();
        }
    }
}
