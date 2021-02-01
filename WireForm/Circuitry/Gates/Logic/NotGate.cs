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
    [Gate("Logic", "NOT")]
    class NotGate : Gate
    {
        public NotGate(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-1, -.5f, 2, 1))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-1, 0))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0))
            };
        }

        protected override void Compute()
        {
            Outputs[0].Values = !Inputs[0].Values;
        }

        protected override async Task DrawGate(PainterScope painter)
        {
            await painter.DrawLine(Color.Black, PenWidth, new Vec2(-1, .75f), new Vec2(-1, -.75f));
            await painter.DrawLine(Color.Black, PenWidth, new Vec2(-1, .75f), new Vec2(1, 0));
            await painter.DrawLine(Color.Black, PenWidth, new Vec2(-1, -.75f), new Vec2(1, 0));
        }

        public override BoardObject Copy()
        {
            return new NotGate(StartPoint, Direction);
        }
    }
}
