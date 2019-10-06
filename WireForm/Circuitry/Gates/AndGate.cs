using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    class AndGate : Gate
    {
        public AndGate(Vec2 Position)
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
            Outputs[0].Value = Inputs[0].Value.And(Inputs[1].Value);
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, 1.5f), Position + new Vec2(-2, -1.5f));

            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, 1.5f), Position + new Vec2(-.5f + .1f, 1.5f));
            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, -1.5f), Position + new Vec2(-.5f + .1f, -1.5f));
            gfx._DrawArc(Color.Black, 10, Position.X - 2f, Position.Y - 1.5f, 3f, 3f, 270, 180);
        }
    }
}
