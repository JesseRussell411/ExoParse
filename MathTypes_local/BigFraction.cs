using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Data.SqlTypes;

namespace MathTypes
{
    /// <summary>
    /// Represents a fraction of arbitrary size and precision.
    /// </summary>
    /// 
    /// <Author>
    /// Jesse Russell
    /// </Author>
    public struct BigFraction
    {
        #region public Properties
        /// <summary>
        /// Top number.
        /// </summary>
        public uBigInteger Numerator { get; }
        /// <summary>
        /// Bottom number.
        /// </summary>
        public uBigInteger Denominator { get; }
        /// <summary>
        /// Stores the sign of the fraction. True for negative. False for positive.
        /// </summary>
        public bool IsNegative { get; }
        #endregion

        #region public Constructors
        public BigFraction(uBigInteger? numerator = null, uBigInteger? denominator = null, bool isNegative = isNegative_default)
        {
            Numerator = numerator ?? numerator_default;
            Denominator = denominator ?? denominator_default;
            IsNegative = isNegative;
        }
        #endregion

        #region public Methods
        public int CompareTo(BigFraction other)
        {
            BigFraction a = EqualizeDenominators(other, out BigFraction b);

            if (a.IsNegative ^ b.IsNegative)
            {
                if (b.IsNegative)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return a.Numerator.CompareTo(b.Numerator) * (a.IsNegative ? -1 : 1);
            }
        }
        public BigFraction Clone(uBigInteger? numerator_new = null, uBigInteger? denominator_new = null, bool? isNegative_new = null)
            => new BigFraction(numerator_new ?? Numerator, denominator_new ?? Denominator, isNegative_new ?? IsNegative);
        #region Math
        /// <summary>
        /// Efficiently multiplies the calling fraction by -1;
        /// </summary>
        /// <returns></returns>
        public BigFraction Neg() => new BigFraction(Numerator, Denominator, !IsNegative);

        /// <summary>
        /// Adds other to the calling function.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public BigFraction Add(BigFraction other)
        {
            uBigInteger numerator = Numerator;
            uBigInteger denominator = Denominator;
            bool isNegative = IsNegative;

            uBigInteger other_num = other.Numerator;

            // Equalize the denominators:
            if (denominator != other.Denominator)
            {
                numerator *= other.Denominator;
                other_num = other.Numerator * denominator;
                denominator *= other.Denominator;
            }
            //


            if (isNegative ^ other.IsNegative)
            {
                if (numerator > other_num)
                {
                    //IsNegative doesn't change.
                    numerator -= other_num;
                }
                else if (Numerator < other_num)
                {
                    isNegative = other.IsNegative;
                    numerator = other_num - numerator;
                }
                else // ==
                {
                    numerator = 0;
                    isNegative = false;
                }
            }
            else
            {
                numerator += other_num;
                isNegative ^= other.IsNegative;
            }

            return new BigFraction(numerator, denominator, isNegative);
        }

        /// <summary>
        /// Subtracts other from the calling function.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public BigFraction Subtract(BigFraction other) => Add(other.Neg());

        /// <summary>
        /// Multiplies the calling fraction by other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public BigFraction Multiply(BigFraction other)
        {
            return new BigFraction(
                Numerator * other.Numerator,
                Denominator * other.Denominator,
                IsNegative ^ other.IsNegative);
        }

        /// <summary>
        /// Divides the calling fraction by another other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public BigFraction Divide(BigFraction other) => Multiply(other.Reciprocate());

        /// <summary>
        /// Replaces the calling fraction with its reciprocal.
        /// </summary>
        /// <returns></returns>
        public BigFraction Reciprocate() => new BigFraction(Denominator, Numerator, IsNegative);

        /// <summary>
        /// Raises the calling fraction to p.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <summary>
        /// Raises the calling fraction to p.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public BigFraction Power(BigInteger p)
        {
            uBigInteger numerator = Numerator;
            uBigInteger denominator = Denominator;

            BigInteger stop = BigInteger.Abs(p);
            for (BigInteger i = 1; i < stop; i++)
            {
                numerator *= Numerator;
                denominator *= Denominator;
            }

            // Deal with negative powers:
            if (p < 0)
            {
                return new BigFraction(denominator, numerator, IsNegative);
            }
            else
            {
                return new BigFraction(numerator, denominator, IsNegative);
            }
            //
        }

