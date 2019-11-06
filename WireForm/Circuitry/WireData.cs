using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Circuitry
{
    public class WireData
    {
        public BitValue[] BitValues;

        public WireData(int valueCount)
        {
            BitValues = new BitValue[valueCount];
        }
    }

    //public enum BitValue
    //{
    //    Nothing,
    //    Error,
    //    Zero,
    //    One
    //}

    public struct BitValue
    {
        public const int Nothing = 0;
        public const int Error = 1;
        public const int Zero = 2;
        public const int One = 3;

        public readonly int Selected;

        private BitValue(int value)
        {
            Selected = value;
        }

        public static BitValue operator !(BitValue value)
        {
            if (value == One)  return Zero;
            if (value == Zero) return One;
            else return value;
        }

        public static BitValue operator &(BitValue value1, BitValue value2)
        {
            if (CheckInputs(out var ret, value1, value2)) return ret;

            if (value1 == One && value2 == One) return One;
            return Zero;
        }

        public static BitValue operator |(BitValue value1, BitValue value2)
        {
            if (CheckInputs(out var ret, value1, value2)) return ret;

            if (value1 == One || value2 == One) return One;
            return Zero;
        }

        public static BitValue operator ^(BitValue value1, BitValue value2)
        {
            if (CheckInputs(out var ret, value1, value2)) return ret;

            if ((value1 == One || value2 == One) && !(value1 == One && value2 == One)) return One;
            return Zero;
        }


        //Backend

        /// <summary>
        /// Checks if the inputs are valid
        /// </summary>
        /// <param name="returnValue">If INVALID, this is the type of invalidity</param>
        /// <returns>true if INVALID</returns>
        private static bool CheckInputs(out BitValue returnValue, params BitValue[] values)
        {
            bool valid   = true;
            bool error   = false;
            foreach(BitValue value in values)
            {

                if (value == Nothing)
                {
                    valid = false;
                }
                else if(value == Error)
                {
                    valid = false;
                    error = true;
                }
            }

            if (valid)
            {
                returnValue = default(BitValue);
                return false;
            }
            else if (error)
            {
                returnValue = Error;
                return true;
            }
            else
            {
                returnValue = Nothing;
                return true;
            }
        }


        public static implicit operator BitValue(int value)
        {
            return new BitValue(value);
        }

        public static bool operator ==(BitValue value, int valueRep)
        {
            return value.Selected == valueRep;
        }

        public static bool operator !=(BitValue value, int valueRep)
        {
            return value.Selected != valueRep;
        }

        public override bool Equals(object obj)
        {
            return obj is BitValue value &&
                   Selected == value.Selected;
        }

        public override int GetHashCode()
        {
            return 731004340 + Selected.GetHashCode();
        }
    }
}