using Newtonsoft.Json;

namespace Wireform.Circuitry.Data
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
        /// Checks if the inputs are valid
        /// </summary>
        /// <param name="returnValue">If INVALID, this is the type of invalidity</param>
        /// <returns>true if INVALID</returns>
        private static bool CheckInputs(out BitValue returnValue, params BitValue[] values)
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
                returnValue = default;
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
            return new BitValue((byte)value);
        }

        public static implicit operator BitValue(long value)
        {
            return new BitValue((byte)value);
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
            switch (Selected)
            {
                case 0:
                    return "Nothing";
                case 1:
                    return "Error";
                case 2:
                    return "Zero";
                case 3:
                    return "One";
            }
            throw new System.Exception();
        }
    }
}