using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Gates
{
    public class BitSource : Gate
    {
        public BitSource(Vec2 Position, Dictionary<Vec2, List<CircuitConnector>> connections) 
            : base(Position)
        {
            Inputs = new GatePin[0];
            Outputs = new GatePin[] { new GatePin(this, new Vec2(), BitValue.One) };
        }

        protected override void draw(Graphics gfx)
        {
            Painter.DrawGate(gfx, Position, Color.Green);
        }

        protected override void compute()
        {
            Outputs[0].Value = BitValue.One;
        }
    }
}
