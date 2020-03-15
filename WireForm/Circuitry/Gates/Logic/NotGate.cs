using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates.Logic
{
    class NotGate : Gate
    {
        public NotGate(Vec2 Position)
            : base(Position, new BoxCollider(-1, -.5f, 2, 1))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-1, 0))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0))
            };
        }

        protected override void compute()
        {
            Outputs[0].Values = !Inputs[0].Values;
        }

        protected override void draw(Painter painter)
        {
            painter.DrawLine(Color.Black, 10, new Vec2(-1, .75f), new Vec2(-1, -.75f));
            painter.DrawLine(Color.Black, 10, new Vec2(-1, .75f), new Vec2(1, 0));
            painter.DrawLine(Color.Black, 10, new Vec2(-1, -.75f), new Vec2(1, 0));
        }

        public override CircuitObject Copy()
        {
            return new NotGate(StartPoint);
        }
    }
}
