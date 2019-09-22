using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Gates
{
    class NotGate : Gate
    {
        public NotGate(Point Position, Dictionary<Point, List<CircuitConnector>> connections)
            : base(Position, 1, connections)
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Point(-1, 0), BitValue.Nothing)
            };
            connections.AddConnection(Inputs[0]);

            Outputs = new GatePin[] {
                new GatePin(this, Point.Empty, BitValue.Error)
            };
            connections.AddConnection(Outputs[0]);
        }

        protected override void compute()
        {
            Outputs[0].Value = Inputs[0].Value.Not();
        }

        protected override void draw(Graphics gfx)
        {
            Painter.DrawGate(gfx, Position, Color.Black);
        }
    }
}
