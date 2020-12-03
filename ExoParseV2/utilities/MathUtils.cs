using System;
using System.Numerics;
using MathTypes;


namespace ExoParseV2
{
    public static class MathUtils
    {
        private static Random rand = new Random();
        public static IntFloatFrac? Div(IntFloatFrac? a, IntFloatFrac? b)
        {
            if (b == 0)
            {
                throw new ExecutionException("Divide by zero error.");

            }

            return a / b;
        }
        public static IntFloatFrac? FloorDiv(IntFloatFrac? a, IntFloatFrac? b)
        {
            //return Floor(Div(a, b));
            if (a == null || b == null) { return null; }

            return IntFloatFrac.FloorDivide((IntFloatFrac) a, (IntFloatFrac)b);
        }
        public static IntFloatFrac? FloatDiv(IntFloatFrac? a, IntFloatFrac? b)
        {
            if (a == null || b == null) { return null; }
            return a?.Float / b?.Float;
        }
        public static IntFloatFrac? Log(IntFloatFrac? d, IntFloatFrac? newBase)
        {
            if (d == null || newBase == null) { return null; }
            return IntFloatFrac.Log((IntFloatFrac)d, ((IntFloatFrac)newBase).Float);
        }
        public static IntFloatFrac? Log10(IntFloatFrac? d)
        {
            if (d == null) { return null; }
            return IntFloatFrac.Log10((IntFloatFrac)d);
        }
        public static IntFloatFrac? Ln(IntFloatFrac? d)
        {
            if (d == null) { return null; }
            return IntFloatFrac.Log((IntFloatFrac)d, Math.E);
        }
        public static IntFloatFrac? Factorial(IntFloatFrac? n)
        {
            if (n == null) { return null; }
            if (n == 0.0) { return 1.0; }

            if (n < 0 || n != IntFloatFrac.Floor((IntFloatFrac)n))
            {
                throw new ExecutionException($"Factorial can only be performed on positive integers and 0.");
            }

            BigInteger n_BigInteger = n?.Int ?? 0;

            BigInteger fact = n_BigInteger;
            n_BigInteger--;
            for (; n_BigInteger > 1; n_BigInteger--)
            {
                fact *= n_BigInteger;
            }

            return (IntFloatFrac)fact;
        }

        #region trigonometry
        public static IntFloatFrac? Sin(IntFloatFrac? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Sin(x?.Float ?? 0);
            }
        }

        public static IntFloatFrac? Cos(IntFloatFrac? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Cos(x?.Float ?? 0);
            }
        }
        public static IntFloatFrac? Tan(IntFloatFrac? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Tan(x?.Float ?? 0);
            }
        }
        public static IntFloatFrac? Asin(IntFloatFrac? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Asin(x?.Float ?? 0);
            }
        }

        public static IntFloatFrac? Acos(IntFloatFrac? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Acos(x?.Float ?? 0);
            }
        }

        public static IntFloatFrac? Atan(IntFloatFrac? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Atan(x?.Float ?? 0);
            }
        }

        public static IntFloatFrac? ToDegrees(IntFloatFrac? radians)
        {
            return radians * (180.0 / Math.PI);
        }
        public static IntFloatFrac? ToRadians(IntFloatFrac? degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
        #endregion

        public static IntFloatFrac? Power(IntFloatFrac? x, IntFloatFrac? y)
        {
            if (x == null || y == null)
            {
                return null;
            }
            else
            {
                return IntFloatFrac.Pow((IntFloatFrac)x, (int)(IntFloatFrac)y);
            }
        }
        public static IntFloatFrac? Floor(IntFloatFrac? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return IntFloatFrac.Floor((IntFloatFrac)d);
            }
        }
        public static IntFloatFrac? Ceiling(IntFloatFrac? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return IntFloatFrac.Ceiling((IntFloatFrac)d);
            }
        }
        public static IntFloatFrac? Abs(IntFloatFrac? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return IntFloatFrac.Abs((IntFloatFrac)d);
            }
        }
        public static IntFloatFrac? Sign(IntFloatFrac? d) => d?.Sign;

        public static IntFloatFrac? Min(IntFloatFrac? a, IntFloatFrac? b)
        {
            if (a == null || b == null)
            {
                return null;
            }
            else
            {
                return IntFloatFrac.Min((IntFloatFrac)a, (IntFloatFrac)b);
            }
        }

        public static IntFloatFrac? Max(IntFloatFrac? a, IntFloatFrac? b)
        {
            if (a == null || b == null)
            {
                return null;
            }
            else
            {
                return IntFloatFrac.Max((IntFloatFrac)a, (IntFloatFrac)b);
            }
        }

        public static IntFloatFrac? Round(IntFloatFrac? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return (BigInteger)Math.Round((double)d);
            }
        }
        public static IntFloatFrac? Round(IntFloatFrac? d, IntFloatFrac? decimals)
        {
            if (d == null || decimals == null || decimals != Floor(decimals))
            {
                return null;
            }
            else
            {
                BigInteger decimals_bigInt = decimals?.Int ?? 0;
                int decimals_int = decimals_bigInt > 15 ? 15 : decimals_bigInt < 0 ? 0 : (int)decimals_bigInt;
                return Math.Round((double)d, decimals_int);
            }
        }
    }
}