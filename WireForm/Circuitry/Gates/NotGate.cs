using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    class NotGate : Gate
    {
        public NotGate(Vec2 Position)
            : base(Position, new BoxCollider(-2, 0, 3, 0))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-2, 0), BitValue.Nothing)
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
            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, .75f), Position + new Vec2(-2, -.75f));

            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, .75f), Position + new Vec2());
            gfx._DrawLine(Color.Black, 10, Position + new Vec2(-2, -.75f), Position + new Vec2());
        }
    }
}
