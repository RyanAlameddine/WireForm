using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates.Logic
{
    class AndGate : Gate
    {
        public AndGate(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-2, -1.5f, 3, 3))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-2, -1)),
                new GatePin(this, new Vec2(-2, 1))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0))
            };
        }

        protected override void compute()
        {
            Outputs[0].Values = Inputs[0].Values & Inputs[1].Values;
        }

        protected override void draw(Painter painter)
        {
            painter.DrawLine(Color.Black, 10, new Vec2(-2, 1.5f), new Vec2(-2, -1.5f));
            painter.DrawLine(Color.Black, 10, new Vec2(-2, 1.5f), new Vec2(-.5f + .1f, 1.5f));
            painter.DrawLine(Color.Black, 10, new Vec2(-2, -1.5f), new Vec2(-.5f + .1f, -1.5f));
            painter.DrawArc(Color.Black, 10, new Vec2(-2f, -1.5f), new Vec2(3f, 3f), 270, 180);
        }

        public override CircuitObject Copy()
        {
            return new NandGate(StartPoint, Direction);
        }
    }
}
