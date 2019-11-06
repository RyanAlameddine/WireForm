using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    class XorGate : Gate
    {
        public XorGate(Vec2 Position)
            : base(Position, new BoxCollider(-2, -1.5f, 4, 3))
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
            Outputs[0].Values = Inputs[0].Values.Xor(Inputs[1].Values);
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawArcC(Color.Black, 10, StartPoint.X - 3.5f - .75f, StartPoint.Y, 5, 5, 321, 78);

            gfx._DrawArcC(Color.Black, 10, StartPoint.X - 2.5f - .75f, StartPoint.Y, 5, 5, 321, 78);

            gfx._DrawArcC(Color.Black, 10, StartPoint.X - 1.3f, StartPoint.Y + 2, 8f, 7, 270, 60);
            gfx._DrawArcC(Color.Black, 10, StartPoint.X - 1.3f, StartPoint.Y - 2, 8f, 7, 90, -60);
        }

        public override CircuitObject Copy()
        {
            return new XorGate(StartPoint);
        }
    }
}
