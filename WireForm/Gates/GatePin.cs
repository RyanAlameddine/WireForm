using Newtonsoft.Json;
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
        [JsonIgnore]
        public override Point StartPoint { get; set; }
        Point localPoint;
        public Point LocalPoint
        {
            get
            {
                return localPoint;
            }
            set
            {
                localPoint = value;
                StartPoint = MathHelper.Plus(value, Parent.Position);
            }
        }
        [JsonIgnore]
        public BitValue Value { get; set; }
        public Gate Parent { get; set; }
        public GatePin(Gate Parent, Point LocalStart, BitValue value)
        {
            this.Parent = Parent;

            this.LocalPoint = LocalStart;
            this.Value = Value;
        }
    }
}
