using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Circuitry.Gates.Utilities
{
    public static class LogicExtensions
    {
        private static bool isUndefined(this BitValue value)
        {
            if(value == BitValue.Error || value == BitValue.Nothing)
            {
                return true;
            }
            return false;
        }

        public static BitValue Not(this BitValue value)
        {
            switch (value)
            {
                case BitValue.One:
                    return BitValue.Zero;
                case BitValue.Zero:
                    return BitValue.One;
                case BitValue.Nothing:
                    return BitValue.Error;
                case BitValue.Error:
                    return BitValue.Error;
            }
            throw new Exception("shouldnt happen");
        }

        public static BitValue And(this BitValue value1, BitValue value2)
        {
            if(value1.isUndefined() || value2.isUndefined())
            {
                return BitValue.Error;
            }
            if(value1 == value2 && value1 == BitValue.One)
            {
                return BitValue.One;
            }
            return BitValue.Zero;
        }
    }
}
