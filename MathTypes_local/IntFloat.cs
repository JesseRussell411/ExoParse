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
    /// Combination BigInteger and double (a little like python). Automatically chooses best way of storing value based on context.
    /// </summary>
    /// 
    /// <Author>
    /// Jesse Russell
    /// </Author>
    public struct IntFloat : IComparable, IComparable<IntFloat>, IEquatable<IntFloat>
    {
        #region public Properties
        public double Float { get => floatNotInt ? floating : (double)integer; }
        public BigInteger Int { get => !floatNotInt ? integer : (BigInteger)floating; }
        public bool IsFloat { get => floatNotInt; }
        public bool IsInt { get => !floatNotInt; }

        public object Value => floatNotInt ? (object)floating : (object)integer;

        public bool IsNegative { get => floatNotInt ? double.IsNegative(floating) : integer < 0; }
        public bool IsPositive { get => !IsNegative; }

        public bool IsFinite { get => !floatNotInt || double.IsFinite(floating); }
        public bool IsPositiveInfinity { get => floatNotInt && double.IsPositiveInfinity(floating); }
        public bool IsNegativeInfinity { get => floatNotInt && double.IsNegativeInfinity(floating); }
        public bool IsNaN { get => floatNotInt && double.IsNaN(floating); }
        public bool IsNormal { get => floatNotInt && double.IsNormal(floating); }
        public bool IsSubNormal { get => floatNotInt && double.IsSubnormal(floating); }
        #endregion

        #region public Constructors
        public IntFloat(BigInteger value)
        {
            integer = value;
            floating = default;
            floatNotInt = false;
        }

        public IntFloat(double value)
        {
            integer = default;
            floating = value;
            floatNotInt = true;
        }

        public IntFloat(decimal dec)
        {
            if (dec == Math.Floor(dec))
            {
                integer = new BigInteger(dec);
                floating = default;
                floatNotInt = false;
            }
            else
            {
                integer = default;
                floating = (double)dec;
                floatNotInt = true;
            }
        }

        public IntFloat(float f)
        {
            integer = default;
            floating = f;
            floatNotInt = true;
        }

        public IntFloat(sbyte sb) : this((BigInteger)sb) { }
        public IntFloat(short s) : this((BigInteger)s) { }
        public IntFloat(int i) : this((BigInteger)i) { }
        public IntFloat(long l) : this((BigInteger)l) { }
        public IntFloat(byte b) : this((BigInteger)b) { }
        public IntFloat(ushort us) : this((BigInteger) us) { }
        public IntFloat(uint ui) : this((BigInteger)ui) { }
        public IntFloat(ulong ul) : this((BigInteger)ul) { }
        #endregion

        #region public Methods
        #region Comparison
        #region Equals
        public bool Equals(IntFloat other)
        {
            if (floatNotInt && other.floatNotInt)
            {
                return floating == other.floating;
            }
            else if (floatNotInt && !other.floatNotInt)
            {
                if (double.IsFinite(floating))
                {
                    return floating == other.Float;
                }
                else
                {
                    return false;
                }
            }
            else if (!floatNotInt && other.floatNotInt)
            {
                if (double.IsFinite(other.floating))
                {
                    return Float == other.floating;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return integer == other.integer;
            }
        }

        public override bool Equals(object obj)
        {
            return floatNotInt ? floating.Equals(obj) : integer.Equals(obj);
        }
        #endregion
        #region CompareTo
        public int CompareTo(IntFloat other)
        {
            if (floatNotInt)
            {
                return floating.CompareTo(other.Float);
            }
            else
            {
                return integer.CompareTo(other.Int);
            }
        }


        public int CompareTo(double d)
        {
            if (floatNotInt)
            {
                return floating.CompareTo(d);
            }
            else if (double.IsFinite(d))
            {
                return ((double)integer).CompareTo(d);
            }
            else if (double.IsPositiveInfinity(d))
            {
                return -1;
            }
            else if (double.IsNegativeInfinity(d))
            {
                return 1;
            }
            else if (double.IsNaN(d))
            {
                return 1;
            }
            else
            {
                return 1;
            }
        }

        public int CompareTo(BigInteger i)
        {
            if (floatNotInt)
            {
                return floating.CompareTo((double)i);
            }
            else
            {
                return integer.CompareTo(i);
            }
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case IntFloat inf:
                    return CompareTo(inf);
                case double d:
                    return CompareTo(d);
                case float f:
                    return CompareTo(f);
                case BigInteger bi:
                    return CompareTo(bi);
                case long l:
                    return CompareTo((BigInteger)l);
                case int i:
                    return CompareTo((BigInteger)i);
                case short s:
                    return CompareTo((BigInteger)s);
                case sbyte sb:
                    return CompareTo((BigInteger)sb);
                case UBigInteger ubi:
                    return CompareTo((BigInteger)ubi);
                case byte b:
                    return CompareTo((BigInteger)b);
                case ushort us:
                    return CompareTo((BigInteger)us);
                case uint ui:
                    return CompareTo((BigInteger)ui);
                case ulong ul:
                    return CompareTo((BigInteger)ul);
                default:
                    throw new ArgumentException("The parameter must be a float or integer");
            }
            //if (obj is IntFloat inf) { return CompareTo(inf); }

            //if (obj is double d) { return CompareTo(d); }
            //if (obj is BigInteger big) { return CompareTo(big); }
            ////if (obj is decimal dec) { return CompareTo((double)dec); } *could lead to problems
            //if (obj is float f) { return CompareTo((double)f); }

            //if (obj is ulong ul) { return CompareTo((BigInteger)ul); }
            //if (obj is long l) { return CompareTo((BigInteger)l); }
            //if (obj is uint ui) { return CompareTo((BigInteger)ui); }
            //if (obj is int i) { return CompareTo((BigInteger)i); }
            //if (obj is Int16 i16) { return CompareTo((BigInteger)i16); }

            throw new ArgumentException("The parameter must be a float, double, or integer type. (Parameter 'obj')");
        }
        #endregion
        public override int GetHashCode()
        {
            return floatNotInt ? HashCode.Combine(floating) : HashCode.Combine(integer);
        }
        #endregion
        public override string ToString()
        {
            return floatNotInt ? floating.ToString() : integer.ToString();
        }
        #endregion

        #region public static Methods
        #region Math
        public static IntFloat Add(IntFloat left, IntFloat right)
        {
            if (left.floatNotInt || right.floatNotInt)
            {
                return left.Float + right.Float;
            }
            else
            {
                return left.integer + right.integer;
            }
        }

        public static IntFloat Subtract(IntFloat left, IntFloat right)
        {
            if (left.floatNotInt || right.floatNotInt)
            {
                return left.Float - right.Float;
            }
            else
            {
                return left.integer - right.integer;
            }
        }

        public static IntFloat Multiply(IntFloat left, IntFloat right)
        {
            if (left.floatNotInt || right.floatNotInt)
            {
                return left.Float * right.Float;
            }
            else
            {
                return left.integer * right.integer;
            }
        }

        public static IntFloat Divide(IntFloat left, IntFloat right)
        {
            if (left.floatNotInt || right.floatNotInt)
            {
                return left.Float / right.Float;
            }
            else
            {
                if (left.integer % right.integer == 0)
                {
                    return left.integer / right.integer;
                }
                else
                {
                    return left.Float / right.Float;
                }
            }
        }

        public static IntFloat FloorDivide(IntFloat left, IntFloat right)
        {
            if (left.floatNotInt || right.floatNotInt)
            {
                return (BigInteger)Math.Floor(left.Float / right.Float);
            }
            else
            {
                return BigInteger.Divide(left.integer, right.integer);
            }
        }

        public static IntFloat Remainder(IntFloat left, IntFloat right)
        {
            if (left.floatNotInt || right.floatNotInt)
            {
                return left.Float % right.Float;
            }
            else
            {
                return left.integer % right.integer;
            }
        }

        public static IntFloat Pow(IntFloat value, IntFloat expnent)
        {
            if (expnent < 0 || value.floatNotInt || expnent.floatNotInt)
            {
                return Math.Pow(value.Float, expnent.Float);
            }
            else
            {
                if (expnent.integer > int.MaxValue)
                {
                    // Exponent is too big

                    if (expnent.integer.IsEven)
                    {
                        return double.PositiveInfinity;
                    }
                    else
                    {
                        return value.integer.IsEven ? double.PositiveInfinity : double.NegativeInfinity;
                    }
                }
                else
                {
                    return BigInteger.Pow(value.integer, (int)expnent.integer);
                }
            }
        }

        public static IntFloat Negate(IntFloat value)
        {
            return value.floatNotInt ? (IntFloat)(-value.floating) : (IntFloat)(-value.integer);
        }

        public static IntFloat Floor(IntFloat value)
        {
            if (value.floatNotInt)
            {
                return (BigInteger) Math.Floor(value.floating);
            }
            else
            {
                return value;
            }
        }
        public static IntFloat Ceiling(IntFloat value)
        {
            if (value.floatNotInt)
            {
                return (BigInteger) Math.Ceiling(value.floating);
            }
            else
            {
                return value;
            }
        }
        public static IntFloat Abs(IntFloat value)
        {
            if (value.floatNotInt)
            {
                return Math.Abs(value.floating);
            }
            else
            {
                return BigInteger.Abs(value.integer);
            }
        }
        public static IntFloat Log(IntFloat value, IntFloat baseValue)
        {
            return IntFloat.Log(value, baseValue.Float);
        }
        public static IntFloat Log(IntFloat value, double baseValue)
        {
            if (value.floatNotInt)
            {
                return Math.Log(value.floating, baseValue);
            }
            else
            {
                return BigInteger.Log(value.integer, baseValue);
            }
        }
        public static IntFloat Log10(IntFloat value)
        {
            if (value.floatNotInt)
            {
                return Math.Log10(value.floating);
            }
            else
            {
                return BigInteger.Log10(value.integer);
            }
        }
        public static IntFloat Sign(IntFloat value)
        {
            if (value.floatNotInt)
            {
                return Math.Sign(value.floating);
            }
            else
            {
                return value.integer.Sign;
            }
        }
        public static IntFloat Min(IntFloat val1, IntFloat val2)
        {
            if (!val2.floatNotInt)
            {
                return val1 < val2 ? val1 : val2;
            }
            else
            {
                return val2 < val1 ? val2 : val1;
            }
        }
        public static IntFloat Max(IntFloat val1, IntFloat val2)
        {
            if (!val2.floatNotInt)
            {
                return val1 > val2 ? val1 : val2;
            }
            else
            {
                return val2 > val1 ? val2 : val1;
            }
        }
        #endregion
        #region Parse
        public static bool TryParse(string s, out IntFloat result)
        {
            if (BigInteger.TryParse(s, out BigInteger big))
            {
                result = new IntFloat(big);
                return true;
            }

            if (double.TryParse(s, out double d))
            {
                result = new IntFloat(d);
                return true;
            }

            result = IntFloat.Default;
            return false;
        }

        public static IntFloat Parse(string s)
        {
            if (IntFloat.TryParse(s, out IntFloat result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"{s} is not a valid double or BigInteger");
            }
        }
        #endregion
        #region Casts
        public static implicit operator IntFloat(double d) => new IntFloat(d);
        public static implicit operator IntFloat(BigInteger big) => new IntFloat(big);

        public static implicit operator IntFloat(sbyte i) => new IntFloat(i);
        public static implicit operator IntFloat(short i) => new IntFloat(i);
        public static implicit operator IntFloat(int i) => new IntFloat(i);
        public static implicit operator IntFloat(long i) => new IntFloat(i);

        public static implicit operator IntFloat(byte i) => new IntFloat(i);
        public static implicit operator IntFloat(ushort i) => new IntFloat(i);
        public static implicit operator IntFloat(uint i) => new IntFloat(i);
        public static implicit operator IntFloat(ulong i) => new IntFloat(i);

        public static explicit operator IntFloat(decimal d) => new IntFloat(d);


        public static explicit operator BigInteger(IntFloat iflt) => iflt.Int;
        public static explicit operator double(IntFloat iflt) => iflt.Float;
        public static explicit operator float(IntFloat i) => (float)i.Float;

        public static explicit operator sbyte(IntFloat iflt) => (sbyte)iflt.Int;
        public static explicit operator short(IntFloat iflt) => (short)iflt.Int;
        public static explicit operator int(IntFloat iflt) => (int)iflt.Int;
        public static explicit operator long(IntFloat iflt) => (long)iflt.Int;

        public static explicit operator byte(IntFloat iflt) => (byte)iflt.Int;
        public static explicit operator ushort(IntFloat iflt) => (ushort)iflt.Int;
        public static explicit operator uint(IntFloat iflt) => (uint)iflt.Int;
        public static explicit operator ulong(IntFloat iflt) => (ulong)iflt.Int;

        #endregion
        #region Operators
        public static IntFloat operator +(IntFloat left, IntFloat right) => IntFloat.Add(left, right);
        public static IntFloat operator -(IntFloat left, IntFloat right) => IntFloat.Subtract(left, right);
        public static IntFloat operator *(IntFloat left, IntFloat right) => IntFloat.Multiply(left, right);
        public static IntFloat operator /(IntFloat left, IntFloat right) => IntFloat.Divide(left, right);
        public static IntFloat operator %(IntFloat left, IntFloat right) => IntFloat.Remainder(left, right);
        public static IntFloat operator -(IntFloat value) => IntFloat.Negate(value);
        public static IntFloat operator +(IntFloat value) => value;
        public static IntFloat operator ++(IntFloat value) => value + 1;
        public static IntFloat operator --(IntFloat value) => value - 1;

        public static bool operator >(IntFloat left, IntFloat right) => left.CompareTo(right) > 0;
        public static bool operator >=(IntFloat left, IntFloat right) => left > right || left == right;
        public static bool operator <(IntFloat left, IntFloat right) => left.CompareTo(right) < 0;
        public static bool operator <=(IntFloat left, IntFloat right) => left < right || left == right;
        public static bool operator ==(IntFloat left, IntFloat right) => left.Equals(right);
        public static bool operator !=(IntFloat left, IntFloat right) => !left.Equals(right);
        #endregion
        #endregion

        #region public static Properties
        public static IntFloat Default { get => new IntFloat(); }
        public static IntFloat PositiveInfinity { get => double.PositiveInfinity; }
        public static IntFloat NegativeInfinity { get => double.NegativeInfinity; }
        public static IntFloat NaN { get => double.NaN; }
        #endregion

        #region private Fields
        private readonly bool floatNotInt;
        private readonly double floating;
        private readonly BigInteger integer;
        #endregion
    }
}