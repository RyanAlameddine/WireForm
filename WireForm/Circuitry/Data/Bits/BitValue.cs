using Newtonsoft.Json;
using System;

namespace Wireform.Circuitry.Data.Bits
{
    public struct BitValue
    {
        public const int Nothing = 0;
        public const int Error = 1;
        public const int Zero = 2;
        public const int One = 3;

        public readonly byte Selected;

        [JsonConstructor]
        private BitValue(byte Selected)
        {
            this.Selected = Selected;
        }

        public static BitValue operator !(BitValue value)
        {
            if (value == One) return Zero;
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
        /// Checks if the inputs are valid (not nothing or error)
        /// </summary>
        /// <param name="invalidType">If invalid, this is the type of invalidity (error/nothing)</param>
        /// <returns>true if invalid</returns>
        private static bool CheckInputs(out BitValue invalidType, params BitValue[] values)
        {
            bool valid = true;
            bool error = false;
            foreach (BitValue value in values)
            {

                if (value == Nothing)
                {
                    valid = false;
                }
                else if (value == Error)
                {
                    valid = false;
                    error = true;
                }
            }

            if (valid)
            {
                invalidType = default;
                return false;
            }
            else if (error)
            {
                invalidType = Error;
                return true;
            }
            else
            {
                invalidType = Nothing;
                return true;
            }
        }


        public static implicit operator BitValue(int value)
        {
            return new BitValue((byte)value);
        }

        public static implicit operator BitValue(long value)
        {
            return new BitValue((byte)value);
        }

        public static bool operator ==(BitValue value, BitValue value2)
        {
            return value.Equals(value2);
        }

        public static bool operator !=(BitValue value, BitValue value2)
        {
            return !value.Equals(value2);
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

        public override string ToString()
        {
            return Selected switch
            {
                0 => "Nothing",
                1 => "Error",
                2 => "Zero",
                3 => "One",
                _ => throw new Exception(),
            };
        }

        public char ToChar()
        {
            return Selected switch
            {
                0 => '-',
                1 => 'e',
                2 => '0',
                3 => '1',
                _ => throw new Exception(),
            };
        }
    }
}