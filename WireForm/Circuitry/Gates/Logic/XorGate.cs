using System;
using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Logic
{
    class XorGate : Gate
    {
        public XorGate(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-2, -1.5f, 4, 3))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-2, -1)),
                new GatePin(this, new Vec2(-2, 1))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(2, 0))
            };
        }

        protected override void compute()
        {
            Outputs[0].Values = Inputs[0].Values ^ Inputs[1].Values;
        }

        protected override void draw(PainterScope painter)
        {
            painter.DrawArcC(Color.Black, 10, new Vec2(-3.5f - .75f, 0), new Vec2(5, 5), 321, 78);
            painter.DrawArcC(Color.Black, 10, new Vec2(-2.5f - .75f, 0), new Vec2(5, 5), 321, 78);
            painter.DrawArcC(Color.Black, 10, new Vec2(-1.3f       , 2), new Vec2(8, 7), 270, 60);
            painter.DrawArcC(Color.Black, 10, new Vec2(-1.3f       , -2), new Vec2(8, 7), 90, -60);
        }

        public override CircuitObject Copy()
        {
            return new XorGate(StartPoint, Direction);
        }
    }
}
