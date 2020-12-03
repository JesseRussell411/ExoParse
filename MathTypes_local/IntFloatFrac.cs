using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace MathTypes
{
    public struct IntFloatFrac : IComparable<BigInteger>, IComparable<double>, IComparable<Fraction>
    {
        #region public readonly Fields
        public readonly object Value;
        #endregion

        #region public derived Properties
        public int Sign => Value switch
        {
            BigInteger bi => bi.Sign,
            double d => Math.Sign(d),
            Fraction f => f.Sign,
            _ => throw NotValid
        };
        public bool IsInt => Value is BigInteger;
        public bool IsFloat => Value is double;
        public bool IsFraction => Value is Fraction;
        public Type Type => Value.GetType();
        public BigInteger Int => Value switch
        {
            BigInteger bi => bi,
            double d => (BigInteger)d,
            Fraction f => (BigInteger)f,
            _ => throw NotValid
        };

        public double Float => Value switch
        {
            BigInteger bi => (double)bi,
            double d => d,
            Fraction f => f.ToDouble(),
            _ => throw NotValid
        };

        public Fraction Fraction => Value switch
        {
            BigInteger bi => bi,
            double d => Fraction.FromDouble(d),
            Fraction f => f,
            _ => throw NotValid
        };
        #endregion

        #region public Constructors
        public IntFloatFrac(BigInteger value)
        {
            Value = value;
        }

        public IntFloatFrac(double value)
        {
            Value = value;
        }

        public IntFloatFrac(Fraction value)
        {
            Value = value;
        }
        #endregion

        #region public Methods
        #region Comparison
        public int CompareTo(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => CompareTo(bi),
            double d => CompareTo(d),
            Fraction f => CompareTo(f),
            _ => throw NotValid
        };

        public int CompareTo(BigInteger bi) => Int.CompareTo(bi);
        public int CompareTo(double d) => Float.CompareTo(d);
        public int CompareTo(Fraction f) => Fraction.CompareTo(f);

        public bool Equals(BigInteger bi)
        {
            if (Value is BigInteger self) return self.Equals(bi);
            else if (Value is Fraction f) return f.Equals(bi);
            else if (Value is double d) return d % 1 == 0 && new BigInteger(d).Equals(bi);
            else return false;
        }
        public bool Equals(double d)
        {
            if (Value is double self) return self.Equals(d);
            else if (Value is Fraction f) return f.ToDouble().Equals(d);
            else if (Value is BigInteger bi) return d % 1 == 0 && bi.Equals(new BigInteger(d));
            else return false;
        }
        public bool Equals(Fraction f)
        {
            if (Value is Fraction self) return self.Equals(f);
            else if (Value is BigInteger bi) return new Fraction(bi).Equals(f);
            else if (Value is double d) return f.ToDouble().Equals(d);
            else return false;
        }

        public bool Equals(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => Equals(bi),
            double d => Equals(d),
            Fraction f => Equals(f),
            _ => throw NotValid
        };

        public override bool Equals(Object obj) => obj switch
        {
            BigInteger bi => Equals(bi),
            double d => Equals(d),
            Fraction f => Equals(f),
            float f => Equals((double)f),

            sbyte sb => Equals((BigInteger)sb),
            short s => Equals((BigInteger)s),
            int i => Equals((BigInteger)i),
            long l => Equals((BigInteger)l),

            byte b => Equals((BigInteger)b),
            ushort us => Equals((BigInteger)us),
            uint ui => Equals((BigInteger)ui),
            ulong ul => Equals((BigInteger)ul),
            UBigInteger ubig => Equals((BigInteger)ubig),
            _ => throw new ArgumentException("Parameter must be a fraction, integer, double, or float.")
        };
        public override int GetHashCode() => Value.GetHashCode();
        #endregion
        public override string ToString() => Value.ToString();
        #endregion

        #region public static Methods
        #region Parse
        public static bool TryParse(string s, out IntFloatFrac result)
        {
            BigInteger bi;
            double d;
            Fraction f;
            if (BigInteger.TryParse(s, out bi))
            {
                result = bi;
                return true;
            }
            else if (double.TryParse(s, out d))
            {
                result = d;
                return true;
            }
            else if (Fraction.TryParse(s, out f))
            {
                result = f;
                return true;
            }
            else
            {
                result = Default;
                return false;
            }
        }

        public static IntFloatFrac Parse(string s)
        {
            if (TryParse(s, out IntFloatFrac result))
            {
                return result;
            }
            else
            {
                throw new FormatException("The string is not a valid BigInteger, double, or Fraction.");
            }
        }
        #endregion
        #region Double Pass Through Methods
        public bool IsPositiveInfinity(IntFloatFrac iff) => iff.Value is double d && double.IsPositiveInfinity(d);
        public bool IsNegativeInfinity(IntFloatFrac iff) => iff.Value is double d && double.IsNegativeInfinity(d);
        public bool IsFinite(IntFloatFrac iff) => iff.Value is BigInteger || iff.Value is Fraction || (iff.Value is double d && double.IsFinite(d));
        public bool IsNegative(IntFloatFrac iff) => iff.Value is double d && double.IsNegative(d);
        public bool IsNaN(IntFloatFrac iff) => iff.Value is double d && double.IsNaN(d);
        #endregion

        #region Math
        public static IntFloatFrac Add(IntFloatFrac left, IntFloatFrac right)
        {
            if (left.IsFloat || right.IsFloat) return left.Float + right.Float;
            if (left.IsFraction || right.IsFraction) return left.Fraction + right.Fraction;
            return left.Int + right.Int;
        }
        public static IntFloatFrac Subtract(IntFloatFrac left, IntFloatFrac right)
        {
            if (left.IsFloat || right.IsFloat) return left.Float - right.Float;
            if (left.IsFraction || right.IsFraction) return left.Fraction - right.Fraction;
            return left.Int - right.Int;
        }
        public static IntFloatFrac Multiply(IntFloatFrac left, IntFloatFrac right)
        {
            if (left.IsFloat || right.IsFloat) return left.Float * right.Float;
            if (left.IsFraction || right.IsFraction) return left.Fraction * right.Fraction;
            return left.Int * right.Int;
        }
        public static IntFloatFrac Divide(IntFloatFrac left, IntFloatFrac right)
        {
            if (left.IsFloat || right.IsFloat) return left.Float / right.Float;
            if (left.IsFraction || right.IsFraction) return left.Fraction / right.Fraction;
            return left.Fraction / right.Fraction;
        }
        public static IntFloatFrac FloorDivide(IntFloatFrac left, IntFloatFrac right)
        {
            if (left.IsFloat || right.IsFloat) return Math.Floor(left.Float / right.Float);
            if (left.IsFraction || right.IsFraction) return left.Fraction.Divide(right.Fraction).Floor();
            return left.Int / right.Int;
        }
        public static IntFloatFrac Remainder(IntFloatFrac left, IntFloatFrac right)
        {
            if (left.IsFloat || right.IsFloat) return left.Float % right.Float;
            if (left.IsFraction || right.IsFraction) return left.Fraction % right.Fraction;
            return left.Int % right.Int;
        }
        public static IntFloatFrac Increment(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => bi + 1,
            double d => d + 1.0,
            Fraction f => f.Increment(),
            _ => throw NotValid
        };
        public static IntFloatFrac Decrement(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => bi - 1,
            double d => d - 1.0,
            Fraction f => f.Decrement(),
            _ => throw NotValid
        };
        public static IntFloatFrac Pow(IntFloatFrac iff, int e) => iff.Value switch
        {
            BigInteger bi => BigInteger.Pow(bi, e),
            double d => Math.Pow(d, e),
            Fraction f => Fraction.Pow(f, e),
            _ => throw NotValid
        };
        public static double Pow(IntFloatFrac x, IntFloatFrac y) => Math.Pow(x.Float, y.Float);
        public static IntFloatFrac Neg(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => -bi,
            double d => -d,
            Fraction f => -f,
            _ => throw NotValid
        };
        public static IntFloatFrac Floor(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => bi,
            double d => Math.Floor(d),
            Fraction f => f.Floor().ToBigInteger(),
            _ => throw NotValid
        };
        public static IntFloatFrac Ceiling(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => bi,
            double d => Math.Ceiling(d),
            Fraction f => f.Ceiling().ToBigInteger(),
            _ => throw NotValid
        };
        public static IntFloatFrac Truncate(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => bi,
            double d => Math.Truncate(d),
            Fraction f => f.ToBigInteger(),
            _ => throw NotValid
        };

        public static IntFloatFrac Log(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => BigInteger.Log(bi),
            double d => Math.Log(d),
            Fraction f => Math.Log(f.ToDouble()),
            _ => throw NotValid
        };
        public static IntFloatFrac Log(IntFloatFrac iff, double newBase) => iff.Value switch
        {
            BigInteger bi => BigInteger.Log(bi, newBase),
            double d => Math.Log(d, newBase),
            Fraction f => Math.Log(f.ToDouble(), newBase),
            _ => throw NotValid
        };
        public static IntFloatFrac Log10(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => BigInteger.Log10(bi),
            double d => Math.Log10(d),
            Fraction f => Fraction.Log10(f),
            _ => throw NotValid
        };
        public static IntFloatFrac Log2(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => BigInteger.Log(bi, 2),
            double d => Math.Log2(d),
            Fraction f => Fraction.Log2(f),
            _ => throw NotValid
        };
        public static IntFloatFrac Abs(IntFloatFrac iff) => iff.Value switch
        {
            BigInteger bi => BigInteger.Abs(bi),
            double d => Math.Abs(d),
            Fraction f => Fraction.Abs(f),
            _ => throw NotValid
        };
        public static IntFloatFrac Min(IntFloatFrac a, IntFloatFrac b) => a < b ? a : b;
        public static IntFloatFrac Max(IntFloatFrac a, IntFloatFrac b) => a > b ? a : b;
        #endregion

        #region Operators
        public static IntFloatFrac operator +(IntFloatFrac iff) => iff;
        public static IntFloatFrac operator -(IntFloatFrac iff) => Neg(iff);

        public static IntFloatFrac operator +(IntFloatFrac left, IntFloatFrac right) => Add(left, right);
        public static IntFloatFrac operator -(IntFloatFrac left, IntFloatFrac right) => Subtract(left, right);
        public static IntFloatFrac operator *(IntFloatFrac left, IntFloatFrac right) => Multiply(left, right);
        public static IntFloatFrac operator /(IntFloatFrac left, IntFloatFrac right) => Divide(left, right);
        public static IntFloatFrac operator %(IntFloatFrac left, IntFloatFrac right) => Divide(left, right);

        public static bool operator ==(IntFloatFrac left, IntFloatFrac right) => left.Equals(right);
        public static bool operator !=(IntFloatFrac left, IntFloatFrac right) => !left.Equals(right);

        public static bool operator >(IntFloatFrac left, IntFloatFrac right) => left.CompareTo(right) > 0;
        public static bool operator >=(IntFloatFrac left, IntFloatFrac right) => left.CompareTo(right) >= 0;
        public static bool operator <(IntFloatFrac left, IntFloatFrac right) => left.CompareTo(right) < 0;
        public static bool operator <=(IntFloatFrac left, IntFloatFrac right) => left.CompareTo(right) <= 0;

        #endregion

        #region Casts
        public static implicit operator IntFloatFrac(BigInteger bi) => new IntFloatFrac(bi);
        public static implicit operator IntFloatFrac(double d) => new IntFloatFrac(d);
        public static implicit operator IntFloatFrac(Fraction f) => new IntFloatFrac(f);
        public static implicit operator IntFloatFrac(FractionOperation fo) => new IntFloatFrac(fo);

        public static implicit operator IntFloatFrac(sbyte sb) => new IntFloatFrac((BigInteger)sb);
        public static implicit operator IntFloatFrac(short s) => new IntFloatFrac((BigInteger)s);
        public static implicit operator IntFloatFrac(int i) => new IntFloatFrac((BigInteger)i);
        public static implicit operator IntFloatFrac(long l) => new IntFloatFrac((BigInteger)l);

        public static implicit operator IntFloatFrac(byte b) => new IntFloatFrac((BigInteger)b);
        public static implicit operator IntFloatFrac(ushort us) => new IntFloatFrac((BigInteger)us);
        public static implicit operator IntFloatFrac(uint ui) => new IntFloatFrac((BigInteger)ui);
        public static implicit operator IntFloatFrac(ulong ul) => new IntFloatFrac((BigInteger)ul);


        public static explicit operator BigInteger(IntFloatFrac iff) => iff.Int;
        public static implicit operator double(IntFloatFrac iff) => iff.Float;
        public static explicit operator Fraction(IntFloatFrac iff) => iff.Fraction;

        public static implicit operator IntFloatFrac(IntFloat ift) => ift.Value switch { BigInteger bi => bi, double d => d, _ => Default };
        public static explicit operator IntFloat(IntFloatFrac iff) => iff.Value switch { BigInteger bi => bi, double d => d, Fraction f => f.ToDouble(), _ => throw NotValid };
        #endregion
        #endregion

        #region private static Properties
        private static Exception NotValid => new Exception("Value is not valid because it is neither a BigInteger, double, nor fraction. If you encounter this exception, and you didn't use the parameterless constructor, please contact the developer of this type.");
        #endregion

        #region public static readonly Fields
        public static readonly IntFloatFrac Default = 0.0;
        #endregion

        #region public static get only Properties
        public static IntFloatFrac PositiveInfinity { get => double.PositiveInfinity; }
        public static IntFloatFrac NegativeInfinity { get => double.NegativeInfinity; }
        public static IntFloatFrac NaN { get => double.NaN; }
        public static IntFloatFrac Epsilon { get => double.Epsilon; }
        #endregion

    }
}
