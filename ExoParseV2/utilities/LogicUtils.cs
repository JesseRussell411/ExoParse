using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public static class LogicUtils
    {
        public static readonly double True_double = 1.0;
        public static  readonly double False_double = 0.0;
        public static double? Not(double? num)
        {
            return (!num.ToBool()).ToDouble();
        }
        public static double? And(double? a, double? b)
        {
            return (a.ToBool() & b.ToBool()).ToDouble();
        }
        public static double? Or(double? a, double? b)
        {
            return (a.ToBool() | b.ToBool()).ToDouble();
        }
        public static double? Xor(double? a, double? b)
        {
            return (a.ToBool() ^ b.ToBool()).ToDouble();
        }
        public static double? Buffer(double? num)
        {
            return num.ToBool().ToDouble();
        }
        public static bool? ToBool(this double? self)
        {
            if (self == True_double)
            {
                return true;
            }
            else if (self == False_double)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
        public static bool? ToBool(this double self)
        {
            return ((double?)self).ToBool();
        }
        public static double? ToDouble(this bool? self)
        {
            if (self == true)
            {
                return True_double;
            }
            else if (self == false)
            {
                return False_double;
            }
            else
            {
                return null;
            }
        }
        public static double ToDouble(this bool self)
        {
            return (double)((bool?)self).ToDouble();
        }
    }
}
