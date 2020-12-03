using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace MathTypes
{
    public static class MathUtils
    {
        public static bool TryToDecimal(double d, out decimal result)
        {
            double d_abs = Math.Abs(d);
            double d_abs_right = d_abs - Math.Truncate(d_abs);
            if (d_abs_right >= DecimalEpsilon_double && d_abs <= DecimalMaxValue_double)
            {
                try
                {
                    result = Convert.ToDecimal(d);
                    return true;
                }
                finally { }
            }

            result = default;
            return false;
        }
        public static bool TryToDecimal(BigInteger bi, out decimal result)
        {
            if (DecimalMinValue_bigint <= bi && bi <= DecimalMaxValue_bigint)
            {
                try
                {
                    result = (decimal)bi;
                    return true;
                }
                finally { }
            }

            result = default;
            return false;
        }

        private const decimal DecimalEpsilon = 1e-28M;
        private const double DecimalEpsilon_double = 1e-28; // *from https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types
        private const double DecimalMaxValue_double = 7.922816251426433E+28; // *from Console.WriteLine(Convert.ToDouble(decimal.MaxValue)); *actual result was 7.922816251426434E+28 (4 vs 3 at end); however, this caused an overflow when converting to decimal, so I reduced it slightly.
        private static readonly BigInteger DecimalMaxValue_bigint = (BigInteger)decimal.MaxValue;
        private static readonly BigInteger DecimalMinValue_bigint = (BigInteger)decimal.MinValue;
    }
}
