using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm.Circuitry.Gates
{
    class AndGate : Gate
    {
        public AndGate(Vec2 Position)
            : base(Position)
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-2, -1), BitValue.Nothing),
                new GatePin(this, new Vec2(-2, 1), BitValue.Nothing)
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(), BitValue.Error)
            };
        }

        protected override void compute()
        {
            Outputs[0].Value = Inputs[0].Value.And(Inputs[1].Value);
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawArc(Color.Black, 10, Position.X - 2.5f, Position.Y - 1.25f, 2.5f, 2.5f, 270, 180);
            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, 1.25f), Position + new Vec2(-2, -1.25f));
            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, 1.25f), Position + new Vec2(-1, 1.25f));
            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, -1.25f), Position + new Vec2(-1, -1.25f));
        }
    }
}
