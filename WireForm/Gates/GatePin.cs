using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Gates
{
    public class GatePin : CircuitConnector
    {
        public override Point StartPoint { get; set; }
        public BitValue Value { get; set; }
        public Gate Parent { get; set; }
        public GatePin(Gate Parent, Point Start, BitValue value)
        {
            this.StartPoint = Start;
            this.Parent = Parent;
            this.Value = Value;
        }
    }
}
