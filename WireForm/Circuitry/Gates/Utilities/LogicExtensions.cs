using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Circuitry.Gates.Utilities
{
    public static class LogicExtensions
    {
        private static bool isNothing(this BitValue value)
        {
            return value == BitValue.Nothing;
        }

        private static bool isError(this BitValue value)
        {
            return value == BitValue.Error;
        }

        private static bool isOne(this BitValue value)
        {
            return value == BitValue.One;
        }

        private static bool isZero(this BitValue value)
        {
            return value == BitValue.Zero;
        }
        private static bool isUndefined(this BitValue value)
        {
            if (value == BitValue.Error || value == BitValue.Nothing)
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
                    return BitValue.Nothing;
                case BitValue.Error:
                    return BitValue.Error;
            }
            throw new Exception("how tf did this happen");
        }

        public static BitValue And(this BitValue value1, BitValue value2)
        {
            if (value1.isNothing() && value2.isNothing()) return BitValue.Nothing;
            if (value1.isUndefined() || value2.isUndefined()) return BitValue.Error;

            if (value1 == value2 && value1 == BitValue.One)
            {
                return BitValue.One;
            }
            return BitValue.Zero;
        }

        public static BitValue Or(this BitValue value1, BitValue value2)
        {
            if (value1.isNothing() && value2.isNothing()) return BitValue.Nothing;
            if (value1.isUndefined() || value2.isUndefined()) return BitValue.Error;

            if (value2 == BitValue.One || value1 == BitValue.One)
            {
                return BitValue.One;
            }
            return BitValue.Zero;
        }

        public static BitValue Nor(this BitValue value1, BitValue value2)
        {
            if (value1.isNothing() && value2.isNothing()) return BitValue.Nothing;
            if (value1.isUndefined() || value2.isUndefined()) return BitValue.Error;

            if (value2 == BitValue.One || value1 == BitValue.One)
            {
                return BitValue.Zero;
            }
            return BitValue.One;
        }

        public static BitValue Xor(this BitValue value1, BitValue value2)
        {
            if (value1.isNothing() && value2.isNothing()) return BitValue.Nothing;
            if (value1.isUndefined() || value2.isUndefined()) return BitValue.Error;

            if (value1 != value2 && (value2 == BitValue.One || value1 == BitValue.One))
            {
                return BitValue.One;
            }
            return BitValue.Zero;
        }
    }
}
