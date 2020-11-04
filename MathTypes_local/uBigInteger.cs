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
    /// Unsigned BigInteger
    /// </summary>
    ///
    /// <Author>
    /// Jesse Russell
    /// </Author>
    public struct uBigInteger : IComparable, IComparable<uBigInteger>, IEquatable<uBigInteger>
    {
        #region public Properties
        public BigInteger Value { get; }


        public bool IsEven { get => Value.IsEven; }
        public bool IsZero { get => Value.IsZero; }
        public bool IsOne { get => Value.IsOne; }
        public bool IsPowerOfTwo { get => Value.IsPowerOfTwo; }
        public int Sign { get => Value.Sign; }
        #endregion
        
        #region public Constructors
        /// <summary>
        /// Constructs new uBigInteger and sets Value to the absolute-value of value.
        /// </summary>
        /// <param name="value"></param>
        public uBigInteger(BigInteger value)
        {
            Value = BigInteger.Abs(value);
        }
        #endregion
        
        #region public Methods
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public override bool Equals(object other) => other is uBigInteger ubig ? Value.Equals(ubig.Value) : Value.Equals(other);

        public int CompareTo(object obj)
        {
            if (obj is uBigInteger ubig) { return Value.CompareTo((object)ubig.Value); }

            return Value.CompareTo(obj);
        }

        public int CompareTo([AllowNull] uBigInteger other) => Value.CompareTo(other.Value);

        public bool Equals([AllowNull] uBigInteger other) => Value.Equals(other.Value);
        #endregion
        
        #region public static Methods
        #region parse
        public static uBigInteger Parse(string s) => BigInteger.Parse(s);
        public static bool TryParse(string s, out uBigInteger result)
        {
            if (BigInteger.TryParse(s, out BigInteger big))
            {
                result = (uBigInteger)big;
                return true;
            }
            else
            {
                result = (uBigInteger)big;
                return false;
            }
        }
        #endregion

        #region casts
        public static implicit operator BigInteger(uBigInteger ubig) => ubig.Value;
        public static implicit operator uBigInteger(BigInteger big) => new uBigInteger(big);

        public static implicit operator uBigInteger(ulong ul) => new uBigInteger(ul);
        public static implicit operator uBigInteger(long l) => new uBigInteger(l);
        public static implicit operator uBigInteger(uint ui) => new uBigInteger(ui);
        public static implicit operator uBigInteger(int i) => new uBigInteger(i);
        public static implicit operator uBigInteger(Int16 i16) => new uBigInteger(i16);

        public static explicit operator uBigInteger(float f) => new BigInteger(f);
        public static explicit operator uBigInteger(double d) => new BigInteger(d);
        public static explicit operator uBigInteger(decimal dec) => new BigInteger(dec);

        public static explicit operator ulong(uBigInteger ubig) => (ulong)ubig.Value;
        public static explicit operator long(uBigInteger ubig) => (long)ubig.Value;
        public static explicit operator uint(uBigInteger ubig) => (uint)ubig.Value;
        public static explicit operator int(uBigInteger ubig) => (int)ubig.Value;
        public static explicit operator Int16(uBigInteger ubig) => (Int16)ubig.Value;

        public static explicit operator float(uBigInteger ubig) => (float)ubig.Value;
        public static explicit operator double(uBigInteger ubig) => (double)ubig.Value;
        public static explicit operator decimal(uBigInteger ubig) => (decimal)ubig.Value;
        #endregion

        #region Operators
        public static uBigInteger operator +(uBigInteger self, uBigInteger other) => self.Value + other.Value;
        public static uBigInteger operator -(uBigInteger self, uBigInteger other) => self.Value - other.Value;
        public static uBigInteger operator *(uBigInteger self, uBigInteger other) => self.Value * other.Value;
        public static uBigInteger operator /(uBigInteger self, uBigInteger other) => self.Value / other.Value;
        public static uBigInteger operator %(uBigInteger self, uBigInteger other) => self.Value % other.Value;
        public static uBigInteger operator +(uBigInteger self) => self;
        public static uBigInteger operator ++(uBigInteger self) => self.Value + 1;
        public static uBigInteger operator --(uBigInteger self) => self.Value - 1;

        public static bool operator >(uBigInteger self, uBigInteger other)  => self.Value > other.Value;
        public static bool operator >=(uBigInteger self, uBigInteger other) => self.Value >= other.Value;
        public static bool operator <(uBigInteger self, uBigInteger other)  => self.Value < other.Value;
        public static bool operator <=(uBigInteger self, uBigInteger other) => self.Value <= other.Value;
        public static bool operator ==(uBigInteger self, uBigInteger other) => self.Value == other.Value;
        public static bool operator !=(uBigInteger self, uBigInteger other) => self.Value != other.Value;
        #endregion

        public static uBigInteger Abs(uBigInteger value) => value; //* What's the point!

        public static int Compare(uBigInteger left, uBigInteger right) => BigInteger.Compare(left.Value, right.Value);
        public static bool Equals(uBigInteger left, uBigInteger right) => BigInteger.Equals(left.Value, right.Value);
        public static uBigInteger Max(uBigInteger left, uBigInteger right) => BigInteger.Max(left.Value, right.Value);
        public static uBigInteger Min(uBigInteger left, uBigInteger right) => BigInteger.Min(left.Value, right.Value);

        public static uBigInteger GreatestCommonDivisor(uBigInteger left, uBigInteger right) => BigInteger.GreatestCommonDivisor(left.Value, right.Value);
        public static double Log(uBigInteger value) => BigInteger.Log(value);
        public static double Log10(uBigInteger value) => BigInteger.Log10(value);
        public static double Log(uBigInteger value, double baseValue) => BigInteger.Log(value, baseValue);

        public static uBigInteger Add(uBigInteger left, uBigInteger right)      => left + right;
        public static uBigInteger Subtract(uBigInteger left, uBigInteger right) => left - right;
        public static uBigInteger Multiply(uBigInteger left, uBigInteger right) => left * right;
        public static uBigInteger Divide(uBigInteger left, uBigInteger right)   => left / right;
        public static uBigInteger Remainder(uBigInteger dividend, uBigInteger divisor) => BigInteger.Remainder(dividend, divisor);
        public static uBigInteger DivRem(uBigInteger dividend, uBigInteger divisor, out uBigInteger remainder) => uBigInteger.DivRem(dividend, divisor, out remainder);
        public static uBigInteger Pow(uBigInteger value, int exponent) => BigInteger.Pow(value, exponent);
        

        public static uBigInteger ModPow(uBigInteger value, uBigInteger exponent, uBigInteger modulus) => BigInteger.ModPow(value, exponent, modulus);
        #endregion
        
        #region public static Properties
        public static uBigInteger Zero { get => BigInteger.Zero; }
        public static uBigInteger One { get => BigInteger.One; }
        #endregion
    }
}
