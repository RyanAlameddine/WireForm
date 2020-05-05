using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;

using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using Wireform.Circuitry.Utils;
using System.Linq;

namespace Wireform.Circuitry.Gates
{
    [Gate]
    public class BitSource : Gate
    {
        public BitSource(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-.5f, -.5f, 1, 1))
        {
            Inputs = Array.Empty<GatePin>();
            Outputs = new GatePin[] {
                new GatePin(this, new Vec2())
            };
        }

        protected override void Draw(PainterScope painter)
        {
            painter.DrawRectangle(Color.Green, 10, new Vec2(-.4f, -.4f), new Vec2(.8f, .8f));
        }

        protected override void Compute()
        {
            Outputs[0].Values = Outputs[0].Values.Select((_) => currentValue);
        }

        public override CircuitObject Copy()
        {
            var gate = new BitSource(StartPoint, Direction)
            {
                currentValue = currentValue,
                BitDepth = BitDepth
            };
            return gate;
        }


        [JsonIgnore]
        [CircuitPropertyAction("Toggle", 't', true)]
        [CircuitProperty(2, 3, false, new[] { "Zero", "One" })]
        public int Value
        {
            get
            {
                return currentValue.Selected;
            }
            set
            {
                currentValue = value;
            }
        }

        [CircuitProperty(1, 32, false)]
        public int BitDepth
        {
            get => Outputs[0].Values.Count;
            set
            {
                Outputs[0].Values = new BitArray(value);
            }
        }

        [HideCircuitAttributes]
        public override Direction Direction { get => base.Direction; set => base.Direction = value; }

        public BitValue currentValue = BitValue.One;
    }
}
