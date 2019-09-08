using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    public class WireData
    {
        public BitValue[] bitValues;

        public WireData(int valueCount)
        {
            bitValues = new BitValue[valueCount];
        }
    }

    public enum BitValue
    {
        Nothing,
        Error,
        Zero,
        One
    }
}
