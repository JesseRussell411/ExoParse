using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;

namespace MathTypes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Doudec : IComparable<Doudec>, IComparable<double>, IComparable<decimal>, IEquatable<Doudec>, IEquatable<double>, IEquatable<decimal>
    {
        #region public Constructors
        public Doudec(double d)
        {
            doubleNotDecimal = true;
            decim = default;
            doub = d;
        }
        public Doudec(decimal dec)
        {
            doubleNotDecimal = false;
            doub = default;
            decim = dec;
        }
        #endregion

        #region public Properties
        public double Double => doubleNotDecimal ? doub : Convert.ToDouble(decim);
        public decimal Decimal => doubleNotDecimal ? Convert.ToDecimal(doub) : decim;
        public object Value => doubleNotDecimal ? (object)doub : decim;
        public bool IsDouble => doubleNotDecimal;
        public bool IsDecimal => !doubleNotDecimal;
        #endregion

        #region public Methods
        #region Math
        public Doudec Increment() => doubleNotDecimal ? (Doudec)doub + 1 : decim + 1;
        public Doudec Decrement() => doubleNotDecimal ? (Doudec)doub - 1 : decim - 1;
        #endregion

        #region Comparison
        public int CompareTo(Doudec other)
        {
            if (doubleNotDecimal || other.doubleNotDecimal) return Double.CompareTo(other.Double);
            else return decim.CompareTo(other.decim);
        }
        public int CompareTo(double d) => Double.CompareTo(d);
        public int CompareTo(decimal dec) => doubleNotDecimal ? doub.CompareTo(Convert.ToDouble(dec)) : decim.CompareTo(dec);
        public bool Equals(Doudec other)
        {
            if (!doubleNotDecimal)
            {
                if (other.TryToDecimal(out decimal other_dec))
                {
                    return decim.Equals(other_dec);
                }
                else return false;
            }
            else if (!other.doubleNotDecimal)
            {
                if (TryToDecimal(out decimal this_dec))
                {
                    return this_dec.Equals(other.decim);
                }
                else return false;
            }
            else
            {
                return doub.Equals(other.doub);
            }
        }
        public bool Equals(double d) => doub.Equals(d);
        public bool Equals(decimal dec) => TryToDecimal(out decimal this_dec) && this_dec.Equals(dec);

        public override bool Equals(object obj) => obj switch
        {
            Doudec dd => Equals(dd),
            double d => Equals(d),
            decimal dec => Equals(dec),
            _ => throw new ArgumentException("Argument must be a Doudec, double, or decimal")
        };

        public override int GetHashCode()
        {
            if (TryToDecimal(out decimal result))
            {
                return result.GetHashCode();
            }
            else
            {
                return Double.GetHashCode();
            }
        }

        public override string ToString() => doubleNotDecimal ? doub.ToString() : decim.ToString();
        #endregion
        public bool TryToDecimal(out decimal result)
        {
            if (!doubleNotDecimal)
            {
                result = decim;
                return true;
            }
            else
            {
                return MathUtils.TryToDecimal(doub, out result);
            }
        }
        #endregion

        #region public static Methods
        #region Casts
        public static implicit operator Doudec(double d) => FromDouble(d);
        public static implicit operator Doudec(decimal dec) => new Doudec(dec);
        public static implicit operator Doudec(float f) => FromDouble(f);

        public static implicit operator Doudec(sbyte i) => new Doudec((decimal)i);
        public static implicit operator Doudec(short i) => new Doudec((decimal)i);
        public static implicit operator Doudec(int i) => new Doudec((decimal)i);
        public static implicit operator Doudec(long i) => new Doudec((decimal)i);

        public static implicit operator Doudec(byte i) => new Doudec((decimal)i);
        public static implicit operator Doudec(ushort i) => new Doudec((decimal)i);
        public static implicit operator Doudec(uint i) => new Doudec((decimal)i);
        public static implicit operator Doudec(ulong i) => new Doudec((decimal)i);

        public static explicit operator Doudec(BigInteger bi) => FromBigInteger(bi);
        public static explicit operator Doudec(UBigInteger bi) => FromBigInteger(bi);

        public static implicit operator double(Doudec dd) => dd.Double;
        public static explicit operator decimal(Doudec dd) => dd.Decimal;
        public static explicit operator float(Doudec dd) => (float)dd.Double;

        public static explicit operator BigInteger(Doudec d) => d.doubleNotDecimal ? (BigInteger)d.doub : (BigInteger)d.decim;
        public static explicit operator UBigInteger(Doudec d) => d.doubleNotDecimal ? (UBigInteger)d.doub : (UBigInteger)d.decim;
        #endregion

        #region double Passthrough
        public static bool IsNaN(Doudec d) => d.doubleNotDecimal && double.IsNaN(d.doub);
        public static bool IsInfinity(Doudec d) => d.doubleNotDecimal && double.IsInfinity(d.doub);
        public static bool IsFinite(Doudec d) => !d.doubleNotDecimal || double.IsFinite(d.doub);
        public static bool IsNegative(Doudec d) => d.doubleNotDecimal ? double.IsNegative(d.doub) : d.decim < 0;
        public static bool IsNormal(Doudec d) => d.doubleNotDecimal && double.IsNormal(d.doub);
        public static bool IsSubnormal(Doudec d) => d.doubleNotDecimal && double.IsSubnormal(d.doub);
        public static bool IsPositiveInfinity(Doudec d) => d.doubleNotDecimal && double.IsPositiveInfinity(d.doub);
        public static bool IsNegativeInfinity(Doudec d) => d.doubleNotDecimal && double.IsNegativeInfinity(d.doub);
        #endregion

        #region Parse
        public static bool TryParse(string s, out Doudec result)
        {
            if (decimal.TryParse(s, out decimal dec))
            {
                result = dec;
                return true;
            }
            else if (double.TryParse(s, out double d))
            {
                result = d;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        public Doudec Parse(string s)
        {
            if (TryParse(s, out Doudec result))
            {
                return result;
            }
            else
            {
                throw new FormatException("The string could not be parsed as either a double or a decimal.");
            }
        }
        #endregion

        #region Operators
        public static Doudec operator -(Doudec dd) => Neg(dd);
        public static Doudec operator +(Doudec dd) => dd;

        public static Doudec operator +(Doudec left, Doudec right) => Add(left, right);
        public static Doudec operator -(Doudec left, Doudec right) => Subtract(left, right);
        public static Doudec operator *(Doudec left, Doudec right) => Multiply(left, right);
        public static Doudec operator /(Doudec left, Doudec right) => Divide(left, right);
        public static Doudec operator %(Doudec left, Doudec right) => Remainder(left, right);

        public static Doudec operator ++(Doudec dd) => dd.Increment();
        public static Doudec operator --(Doudec dd) => dd.Decrement();

        public static bool operator ==(Doudec left, Doudec right) => left.Equals(right);
        public static bool operator !=(Doudec left, Doudec right) => !left.Equals(right);

        public static bool operator >(Doudec left, Doudec right) => left.CompareTo(right) > 0;
        public static bool operator >=(Doudec left, Doudec right) => left.CompareTo(right) >= 0;
        public static bool operator <(Doudec left, Doudec right) => left.CompareTo(right) < 0;
        public static bool operator <=(Doudec left, Doudec right) => left.CompareTo(right) <= 0;
        #endregion

        #region Math
        public static Doudec Add(Doudec left, Doudec right)
        {
            if(left.doubleNotDecimal || right.doubleNotDecimal)
            {
                return left.Double + right.Double;
            }
            else
            {
                try
                {
                    return left.decim + right.decim;
                }
                catch(OverflowException oe)
                {
                    return left.Double + right.Double;
                }
            }
        }
        public static Doudec Subtract(Doudec left, Doudec right)
        {
            if(left.doubleNotDecimal || right.doubleNotDecimal)
            {
                return left.Double - right.Double;
            }
            else
            {
                try
                {
                    return left.decim - right.decim;
                }
                catch(OverflowException oe)
                {
                    return left.Double - right.Double;
                }
            }
        }
        public static Doudec Multiply(Doudec left, Doudec right)
        {
            if(left.doubleNotDecimal || right.doubleNotDecimal)
            {
                return left.Double * right.Double;
            }
            else
            {
                try
                {
                    return left.decim * right.decim;
                }
                catch(OverflowException oe)
                {
                    return left.Double * right.Double;
                }
            }
        }
        public static Doudec Divide(Doudec left, Doudec right)
        {
            if(left.doubleNotDecimal || right.doubleNotDecimal)
            {
                return left.Double / right.Double;
            }
            else
            {
                try
                {
                    return left.decim / right.decim;
                }
                catch(OverflowException oe)
                {
                    return left.Double / right.Double;
                }
            }
        }
        public static Doudec Remainder(Doudec left, Doudec right)
        {
            if(left.doubleNotDecimal || right.doubleNotDecimal)
            {
                return left.Double % right.Double;
            }
            else
            {
                try
                {
                    return left.decim % right.decim;
                }
                catch(OverflowException oe)
                {
                    return left.Double % right.Double;
                }
            }
        }
        public static Doudec Pow(Doudec x, Doudec y) => Math.Pow(x.Double, y.Double);
        public static Doudec Floor(Doudec dd) => dd.doubleNotDecimal ? (Doudec) Math.Floor(dd.Double) : Math.Floor(dd.Decimal);
        public static Doudec Ceiling(Doudec dd) => dd.doubleNotDecimal ? (Doudec) Math.Ceiling(dd.Double) : Math.Ceiling(dd.Decimal);
        public static Doudec Truncate(Doudec dd) => dd.doubleNotDecimal ? (Doudec) Math.Truncate(dd.Double) : Math.Truncate(dd.Decimal);
        public static Doudec Abs(Doudec dd) => dd.doubleNotDecimal ? (Doudec) Math.Abs(dd.Double) : Math.Abs(dd.Decimal);
        public static Doudec Log(Doudec x) => Math.Log(x.Double);
        public static Doudec Log(Doudec x, Doudec newbase) => Math.Log(x.Double, newbase.Double);
        public static Doudec Log10(Doudec x) => Math.Log10(x.Double);
        public static Doudec Log2(Doudec x) => Math.Log2(x.Double);
        public static int Sign(Doudec x) => x.doubleNotDecimal ? Math.Sign(x.doub) : Math.Sign(x.decim);
        public static Doudec Neg(Doudec x) => x.doubleNotDecimal ? (Doudec)(-x.doub) : -x.decim;
        #endregion

        #region Factories
        public static Doudec FromDouble(double d) => MathUtils.TryToDecimal(d, out decimal dec) ? new Doudec(dec) : new Doudec(d);
        public static Doudec FromBigInteger(BigInteger bi)
        {
            if (MathUtils.TryToDecimal(bi, out decimal dec))
            {
                return dec;
            }
            else
            {
                return new Doudec((double)bi);
            }
        }
        #endregion
        #endregion

        #region public static Properties { get; }
        public static Doudec NaN => new Doudec(double.NaN);
        public static Doudec PositiveInfinity => new Doudec(double.PositiveInfinity);
        public static Doudec NegativeInfinity => new Doudec(double.NegativeInfinity);
        public static Doudec Epsilon => new Doudec(double.Epsilon);
        #endregion

        #region private readonly Fields
        [FieldOffset(0)]
        private readonly bool doubleNotDecimal;
        [FieldOffset(1)]
        private readonly double doub;
        [FieldOffset(1)]
        private readonly decimal decim;
        #endregion
    }
}
