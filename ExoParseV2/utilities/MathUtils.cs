using System;
using System.Numerics;


namespace ExoParseV2
{
    public static class MathUtils
    {
        public static double? Div(double? a, double? b)
        {
            if (b == 0)
            {
                throw new ExecutionException("Divide by zero error.");
            }

            return a / b;
        }
        public static double? FloorDiv(double? a, double? b)
        {
            return Floor(Div(a, b));
        }
        public static double? Log(double? d, double? newBase)
        {
            if (d == null || newBase == null) { return null; }
            return Math.Log((double)d, (double)newBase);
        }
        public static double? Log10(double? d)
        {
            if (d == null) { return null; }
            return Math.Log10((double)d);
        }
        public static double? Ln(double? d)
        {
            if (d == null) { return null; }
            return Math.Log((double)d);
        }
        public static double? Factorial(double? n)
        {
            if (n == null) { return null; }
            if (n == 0.0)  { return 1.0;  }

            if (n < 0 || n % 1 != 0)
            {
                throw new ExecutionException($"Factorial can only be performed on positive integers and 0.");
            }

            BigInteger n_BigInteger = new BigInteger((double)n);
            if (n_BigInteger > 170)
            {
                return double.PositiveInfinity;
            }

            BigInteger fact = n_BigInteger;
            n_BigInteger--;
            for (; n_BigInteger > 1; n_BigInteger--)
            {
                fact *= n_BigInteger;
            }
            return (double)fact;
        }

        #region trigonometry
        public static double? Sin(double? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Sin((double)x);
            }
        }

        public static double? Cos(double? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Cos((double)x);
            }
        }
        public static double? Tan(double? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Tan((double)x);
            }
        }
        public static double? Asin(double? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Asin((double)x);
            }
        }

        public static double? Acos(double? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Acos((double)x);
            }
        }

        public static double? Atan(double? x)
        {
            if (x == null)
            {
                return null;
            }
            else
            {
                return Math.Atan((double)x);
            }
        }

        public static double? ToDegrees(double? radians)
        {
            return radians * (180.0 / Math.PI);
        }
        public static double? ToRadians(double? degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
        #endregion

        public static double? Power(double? x, double? y)
        {
            if (x == null || y == null)
            {
                return null;
            }
            else
            {
                return Math.Pow((double)x, (double)y);
            }
        }
        public static double? Floor(double? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return Math.Floor((double)d);
            }
        }
        public static double? Ceiling(double? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return Math.Ceiling((double)d);
            }
        }
        public static double? Abs(double? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return Math.Abs((double)d);
            }
        }
        public static double? Sign(double? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return Math.Sign((double)d);
            }
        }

        public static double? Min(double? a, double? b)
        {
            if (a == null || b == null)
            {
                return null;
            }
            else
            {
                return Math.Min((double)a, (double)b);
            }
        }

        public static double? Max(double? a, double? b)
        {
            if (a == null || b == null)
            {
                return null;
            }
            else
            {
                return Math.Max((double)a, (double)b);
            }
        }

        public static double? Round(double? d)
        {
            if (d == null)
            {
                return null;
            }
            else
            {
                return Math.Round((double)d);
            }
        }
        public static double? Round(double? d, double? decimals)
        {
            if (d == null || decimals == null || decimals != Floor(decimals))
            {
                return null;
            }
            else
            {
                if (decimals > 15) { decimals = 15; }
                if (decimals < 0) { decimals = 0; }
                return Math.Round((double)d, (int)decimals);
            }
        }
    }
}