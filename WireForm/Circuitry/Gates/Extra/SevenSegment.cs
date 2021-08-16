using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Extra
{
    [Gate("Extra", "7-Segment Display")]
    public class SevenSegment : Gate
    {
        [JsonConstructor]
        public SevenSegment(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-1, -1, 2, 1)) 
        {
            Inputs = new GatePin[] 
            {
                new GatePin(this, new Vec2())
            };
            Outputs = Array.Empty<GatePin>();
        }

        protected override void Compute()
        {
            throw new NotImplementedException();
        }

        protected override Task DrawGate(PainterScope painter)
        {
            throw new NotImplementedException();
        }

        public override BoardObject Copy()
        {
            throw new NotImplementedException();
        }
    }
}
