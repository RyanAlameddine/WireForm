using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates
{
    public class BitSource : Gate
    {
        public BitSource(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-.5f, -.5f, 1, 1))
        {
            Inputs = new GatePin[0];
            Outputs = new GatePin[] {
                new GatePin(this, new Vec2())
            };
        }

        protected override void draw(PainterScope painter)
        {
            painter.DrawRectangle(Color.Green, 10, new Vec2(-.4f, -.4f), new Vec2(.8f, .8f));
        }

        protected override void compute()
        {
            for (int i = 0; i < BitDepth; i++)
            {
                Outputs[0].Values.Set(i, currentValue);
            }
        }

        public override CircuitObject Copy()
        {
            var gate = new BitSource(StartPoint, Direction);
            gate.currentValue = currentValue;
            gate.BitDepth = BitDepth;
            return gate;
        }


        [JsonIgnore]
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
            get
            {
                return Outputs[0].Values.Length;
            }
            set
            {
                Outputs[0].Values = new BitArray(value);
            }
        }

        [HideCircuitAttributes]
        public override Direction Direction { get => base.Direction; set => base.Direction = value; }

        public BitValue currentValue = BitValue.One;

        [CircuitAction("Toggle", 't')]
        public void Toggle()
        {
            currentValue = !currentValue;
        }
    }
}
