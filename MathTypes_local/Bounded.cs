using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MathTypes
{
    class Bounded<T> where T : IComparable<T>
    {
        #region public Properties
        public T Value
        {
            get => value;
            set => this.value = Check(value);
        }
        public T LowerBound { get => lowerBound; set => lowerBound = value; }
        public T UpperBound { get => upperBound; set => upperBound = value; }
        public Func<(T currentValue, T newValue), (T lowerBound, T upperBound), T> CheckBehavior { get; set; } = FailCheck;
        #endregion

        #region public Methods
        public T Check(T newValue)
        {
            return CheckBehavior((value, newValue), (lowerBound, upperBound));
        }
        #endregion

        #region private static Methods
        private static T FailCheck((T currentValue, T newValue) val, (T lowerBound, T upperBound) bound)
        {
            if (val.newValue.CompareTo(bound.lowerBound) >= 0 && val.newValue.CompareTo(bound.upperBound) < 0)
            {
                return val.newValue;
            }
            else
            {
                return val.currentValue;
            }
        }
        #endregion

        #region private Fields
        private T value;
        private T lowerBound;
        private T upperBound;
        #endregion
    }
}
