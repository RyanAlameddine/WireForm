using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WireForm.Circuitry.Data
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

        public Color BitColor()
        {

            if (Length == 1)
            {
                switch (BitValues[0].Selected)
                {
                    case BitValue.Error:
                        return Color.DarkRed;
                    case BitValue.Nothing:
                        return Color.DimGray;
                    case BitValue.One:
                        return Color.Blue;
                    case BitValue.Zero:
                        return Color.DarkBlue;
                }
                throw new NullReferenceException();
            }
            else
            {
                return Color.Black;
            }
        }

        public static BitArray operator !(BitArray values)
        {
            BitArray newBits = new BitArray(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                newBits[i] = !values[i];
            }
            return newBits;
        }

        public static BitArray operator &(BitArray values1, BitArray values2)
        {
            BitArray newBits = new BitArray(values1.Length);
            for (int i = 0; i < newBits.Length; i++)
            {
                newBits[i] = values1[i] & values2[i];
            }
            return newBits;
        }

        public static BitArray operator |(BitArray values1, BitArray values2)
        {
            BitArray newBits = new BitArray(values1.Length);
            for (int i = 0; i < newBits.Length; i++)
            {
                newBits[i] = values1[i] | values2[i];
            }
            return newBits;
        }

        public static BitArray operator ^(BitArray values1, BitArray values2)
        {
            BitArray newBits = new BitArray(values1.Length);
            for (int i = 0; i < newBits.Length; i++)
            {
                newBits[i] = values1[i] ^ values2[i];
            }
            return newBits;
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