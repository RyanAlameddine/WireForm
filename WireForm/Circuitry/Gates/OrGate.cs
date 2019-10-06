using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    class OrGate : Gate
    {
        public OrGate(Vec2 Position)
            : base(Position, new BoxCollider(-2, -1, 3, 2))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-2, -1), BitValue.Nothing),
                new GatePin(this, new Vec2(-2, 1), BitValue.Nothing)
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0), BitValue.Error)
            };
        }

        protected override void compute()
        {
            Outputs[0].Value = Inputs[0].Value.Or(Inputs[1].Value);
        }

        protected override void draw(Graphics gfx)
        {

            gfx._DrawArcC(Color.Black, 10, Position.X - 3.5f - .75f, Position.Y, 5, 5, 321, 78);
            //gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, 1.25f), Position + new Vec2(-2, -1.25f));

            //gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2 - .15f, 1.25f), Position + new Vec2(-1, 1.25f));
            //gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2 - .15f, -1.25f), Position + new Vec2(-1, -1.25f));
            //gfx._DrawArc(Color.Black, 10, Position.X - 2.5f, Position.Y - 1.25f, 2.5f, 2.5f, 270, 180);

            gfx._DrawArcC(Color.Black, 10, Position.X - 2.3f, Position.Y + 2, 8f, 7, 270, 60);
            gfx._DrawArcC(Color.Black, 10, Position.X - 2.3f, Position.Y - 2, 8f, 7, 90, -60);
            //gfx._DrawArcC(Color.Black, 10, Position.X - 3.5f - .25f, Position.Y, 6, 6, float.Parse(Form1.debug1Value), float.Parse(Form1.debug2Value));
        }
    }
}
