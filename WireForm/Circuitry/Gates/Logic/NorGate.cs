using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Logic
{
    class NorGate : DynamicGate
    {
        [JsonConstructor]
        public NorGate(Vec2 Position, Direction direction) : this(Position, direction, 2, 1) { }

        public NorGate(Vec2 Position, Direction direction, int inputCount, int outputCount)
            : base(Position, direction, new BoxCollider(-2, -1.5f, 3, 3), new Vec2(-2, 0), new Vec2(1, 0), inputCount, outputCount) { }

        protected override void compute()
        {
            Outputs[0].Values = ! FoldInputs((a, c) => a | c);
        }

        protected override void draw(PainterScope painter)
        {
            painter.DrawArcC(Color.Black, 10, new Vec2(-3.5f - .75f, 0), new Vec2(5, 5), 321, 78);

            painter.DrawArcC(Color.Black, 10, new Vec2(-2.3f, 2), new Vec2(8, 7), 270, 60);
            painter.DrawArcC(Color.Black, 10, new Vec2(-2.3f, -2), new Vec2(8, 7), 90, -60);

            painter.DrawEllipseC(Color.Black, 10, new Vec2(1, 0), new Vec2(.4f, .4f));

            base.draw(painter);
        }

        public override CircuitObject Copy()
        {
            return new NorGate(StartPoint, Direction, InputCount, OutputCount);
        }
    }
}
