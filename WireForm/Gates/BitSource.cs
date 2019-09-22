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
        public BitSource(Point Position, Dictionary<Point, List<CircuitConnector>> connections) 
            : base(Position, 0, connections)
        {
            Inputs = new GatePin[0];
            Outputs = new GatePin[] { new GatePin(this, Point.Empty, BitValue.One) };
            connections.AddConnection(Outputs[0]);
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
