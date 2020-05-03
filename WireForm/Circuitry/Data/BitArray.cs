using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Wireform.Circuitry.Data
{
    [JsonConverter(typeof(BitArrayConverter))]
    public struct BitArray
    {
        public BitValue[] BitValues;

        [JsonIgnore]
        public int Length { get => BitValues.Length; }

        public BitArray(int Length)
        {
            BitValues = new BitValue[Length];
        }

        public BitArray(BitValue[] values)
        {
            BitValues = values;
        }

        public void Set(int i, BitValue value)
        {
            BitValues[i] = value;
        }

        public BitValue this[int i]
        {
            get
            {
                return BitValues[i];
            }
            set
            {
                BitValues[i] = value;
            }
        }

        /// <summary>
        /// Gets all colors of the bits
        /// </summary>
        public Color[] BitColors()
        {
            Color[] colors = new Color[Length];
            for(int i = 0; i < colors.Length; i++)
            {
                colors[i] = GetBitColor(i);
            }
            return colors;
        }

        /// <summary>
        /// Gets the color of the bit at a specified index
        /// </summary>
        public Color GetBitColor(int index)
        {
            return BitValues[index].Selected switch
            {
                BitValue.Error => Color.DarkRed,
                BitValue.Nothing => Color.DimGray,
                BitValue.One => Color.FromArgb(51, 171, 212),//Color.Blue,
                BitValue.Zero => Color.FromArgb(26, 30, 153),//Color.DarkBlue,

                _ => throw new Exception("BitValue undefined")
            };
        }

        /// <summary>
        /// Binary not operator
        /// </summary>
        public static BitArray operator !(BitArray values)
        {
            BitArray newBits = new BitArray(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                newBits[i] = !values[i];
            }
            return newBits;
        }

        /// <summary>
        /// Binary and operator
        /// </summary>
        public static BitArray operator &(BitArray values1, BitArray values2)
        {
            int minLength = ErrorOverflow(values1, values2, out var newBits);
            for (int i = 0; i < minLength; i++)
            {
                newBits[i] = values1[i] & values2[i];
            }
            return newBits;
        }

        /// <summary>
        /// Binary or operator
        /// </summary>
        public static BitArray operator |(BitArray values1, BitArray values2)
        {
            int minLength = ErrorOverflow(values1, values2, out var newBits);
            for (int i = 0; i < minLength; i++)
            {
                newBits[i] = values1[i] | values2[i];
            }
            return newBits;
        }

        /// <summary>
        /// Binary xor operator.
        /// </summary>
        public static BitArray operator ^(BitArray values1, BitArray values2)
        {
            int minLength = ErrorOverflow(values1, values2, out var newBits);
            for (int i = 0; i < minLength; i++)
            {
                newBits[i] = values1[i] ^ values2[i];
            }
            return newBits;
        }

        /// <summary>
        /// Outputs one only if exactly one input is one
        /// </summary>
        public static BitArray Only1Input1(IEnumerable<BitArray> bitArrays)
        {
            List<BitValue> bitValues = new List<BitValue>();
            int minLength = int.MaxValue;

            //Nothing + Anything = Anything, 0 + 1 = 1, 1 + 1 = Error
            foreach(BitArray array in bitArrays)
            {
                minLength = Math.Min(minLength, array.Length);
                while (array.Length > bitValues.Count) bitValues.Add(BitValue.Nothing);

                for(int i = 0; i < array.Length; i++)
                {
                    if (bitValues[i] == BitValue.Nothing)
                    {
                        bitValues[i] = array[i];
                    }
                    else if(bitValues[i] == BitValue.One && array[i] == BitValue.One)
                    {
                        bitValues[i] = BitValue.Error;
                    }
                    else if(bitValues[i] == BitValue.Zero && array[i] == BitValue.One)
                    {
                        bitValues[i] = BitValue.One;
                    }
                }
            }

            //Replace all errors with zero
            for(int i = 0; i < minLength; i++)
            {
                if (bitValues[i] == BitValue.Error) bitValues[i] = BitValue.Zero;
            }

            //Replace all trailing values with error
            for(int i = minLength; i < bitValues.Count; i++)
            {
                bitValues[i] = BitValue.Error;
            }

            return new BitArray(bitValues.ToArray());
        }

        public void CopyTo(out BitArray data)
        {
            data = new BitArray(Length);
            for(int i = 0; i < Length; i++)
            {
                data[i] = BitValues[i];
            }
        }

        /// <summary>
        /// Finds the amount of bits shared between the two arrays, then errors out the output bits for the rest of the indicies
        /// E.g. values1 = { 0, 0, 1, 1 }
        ///      values2 = { 0, 1, 1, 0, 0, 1 }
        ///      ---------------------
        ///      newBits = { 0, 0, 0, 0, Error, Error }
        /// </summary>
        /// <returns>the length of the array before it begins erroring</returns>
        private static int ErrorOverflow(BitArray values1, BitArray values2, out BitArray newBits)
        {
            int min = Math.Min(values1.Length, values2.Length);
            int max = Math.Max(values1.Length, values2.Length);
            newBits = new BitArray(max);
            for(int i = min; i < max; i++)
            {
                newBits[i] = BitValue.Error;
            }
            return min;
        }

        public static bool operator ==(BitArray values1, BitArray values2)
        {
            return values1.BitValues == values2.BitValues;
        }

        public static bool operator !=(BitArray values1, BitArray values2)
        {
            return values1.BitValues != values2.BitValues;
        }

        public override bool Equals(object obj)
        {
            return obj is BitArray array &&
                   EqualityComparer<BitValue[]>.Default.Equals(BitValues, array.BitValues);
        }

        public override int GetHashCode()
        {
            return 1291433875 + EqualityComparer<BitValue[]>.Default.GetHashCode(BitValues);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{ ");
            foreach (var value in BitValues)
            {
                sb.Append(value);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" }");
            return sb.ToString();
        }
    }
}