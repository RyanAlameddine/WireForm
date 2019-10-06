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
            : base(Position, new BoxCollider(-3, -1.5f, 4, 3))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-3, -1), BitValue.Nothing),
                new GatePin(this, new Vec2(-3, 1), BitValue.Nothing)
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0), BitValue.Error)
            };
        }

        protected override void compute()
        {
            Outputs[0].Value = Inputs[0].Value.Xor(Inputs[1].Value);
        }

        protected override void draw(Graphics gfx)
        {

            gfx._DrawArcC(Color.Black, 10, Position.X - 4.5f - .75f, Position.Y, 5, 5, 321, 78);

            gfx._DrawArcC(Color.Black, 10, Position.X - 3.5f - .75f, Position.Y, 5, 5, 321, 78);

            gfx._DrawArcC(Color.Black, 10, Position.X - 2.3f, Position.Y + 2, 8f, 7, 270, 60);
            gfx._DrawArcC(Color.Black, 10, Position.X - 2.3f, Position.Y - 2, 8f, 7, 90, -60);
        }
    }
}
