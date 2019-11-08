using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Data;
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
            Outputs = new GatePin[] { 
                new GatePin(this, new Vec2()) 
            };
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawRectangle(Color.Green, 10, StartPoint.X - .4f, StartPoint.Y - .4f, .8f, .8f);
        }

        public BitValue currentValue = BitValue.One;
        protected override void compute()
        {
            Outputs[0].Values.Set(0, currentValue);
        }

        [CircuitAction("Toggle", System.Windows.Forms.Keys.T)]
        public void Toggle()
        {
            currentValue = !currentValue;
        }

        public override CircuitObject Copy()
        {
            var gate = new BitSource(StartPoint);
            gate.currentValue = currentValue;
            return gate;
        }
    }
}
