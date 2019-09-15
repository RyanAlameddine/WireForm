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
            : base(Position, new GatePin[] { new GatePin(null, Point.Empty, BitValue.One) }, 0, connections)
        {
            
        }

        protected override void draw(Graphics gfx)
        {
            Painter.DrawGate(gfx, Position, Color.Green);
        }

        protected override void compute(BitValue[] inputs)
        {
            Outputs[0].Value = BitValue.One;
        }
    }
}
