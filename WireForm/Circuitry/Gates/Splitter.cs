using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates.Logic
{
    class Splitter : Gate
    {
        [JsonConstructor]
        public Splitter(Vec2 Position)
            : base(Position, new BoxCollider(0, 0, 2, 1))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(0, 0))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(2, 1))
            };
        }

        public Splitter(Vec2 Position, int bitDepth, int direction)
            : base(Position, new BoxCollider(0, 0, 2, 1))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(0, 0))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(2, 1))
            };
            BitDepth = bitDepth;
            Direction = direction;
        }

        protected override void compute()
        {
            if (direction == 0)
            {
                for (int i = 0; i < Inputs[0].Values.Length; i++)
                {
                    Outputs[i].Values.Set(0, Inputs[0].Values[i]);
                }
                for (int i = Inputs[0].Values.Length; i < bitDepth; i++)
                {
                    Outputs[i].Values.Set(0, BitValue.Error);
                }
            }
            else
            {
                Outputs[0].Values = new BitArray(Inputs.Length);
                for(int i = 0; i < Inputs.Length; i++)
                {
                    Outputs[0].Values.Set(i, Inputs[i].Values[0]);
                }
            }
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawLine(Color.DarkGray, 10, StartPoint, StartPoint + new Vec2(1, 1));

            gfx._DrawLine(Color.DarkGray, 10, StartPoint + new Vec2(1, 1), StartPoint + new Vec2(1, 1 + bitDepth - 1));

            gfx._DrawLine(Color.DarkGray, 10, StartPoint + new Vec2(1 - 1 / 20f, 1), StartPoint + new Vec2(2, 1));
            for (int i = 1; i < bitDepth; i++)
            {
                gfx._DrawLine(Color.DarkGray, 10, StartPoint + new Vec2(1, 1 + i), StartPoint + new Vec2(2, 1 + i));
            }

            if (direction == 0)
            {
                gfx._DrawStringC("I/O", Color.Black, StartPoint.X + 1f, StartPoint.Y + .5f, 2);
            }
            else
            {
                gfx._DrawStringC("O/I", Color.Black, StartPoint.X + 1f, StartPoint.Y + .5f, 2);
            }
        }

        public override CircuitObject Copy()
        {
            Splitter splitter = new Splitter(StartPoint, bitDepth, direction);

            return splitter;
            //TODO FIX THIS PROBABLY DOESNT WORK
        }

        int bitDepth = 1;
        [CircuitProperty(1, 32, true)]
        public int BitDepth
        {
            get
            {
                return bitDepth;
            }
            set
            {
                bitDepth = value;
                HitBox = new BoxCollider(0, 0, 2, value);
                if (direction == 0)
                {
                    Outputs = new GatePin[value];
                    for (int i = 0; i < value; i++)
                    {
                        Outputs[i] = new GatePin(this, new Vec2(2, i + 1));
                    }
                }
                else
                {
                    Inputs = new GatePin[value];
                    for (int i = 0; i < value; i++)
                    {
                        Inputs[i] = new GatePin(this, new Vec2(2, i + 1));
                    }
                }
            }
        }

        private int direction = 0;
        [CircuitProperty(0, 1, true, new[] { "Expand", "Contract" })]
        public int Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    var temp = Outputs;
                    Outputs = Inputs;
                    Inputs = temp;
                }
            }
        }

        [CircuitAction("Toggle", System.Windows.Forms.Keys.T)]
        public void Toggle()
        {
            if(direction == 0)
            {
                Direction = 1;
            }
            else
            {
                Direction = 0;
            }
        }
    }
}
