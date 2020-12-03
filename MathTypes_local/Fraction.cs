#define USE_OP

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
    /// Represents a fraction of arbitrary size.
    /// </summary>
    /// 
    /// <Author>
    /// Jesse Russell
    /// </Author>
    public struct Fraction : IComparable<Fraction>
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
        public bool Negative { get; }

        #region Derived Properties
        /// <summary>
        /// Returns Numerator as positive or negative depending on the value of the Negative property. Numerator * -1 if Negative; Numerator * 1 if not Negative.
        /// </summary>
        public BigInteger SignedNumerator => Numerator.Value * (Negative ? -1 : 1);
        #endregion
        #endregion

        #region public Constructors
        public Fraction(BigInteger? numerator = null, BigInteger? denominator = null, bool? negative = null)
        {
            Numerator = (uBigInteger?) numerator ?? numerator_default;
            Denominator = (uBigInteger?) denominator ?? denominator_default;
            Negative = negative ?? (numerator < 0) ^ (denominator < 0);
        }
        #endregion

        #region public Methods
        public Fraction Clone(uBigInteger? numerator_new = null, uBigInteger? denominator_new = null, bool? isNegative_new = null)
            => new Fraction(numerator_new ?? Numerator, denominator_new ?? Denominator, isNegative_new ?? Negative);
        public override string ToString()
        {
            return $"{(Negative ? "-" : "")}{Numerator}/{Denominator}";
        }
        #region Comparison
        public int CompareTo(Fraction other)
        {
            Fraction a = EqualizeDenominators(other, out Fraction b);

            return (a.Numerator.Value * (a.Negative ? -1 : 1)).CompareTo(b.Numerator.Value * (a.Negative ? -1 : 1));
            
        }
        #endregion
        #region Equality
        public override int GetHashCode()
        {
            Fraction this_simp = Simplify();
            return HashCode.Combine(this_simp.Numerator, this_simp.Denominator, this_simp.Negative);
        }
        public override bool Equals(object obj)
        {
            Fraction? nfrac = obj as Fraction?;
            return nfrac != null && SoftEquals((Fraction)nfrac);
        }

        /// <summary>
        /// Returns true if and only if all parameters of other match all parameters of this Fraction.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HardEquals(Fraction other)
        {
            return Negative == other.Negative &&
                   Numerator == other.Numerator &&
                   Denominator == other.Denominator;
        }

        /// <summary>
        /// Simplifies both fractions before calling HardEquals. Determines whether the this Fraction is mathematically equal to other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SoftEquals(Fraction other) => Simplify().HardEquals(other.Simplify());
        #endregion
        #region Math
        /// <summary>
        /// Efficiently multiplies the calling fraction by -1;
        /// </summary>
        /// <returns></returns>
        public Fraction Neg() => new Fraction(Numerator, Denominator, !Negative);

        /// <summary>
        /// Adds other to the calling function.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Fraction Add(Fraction other)
        {
            uBigInteger numerator = Numerator;
            uBigInteger denominator = Denominator;
            bool isNegative = Negative;

            uBigInteger other_num = other.Numerator;

            // Equalize the denominators:
            if (denominator != other.Denominator)
            {
                numerator *= other.Denominator;
                other_num = other.Numerator * denominator;
                denominator *= other.Denominator;
            }
            //


            if (isNegative ^ other.Negative)
            {
                if (numerator > other_num)
                {
                    //IsNegative doesn't change.
                    numerator -= other_num;
                }
                else if (Numerator < other_num)
                {
                    isNegative = other.Negative;
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
                isNegative ^= other.Negative;
            }

            return new Fraction(numerator, denominator, isNegative);
        }

        /// <summary>
        /// Subtracts other from the calling function.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Fraction Subtract(Fraction other) => Add(other.Neg());

        /// <summary>
        /// Multiplies the calling fraction by other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Fraction Multiply(Fraction other)
        {
            return new Fraction(
                Numerator * other.Numerator,
                Denominator * other.Denominator,
                Negative ^ other.Negative);
        }

        /// <summary>
        /// Divides the calling fraction by another other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Fraction Divide(Fraction other) => Multiply(other.Reciprocate());

        /// <summary>
        /// Replaces the calling fraction with its reciprocal.
        /// </summary>
        /// <returns></returns>
        public Fraction Reciprocate() => new Fraction(Denominator, Numerator, Negative);

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
        public Fraction Power(BigInteger p)
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
                return new Fraction(denominator, numerator, Negative);
            }
            else
            {
                return new Fraction(numerator, denominator, Negative);
            }
            //
        }

        /// <summary>
        /// Simplifies the calling fraction.
        /// </summary>
        /// <returns></returns>
        public Fraction Simplify()
        {
            uBigInteger numerator = Numerator;
            uBigInteger denominator = Denominator;
            bool negative = Negative;

            if (numerator == 0)
            {
                denominator = 1;
                negative = false;
                return new Fraction(numerator, denominator, negative);
            }

            uBigInteger gcd;
            while ((gcd = findGCD(numerator, denominator)) != 1)
            {
                numerator /= gcd;
                denominator /= gcd;
            }


            //for (uBigInteger i = 2; i <= gcd; i++)
            //{
            //    if (numerator % i == 0 && denominator % i == 0)
            //    {
            //        do
            //        {
            //            // Apply the common denominator
            //            numerator /= i;
            //            denominator /= i;
            //            //
            //        } while (numerator % i == 0 && denominator % i == 0);

            //        // Update scd.
            //        gcd = uBigInteger.Min(numerator, denominator);
            //        //gcd = findGCD(Numerator, Denominator);
            //        //
            //    }
            //}

            return new Fraction(numerator, denominator, negative);
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
        #endregion

        #region public static Methods
        #region Parsing
        /// <summary>
        /// Tries to convert the string s to a fraction.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Fraction result)
        {
            // Split string on the '/' character.
            string[] s_split = s.Split('/', 2).Select(str => str.Trim()).ToArray();

            // Try to parse the split string...
            if (s_split.Length == 2)
            {
                // Try to parse numerator and denominator...
                if (BigInteger.TryParse(s_split[0], out BigInteger numerator) && BigInteger.TryParse(s_split[1], out BigInteger denominator))
                {
                    result = new Fraction(numerator, denominator);
                    return true;
                }
                else
                {
                    result = new Fraction();
                    return false;
                }
            }
            else
            {
                // Try to parse just the numerator...
                if (BigInteger.TryParse(s_split.FirstOrDefault(), out BigInteger big))
                {
                    result = big;
                    return true;
                }
                else
                {
                    result = new Fraction();
                    return false;
                }
            }
        }

        public static Fraction Parse(string s)
        {
            if (TryParse(s, out Fraction result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"{s} could not be parsed.");
            }
        }
        #endregion

        #region Casts
        public static implicit operator Fraction(BigInteger big) => new Fraction(big);
        public static implicit operator Fraction(uBigInteger big) => new Fraction(big);
        public static implicit operator Fraction(long l) => new Fraction(l);

        public static explicit operator double(Fraction f) => f.ToDouble();
        public static explicit operator decimal(Fraction f) => f.ToDecimal();
        #endregion

        #region Operators

#if USE_OP
        public static FractionOperation operator +(Fraction left, Fraction right) => left.Add(right);
        public static FractionOperation operator -(Fraction left, Fraction right) => left.Add(right);

        public static FractionOperation operator *(Fraction left, Fraction right) => left.Multiply(right);
        public static FractionOperation operator /(Fraction left, Fraction right) => left.Divide(right);
#else
        public static Fraction operator +(Fraction left, Fraction right) => left.Add(right).Simplify();
        public static Fraction operator -(Fraction left, Fraction right) => left.Add(right).Simplify();

        public static Fraction operator *(Fraction left, Fraction right) => left.Multiply(right).Simplify();
        public static Fraction operator /(Fraction left, Fraction right) => left.Divide(right).Simplify();
#endif

        public static bool operator >(Fraction left, Fraction right) => left.CompareTo(right) > 0;
        public static bool operator >=(Fraction left, Fraction right) => left.CompareTo(right) >= 0;
        public static bool operator <(Fraction left, Fraction right) => left.CompareTo(right) < 0;
        public static bool operator <=(Fraction left, Fraction right) => left.CompareTo(right) <= 0;

        public static bool operator ==(Fraction left, Fraction right) => left.SoftEquals(right);
        public static bool operator !=(Fraction left, Fraction right) => !left.SoftEquals(right);
        #endregion
        #endregion

        #region hidden Methods
        internal Fraction EqualizeDenominators(Fraction other) => Clone(Numerator * other.Denominator, Denominator * other.Denominator);
        internal Fraction EqualizeDenominators(Fraction other, out Fraction other_equalized)
        {
            other_equalized = other.Clone(other.Numerator * other.Denominator, other.Denominator * other.Denominator);
            return EqualizeDenominators(other);
        }
        #endregion

        #region hidden static Methods
        private static uBigInteger findGCD(uBigInteger a, uBigInteger b)
        {
            // Make sure b is smaller than a...
            if (b > a)
            {
                // Swap a and b:
                uBigInteger temp = a;
                a = b;
                b = temp;
                //
            }

            // Special case: b is zero.
            if (b == 0) { return a; }

            // Main loop...
            while (a % b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }

            // Finished.
            return b;
        }
        #endregion

        #region public Defaults
        public static readonly Fraction Default = new Fraction(numerator_default, denominator_default, isNegative_default);
        #endregion

        #region hidden Defaults
        private static readonly uBigInteger numerator_default = 0;
        private static readonly uBigInteger denominator_default = 1;
        private const bool isNegative_default = false;
        #endregion
    }

    /// <summary>
    /// Represents a Fraction that does not auto-simplify between operations such as: +, -, *, /, and %. Simplifies when cast to Fraction, and on call of ToString().
    /// </summary>
    public struct FractionOperation
    {
        /// <summary>
        /// The Fraction represented.
        /// </summary>
        public Fraction Value { get; }
        //public UInt16 OpCount { get; }
        //public static UInt16 Trigger = 5;
        public FractionOperation(Fraction value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.Simplify().ToString();
        }

        #region Operators
        public static FractionOperation operator +(FractionOperation a, Fraction b) => a.Value + b;
        public static FractionOperation operator +(Fraction a, FractionOperation b) => a + b.Value;

        public static FractionOperation operator -(FractionOperation a, Fraction b) => a.Value - b;
        public static FractionOperation operator -(Fraction a, FractionOperation b) => a - b.Value;

        public static FractionOperation operator *(FractionOperation a, Fraction b) => a.Value * b;
        public static FractionOperation operator *(Fraction a, FractionOperation b) => a * b.Value;

        public static FractionOperation operator /(FractionOperation a, Fraction b) => a.Value / b;
        public static FractionOperation operator /(Fraction a, FractionOperation b) => a / b.Value;
        #endregion

        #region Casts
        public static implicit operator Fraction(FractionOperation fo) => fo.Value.Simplify();
        public static implicit operator FractionOperation(Fraction f) => new FractionOperation(f);
        #endregion
    }

    public static class FractionUtils
    {
        public static Fraction NextFraction(this Random rand)
        {
            return new Fraction(rand.Next(int.MinValue, int.MaxValue), rand.Next(int.MinValue, int.MaxValue));
        }

        public static Fraction NextFraction(this Random rand, int minNumerator, int maxNumerator, int minDenominator, int maxDenominator)
        {
            return new Fraction(rand.Next(minNumerator, maxNumerator), rand.Next(minDenominator, maxDenominator));
        }
    }
}