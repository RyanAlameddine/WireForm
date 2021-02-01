using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Data.Bits;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Logic
{
    [Gate("Logic", "XNOR")]
    class XnorGate : DynamicGate
    {
        [JsonConstructor]
        public XnorGate(Vec2 Position, Direction direction) : this(Position, direction, 2, 1) { }

        public XnorGate(Vec2 Position, Direction direction, int inputCount, int outputCount)
            : base(Position, direction, new BoxCollider(-2, -1.5f, 4, 3), new Vec2(-2, 0), new Vec2(2, 0), inputCount, outputCount) { }

        protected override void Compute()
        {
            //Value is zero if and only if only one input is one
            Outputs[0].Values = ! BitArray.Only1Input1(Inputs.Select((x) => x.Values));
        }

        protected override async Task DrawGate(PainterScope painter)
        {
            await painter.DrawArcC(Color.Black, PenWidth, new Vec2(-3.5f - .75f, 0), new Vec2(5, 5), 321, 78);
            await painter.DrawArcC(Color.Black, PenWidth, new Vec2(-2.5f - .75f, 0), new Vec2(5, 5), 321, 78);
            await painter.DrawArcC(Color.Black, PenWidth, new Vec2(-1.3f       , 2), new Vec2(8, 7), 270, 60);
            await painter.DrawArcC(Color.Black, PenWidth, new Vec2(-1.3f       , -2), new Vec2(8, 7), 90, -60);

            await painter.DrawEllipseC(Color.Black, PenWidth, new Vec2(2, 0), new Vec2(.6f, .6f));

            await base.DrawGate(painter);
        }

        public override BoardObject Copy()
        {
            return new XorGate(StartPoint, Direction, InputCount, OutputCount);
        }
    }
}
