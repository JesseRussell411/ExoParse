using JesseRussell.Numerics;

namespace ExoParseV2
{
    public static class LogicUtils
    {
        public static readonly IntFloatFrac True_IntFloat  = 1;
        public static readonly IntFloatFrac False_IntFloat = 0;
        public static IntFloatFrac? Not(IntFloatFrac? num)
        {
            return (!num.ToBool()).ToIntFloat();
        }
        public static IntFloatFrac? And(IntFloatFrac? a, IntFloatFrac? b)
        {
            return (a.ToBool() & b.ToBool()).ToIntFloat();
        }
        public static IntFloatFrac? Or(IntFloatFrac? a, IntFloatFrac? b)
        {
            return (a.ToBool() | b.ToBool()).ToIntFloat();
        }
        public static IntFloatFrac? Xor(IntFloatFrac? a, IntFloatFrac? b)
        {
            return (a.ToBool() ^ b.ToBool()).ToIntFloat();
        }
        public static IntFloatFrac? Buffer(IntFloatFrac? num)
        {
            return num.ToBool().ToIntFloat();
        }
        public static bool? ToBool(this IntFloatFrac? self)
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
        public static bool? ToBool(this IntFloatFrac self)
        {
            return ((IntFloatFrac?)self).ToBool();
        }
        public static IntFloatFrac? ToIntFloat(this bool? self)
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
        public static IntFloatFrac ToIntFloat(this bool self)
        {
            return (IntFloatFrac)((bool?)self).ToIntFloat();
        }
    }
}
