using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.CircuitObjectActions;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    public class BitSource : Gate
    {
        public BitSource(Vec2 Position) 
            : base(Position, new BoxCollider(-.5f, -.5f, 1, 1))
        {
            Inputs = new GatePin[0];
            Outputs = new GatePin[] { new GatePin(this, new Vec2(), BitValue.One) };
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawRectangle(Color.Green, 10, StartPoint.X - .4f, StartPoint.Y - .4f, .8f, .8f);

        }

        private BitValue currentValue = BitValue.One;
        protected override void compute()
        {
            Outputs[0].Value = currentValue;
        }

        [CircuitAction("Toggle", true)]
        public void Toggle()
        {
            currentValue = currentValue.Not();
        }

        public override CircuitObject Copy()
        {
            return new BitSource(StartPoint);
        }
    }
}
