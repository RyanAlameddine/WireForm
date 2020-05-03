using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Logic
{
    [Gate("Logic", "XOR")]
    class XorGate : DynamicGate
    {
        [JsonConstructor]
        public XorGate(Vec2 Position, Direction direction) : this(Position, direction, 2, 1) { }

        public XorGate(Vec2 Position, Direction direction, int inputCount, int outputCount)
            : base(Position, direction, new BoxCollider(-2, -1.5f, 4, 3), new Vec2(-2, 0), new Vec2(2, 0), inputCount, outputCount) { }

        protected override void Compute()
        {
            //Value is one if and only if only one input is one
            Outputs[0].Values = BitArray.Only1Input1(Inputs.Select((x) => x.Values));
        }

        protected override void Draw(PainterScope painter)
        {
            painter.DrawArcC(Color.Black, 10, new Vec2(-3.5f - .75f, 0), new Vec2(5, 5), 321, 78);
            painter.DrawArcC(Color.Black, 10, new Vec2(-2.5f - .75f, 0), new Vec2(5, 5), 321, 78);
            painter.DrawArcC(Color.Black, 10, new Vec2(-1.3f       , 2), new Vec2(8, 7), 270, 60);
            painter.DrawArcC(Color.Black, 10, new Vec2(-1.3f       , -2), new Vec2(8, 7), 90, -60);

            base.Draw(painter);
        }

        public override CircuitObject Copy()
        {
            return new XorGate(StartPoint, Direction, InputCount, OutputCount);
        }
    }
}
