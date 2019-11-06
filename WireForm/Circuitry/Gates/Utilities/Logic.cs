using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Circuitry.Gates.Utilities
{
    public static class Logic
    {
       public static BitValue[] Not(BitValue[] values)
        {
            BitValue[] Output = new BitValue[values.Length];
            for(int i = 0; i < values.Length; i++)
            {
                Output[i] = !values[i];
            }
        }
    }
}
