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
        public NotGate(Vec2 Position, Dictionary<Vec2, List<CircuitConnector>> connections)
            : base(Position)
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-1, 0), BitValue.Nothing)
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(), BitValue.Error)
            };
        }

        protected override void compute()
        {
            Outputs[0].Value = Inputs[0].Value.Not();
        }

        protected override void draw(Graphics gfx)
        {
            gfx.DrawLine(new Pen(Color.Black, 5), (Point) MathHelper.Plus(Position, new Vec2(-2, 1)).Times(50), (Point) MathHelper.Plus(Position, new Vec2()).Times(50));
            gfx.DrawLine(new Pen(Color.Black, 5), (Point) MathHelper.Plus(Position, new Vec2(-2, -1)).Times(50), (Point) MathHelper.Plus(Position, new Vec2()).Times(50));
            gfx.DrawLine(new Pen(Color.Black, 5), (Point) MathHelper.Plus(Position, new Vec2(-2, 1)).Times(50), (Point) MathHelper.Plus(Position, new Vec2(-2, -1)).Times(50));

            Painter.DrawGate(gfx, Position, Color.Black);
        }
    }
}
