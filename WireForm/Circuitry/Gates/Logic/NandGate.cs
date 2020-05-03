using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Logic
{
    [Gate("Logic", "NAND")]
    class NandGate : DynamicGate
    {
        [JsonConstructor]
        public NandGate(Vec2 Position, Direction direction) : this(Position, direction, 2, 1) { }

        public NandGate(Vec2 Position, Direction direction, int inputCount, int outputCount)
            : base(Position, direction, new BoxCollider(-2, -1.5f, 3, 3), new Vec2(-2, 0), new Vec2(1, 0), inputCount, outputCount) { }

        protected override void Compute()
        {
            Outputs[0].Values = ! FoldInputs((a, c) => a & c);
        }

        protected override void Draw(PainterScope painter)
        {
            painter.DrawLine(Color.Black, 10, new Vec2(-2, 1.5f), new Vec2(-2, -1.5f));
            
            painter.DrawLine(Color.Black, 10, new Vec2(-2, 1.5f), new Vec2(-.5f + .1f, 1.5f));
            painter.DrawLine(Color.Black, 10, new Vec2(-2, -1.5f), new Vec2(-.5f + .1f, -1.5f));
            painter.DrawArc(Color.Black, 10, new Vec2(-2f, -1.5f), new Vec2(3f, 3f), 270, 180);
            
            painter.DrawEllipseC(Color.Black, 10, new Vec2(1, 0), new Vec2(.6f, .6f));

            base.Draw(painter);
        }

        public override CircuitObject Copy()
        {
            return new NandGate(StartPoint, Direction, InputCount, OutputCount);
        }
    }
}
