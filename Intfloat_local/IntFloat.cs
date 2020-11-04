using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace MathTypes
{
    /// <summary>
    /// Combination BigInteger and double (like in python). Automatically chooses best way of storing value based on context.
    /// </summary>
    /// 
    /// <Author>
    /// Jesse Russell
    /// </Author>
    public struct IntFloat : IComparable, IComparable<IntFloat>, IEquatable<IntFloat>
    {
        #region private Fields
        private readonly double floating;
        private readonly BigInteger integer;
        private readonly bool floatNotInt;

        private const double floating_default = 0.0;
        private const int integer_default = 0;
        #endregion
        #region public Properties
        public double Float { get => floatNotInt ? floating : (double)integer; }
        public readonly BigInteger Int { get => !floatNotInt ? integer : (BigInteger)floating; }
        public bool IsFloat { get => floatNotInt; }
        public bool IsInt { get => !floatNotInt; }
        #endregion

        #region public Constructors
        public IntFloat(BigInteger value)
        {
            integer = value;
            floating = floating_default;
            floatNotInt = false;
        }

        public IntFloat(double value)
        {
            integer = integer_default;
            floating = value;
            floatNotInt = true;
        }

        public IntFloat(decimal dec)
        {
            if (dec == Math.Floor(dec))
            {
                integer = new BigInteger(dec);
                floating = floating_default;
                floatNotInt = false;
            }
            else
            {
                integer = integer_default;
                floating = (double)dec;
                floatNotInt = true;
            }
        }

        public IntFloat(int integer) : this((BigInteger)integer) { }
        public IntFloat(long integer) : this((BigInteger)integer) { }
        public IntFloat(uint integer) : this((BigInteger)integer) { }
        public IntFloat(ulong integer) : this((BigInteger)integer) { }
        #endregion

        #region Methods
        public override int GetHashCode()
        {
            return floatNotInt ? HashCode.Combine(floating) : HashCode.Combine(integer);
        }
        public override bool Equals(object obj)
        {
            return floatNotInt ? floating.Equals(obj) : integer.Equals(obj);
        }
        #region Math
        public IntFloat Add(IntFloat other)
        {
            if (floatNotInt || other.floatNotInt)
            {
                return Float + other.Float;
            }
            else
            {
                return integer + other.integer;
            }
        }

        public IntFloat Subtract(IntFloat other)
        {
            if (floatNotInt || other.floatNotInt)
            {
                return Float - other.Float;
            }
            else
            {
                return integer - other.integer;
            }
        }

        public IntFloat Multiply(IntFloat other)
        {
            if (floatNotInt || other.floatNotInt)
            {
                return Float * other.Float;
            }
            else
            {
                return integer * other.integer;
            }
        }

        public IntFloat Divide(IntFloat other)
        {
            if (floatNotInt || other.floatNotInt)
            {
                return Float / other.Float;
            }
            else
            {
                if (integer % other.integer == 0)
                {
                    return integer / other.integer;
                }
                else
                {
                    return Float / other.Float;
                }
            }
        }
        public IntFloat FloorDivide(IntFloat other)
        {
            if (floatNotInt || other.floatNotInt)
            {
                return (BigInteger)Math.Floor(Float / other.Float);
            }
            else
            {
                return BigInteger.Divide(this.integer, other.integer);
            }
        }

        public IntFloat Modulus(IntFloat other)
        {
            if (floatNotInt || other.floatNotInt)
            {
                return Float % other.Float;
            }
            else
            {
                return integer % other.integer;
            }
        }

        public IntFloat Raise(IntFloat Exponent)
        {
            if (Exponent < 0 || floatNotInt || Exponent.floatNotInt)
            {
                return Math.Pow(Float, Exponent.Float);
            }
            else
            {
                if (Exponent.integer > int.MaxValue)
                {
                    // Exponent is too big

                    if (Exponent.integer.IsEven)
                    {
                        return double.PositiveInfinity;
                    }
                    else
                    {
                        return integer.IsEven ? double.PositiveInfinity : double.NegativeInfinity;
                    }
                }
                else
                {
                    return BigInteger.Pow(integer, (int)Exponent.integer);
                }
            }
        }

        public IntFloat Negate()
        {
            return floatNotInt ? new IntFloat(-floating) : new IntFloat(-integer);
        }
        public IntFloat Increment()
        {
            return floatNotInt ? new IntFloat(floating + 1.0) : new IntFloat(integer + 1);
        }
        public IntFloat Decrement()
        {
            return floatNotInt ? new IntFloat(floating - 1.0) : new IntFloat(integer - 1);
        }
        #endregion
        public override string ToString()
        {
            return floatNotInt ? floating.ToString() : integer.ToString();
        }

        #region Interface compliance
        public bool Equals(IntFloat other)
        {
            try
            {
                if (!floatNotInt)
                {
                    return integer == other.Int;
                }

                if (!other.floatNotInt)
                {
                    return Int == other.integer;
                }
            }
            catch (OverflowException e)
            {
                return false;
            }

            return floating == other.floating;
        }

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

        public int CompareTo(object obj)
        {
            if (obj is IntFloat inf) { return CompareTo(inf); }

            if (obj is double d) { return Float.CompareTo(d); }
            if (obj is BigInteger big) { return Int.CompareTo(big); }

            if (obj is decimal dec) { return Float.CompareTo(dec); }
            if (obj is float f) { return Float.CompareTo(f); }

            if (obj is ulong ul) { return Int.CompareTo(ul); }
            if (obj is long l) { return Int.CompareTo(l); }
            if (obj is uint ui) { return Int.CompareTo(ui); }
            if (obj is int i) { return Int.CompareTo(i); }
            if (obj is Int16 i16) { return Int.CompareTo(i16); }

            throw new ArgumentException("The parameter must be a floating point number or integer. (Parameter 'obj')");
        }
        #endregion
        #endregion

        #region Static
        #region Math
        public static IntFloat Floor(IntFloat value)
        {
            if (value.floatNotInt)
            {
                return Math.Floor(value.floating);
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
                return Math.Ceiling(value.floating);
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
        public static IntFloat Pow(IntFloat value, IntFloat Exponent)
        {
            return value.Raise(Exponent);
        }
        #endregion
        #region parse
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
        public static readonly IntFloat Default = new IntFloat(0.0);
        public static readonly IntFloat PositiveInfinity = double.PositiveInfinity;
        public static readonly IntFloat NegativeInfinity = double.NegativeInfinity;
        public static readonly IntFloat NaN = double.NaN;
        public static readonly IntFloat Epsilon = double.Epsilon;

        #region math
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

        #region casts
        public static implicit operator IntFloat(BigInteger big) => new IntFloat(big);
        public static implicit operator IntFloat(long i) => new IntFloat(i);
        public static implicit operator IntFloat(int i) => new IntFloat(i);
        public static implicit operator IntFloat(double d) => new IntFloat(d);
        public static explicit operator IntFloat(decimal d) => new IntFloat(d);

        public static explicit operator BigInteger(IntFloat i) => i.Int;
        public static explicit operator long(IntFloat i) => (long)i.Int;
        public static explicit operator int(IntFloat i) => (int)i.Int;

        public static explicit operator double(IntFloat i) => i.Float;
        public static explicit operator float(IntFloat i) => (float)i.Float;
        #endregion

        #region operators
        public static IntFloat operator +(IntFloat self, IntFloat other) => self.Add(other);
        public static IntFloat operator -(IntFloat self, IntFloat other) => self.Subtract(other);
        public static IntFloat operator *(IntFloat self, IntFloat other) => self.Multiply(other);
        public static IntFloat operator /(IntFloat self, IntFloat other) => self.Divide(other);
        public static IntFloat operator %(IntFloat self, IntFloat other) => self.Modulus(other);
        public static IntFloat operator -(IntFloat self) => self.Negate();
        public static IntFloat operator +(IntFloat self) => self;
        public static IntFloat operator ++(IntFloat self) => self.Increment();
        public static IntFloat operator --(IntFloat self) => self.Decrement();

        public static bool operator >(IntFloat self, IntFloat other) => self.CompareTo(other) > 0;
        public static bool operator >=(IntFloat self, IntFloat other) => self > other || self == other;
        public static bool operator <(IntFloat self, IntFloat other) => self.CompareTo(other) < 0;
        public static bool operator <=(IntFloat self, IntFloat other) => self < other || self == other;
        public static bool operator ==(IntFloat self, IntFloat other) => self.Equals(other);
        public static bool operator !=(IntFloat self, IntFloat other) => !self.Equals(other);
        #endregion

        #endregion
    }
}
