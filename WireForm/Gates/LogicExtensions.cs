using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Gates
{
    public static class LogicExtensions
    {
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
    }
}
