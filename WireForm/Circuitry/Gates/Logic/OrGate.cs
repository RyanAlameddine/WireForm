using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Logic
{
    [Gate("Logic", "OR")]
    class OrGate : DynamicGate
    {
        [JsonConstructor]
        public OrGate(Vec2 Position, Direction direction) : this(Position, direction, 2, 1) { }

        public OrGate(Vec2 Position, Direction direction, int inputCount, int outputCount)
            : base(Position, direction, new BoxCollider(-2, -1.5f, 3, 3), new Vec2(-2, 0), new Vec2(1, 0), inputCount, outputCount) { }

        protected override void Compute()
        {
            Outputs[0].Values = FoldInputs((a, c) => a | c);
        }

        protected override async Task DrawGate(PainterScope painter)
        {
            await painter.DrawArcC(Color.Black, PenWidth, new Vec2(-3.5f - .75f, 0), new Vec2(5, 5), 321,  78);
            await painter.DrawArcC(Color.Black, PenWidth, new Vec2(-2.3f       , 2), new Vec2(8, 7), 270,  60);
            await painter.DrawArcC(Color.Black, PenWidth, new Vec2(-2.3f       , -2), new Vec2(8, 7), 90 , -60);

            await base.DrawGate(painter);
        }

        public override BoardObject Copy()
        {
            return new OrGate(StartPoint, Direction, InputCount, OutputCount);
        }
    }
}
