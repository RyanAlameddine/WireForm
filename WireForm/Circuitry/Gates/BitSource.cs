using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm.Circuitry.Gates
{
    public class BitSource : Gate
    {
        public BitSource(Vec2 Position) 
            : base(Position)
        {
            Inputs = new GatePin[0];
            Outputs = new GatePin[] { new GatePin(this, new Vec2(), BitValue.One) };
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawRectangle(Color.Green, 10, Position.X - .4f, Position.Y - .4f, .8f, .8f);

        }

        protected override void compute()
        {
            Outputs[0].Value = BitValue.One;
        }
    }
}
