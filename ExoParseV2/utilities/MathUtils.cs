using System;
using System.Numerics;
using MathTypes;


namespace ExoParseV2
{
    public static class MathUtils
    {
        private static Random rand = new Random();
        public static IntFloat? Div(IntFloat? a, IntFloat? b)
        {
            if (b == 0)
            {
                throw new ExecutionException("Divide by zero error.");

            }

            return a / b;
        }
        public static IntFloat? FloorDiv(IntFloat? a, IntFloat? b)
        {
            //return Floor(Div(a, b));
            if (a == null || b == null) { return null; }

            return IntFloat.FloorDivide((IntFloat) a, (IntFloat)b);
        }
        public static IntFloat? Log(IntFloat? d, IntFloat? newBase)
        {
            if (d == null || newBase == null) { return null; }
            return IntFloat.Log((IntFloat)d, (IntFloat)newBase);
        }
        public static IntFloat? Log10(IntFloat? d)
        {
            if (d == null) { return null; }
            return IntFloat.Log10((IntFloat)d);
        }
        public static IntFloat? Ln(IntFloat? d)
        {
            if (d == null) { return null; }
            return IntFloat.Log((IntFloat)d, Math.E);
        }
        public static IntFloat? Factorial(IntFloat? n)
        {
            if (n == null) { return null; }
            if (n == 0.0) { return 1.0; }

            if (n < 0 || n != IntFloat.Floor((IntFloat)n))
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

            return (IntFloat)fact;
        }

        #region trigonometry
        public static IntFloat? Sin(IntFloat? x)
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

        public static IntFloat? Cos(IntFloat? x)
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
        public static IntFloat? Tan(IntFloat? x)
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
        public static IntFloat? Asin(IntFloat? x)
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

        public static IntFloat? Acos(IntFloat? x)
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

        public static IntFloat? Atan(IntFloat? x)
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

        public static IntFloat? ToDegrees(IntFloat? radians)
        {
            return radians * (180.0 / Math.PI);
        }
        public static IntFloat? ToRadians(IntFloat? degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
        #endregion

        public static IntFloat? Power(IntFloat? x, IntFloat? y)
        {
            if (x == null || y == null)
            {
                return null;
            }
            else
            {
                return IntFloat.Pow((IntFloat)x, (IntFloat)y);
            }
        }
        public static IntFloat? Floor(IntFloat? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return IntFloat.Floor((IntFloat)d);
            }
        }
        public static IntFloat? Ceiling(IntFloat? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return IntFloat.Ceiling((IntFloat)d);
            }
        }
        public static IntFloat? Abs(IntFloat? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return IntFloat.Abs((IntFloat)d);
            }
        }
        public static IntFloat? Sign(IntFloat? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return IntFloat.Sign((IntFloat)d);
            }
        }

        public static IntFloat? Min(IntFloat? a, IntFloat? b)
        {
            if (a == null || b == null)
            {
                return null;
            }
            else
            {
                return IntFloat.Min((IntFloat)a, (IntFloat)b);
            }
        }

        public static IntFloat? Max(IntFloat? a, IntFloat? b)
        {
            if (a == null || b == null)
            {
                return null;
            }
            else
            {
                return IntFloat.Max((IntFloat)a, (IntFloat)b);
            }
        }

        public static IntFloat? Round(IntFloat? d)
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
        public static IntFloat? Round(IntFloat? d, IntFloat? decimals)
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