        /// <summary>
        /// Simplifies the calling fraction.
        /// </summary>
        /// <returns></returns>
        public BigFraction Simplify()
        {
            uBigInteger numerator = Numerator;
            uBigInteger denominator = Denominator;
            bool isNegative = IsNegative;

            if (numerator == 0)
            {
                denominator = 1;
                isNegative = false;
                return new BigFraction(numerator, denominator, isNegative);
            }

            uBigInteger gcd = findGCD(numerator, denominator);

            for (uBigInteger i = 2; i <= gcd; i++)
            {
                if (numerator % i == 0 && denominator % i == 0)
                {
                    do
                    {
                        // Apply the common denominator
                        numerator /= i;
                        denominator /= i;
                        //
                    } while (Numerator % i == 0 && Denominator % i == 0);

                    // Update scd.
                    gcd = uBigInteger.Min(Numerator, Denominator);
                    //gcd = findGCD(Numerator, Denominator);
                    //
                }
            }

            return new BigFraction(numerator, denominator, isNegative);
        }
        #endregion

        #region Conversion
        public IntFloat ToIntFloat()
        {
            if (TryToInteger(out BigInteger big))
            {
                return big;
            }
            else
            {
                return ToDouble();
            }
        }
        public double ToDouble() => (double)Numerator / (double)Denominator;
        public decimal ToDecimal() => (decimal)Numerator / (decimal)Denominator;
        public bool TryToInteger(out BigInteger result)
        {
            result = BigInteger.DivRem(Numerator, Denominator, out BigInteger remainder);
            return remainder == 0;
        }
        #endregion

        #region Casts
        public static implicit operator BigFraction(uBigInteger big) => new BigFraction(big, 1);
        #endregion
        public override string ToString()
        {
            return $"{(IsNegative ? "-" : "")}{Numerator}/{Denominator}";
        }
        #endregion

        #region public static Methods
        // Parse:
        /// Tries to convert the string s to a fraction.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out BigFraction result)
        {
            // Split string on the '/' character.
            string[] s_split = s.Split('/', 2).Select(str => str.Trim()).ToArray();

            bool isNeg = false;

            // Remove '-' from numerator string:
            if (s_split[0].FirstOrDefault() == '-')
            {
                s_split[0] = s_split[0].Substring(1).Trim();
                isNeg = true;
            }
            //

            // Remove '-' from denominator string:
            if (s_split[1].FirstOrDefault() == '-')
            {
                s_split[1] = s_split[1].Substring(1).Trim();
                isNeg ^= true;
            }
            //

            // Try to parse numerator and denominator:
            if (uBigInteger.TryParse(s_split[0], out uBigInteger numerator) && uBigInteger.TryParse(s_split[1], out uBigInteger denominator))
            {
                // if successful set result to the a new fraction and return true
                result = new BigFraction(numerator, denominator, isNeg);
                return true;
            }
            else
            {
                result = Default;
                return false;
            }
            //
        }

        public static BigFraction Parse(string s)
        {
            if (TryParse(s, out BigFraction result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"{s} is not a valid fraction.");
            }
        }
        //

        #region Operators
        public static BigFraction operator +(BigFraction left, BigFraction right) => left.Add(right).Simplify();
        public static BigFraction operator -(BigFraction left, BigFraction right) => left.Add(right).Simplify();
        public static BigFraction operator *(BigFraction left, BigFraction right) => left.Multiply(right).Simplify();
        public static BigFraction operator /(BigFraction left, BigFraction right) => left.Divide(right).Simplify();
        public static bool operator >(BigFraction left, BigFraction right) => left.CompareTo(right) > 0;
        public static bool operator >=(BigFraction left, BigFraction right) => left.CompareTo(right) >= 0;
        public static bool operator <(BigFraction left, BigFraction right) => left.CompareTo(right) < 0;
        public static bool operator <=(BigFraction left, BigFraction right) => left.CompareTo(right) <= 0;
        #endregion
        #endregion

        #region hidden Methods
        private BigFraction EqualizeDenominators(BigFraction other) => Clone(Numerator * other.Denominator, Denominator * other.Denominator);
        private BigFraction EqualizeDenominators(BigFraction other, out BigFraction other_equalized)
        {
            other_equalized = other.Clone(other.Numerator * other.Denominator, other.Denominator * other.Denominator);
            return EqualizeDenominators(other);
        }
        #endregion

        #region hidden static Methods
        private static uBigInteger findGCD(uBigInteger a, uBigInteger b)
        {
            // Make sure b is smaller than a:
            if (b > a)
            {
                // Swap a and b:
                uBigInteger temp = a;
                a = b;
                b = temp;
                //
            }
            //

            if (a % b == 0)
            {
                return b;
            }
            else
            {
                return findGCD(a % b, a);
            }
        }
        #endregion

        #region public Defaults
        public static readonly BigFraction Default = new BigFraction(numerator_default, denominator_default, isNegative_default);
        #endregion

        #region hidden Defaults
        private static readonly uBigInteger numerator_default = 0;
        private static readonly uBigInteger denominator_default = 1;
        private const bool isNegative_default = false;
        #endregion
    }
}