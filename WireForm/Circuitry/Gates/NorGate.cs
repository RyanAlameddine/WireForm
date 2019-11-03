using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    class NorGate : Gate
    {
        public NorGate(Vec2 Position)
            : base(Position, new BoxCollider(-2, -1.5f, 3, 3))
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
            Outputs[0].Value = Inputs[0].Value.Nor(Inputs[1].Value);
        }

        protected override void draw(Graphics gfx)
        {

            gfx._DrawArcC(Color.Black, 10, StartPoint.X - 3.5f - .75f, StartPoint.Y, 5, 5, 321, 78);
            
            gfx._DrawArcC(Color.Black, 10, StartPoint.X - 2.3f, StartPoint.Y + 2, 8f, 7, 270, 60);
            gfx._DrawArcC(Color.Black, 10, StartPoint.X - 2.3f, StartPoint.Y - 2, 8f, 7, 90, -60);

            gfx._DrawEllipseC(Color.Black, 10, StartPoint.X + 1, StartPoint.Y, .4f, .4f);
        }

        public override CircuitObject Copy()
        {
            return new OrGate(StartPoint);
        }
    }
}
