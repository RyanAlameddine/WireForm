using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Splitter(Vec2 Position, int splitCount, int inputDepth, int direction)
            : base(Position, new BoxCollider(0, 0, 2, 1))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(0, 0))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(2, 1))
            };
            Direction = direction;
            SplitCount = splitCount;
            SplitDepth = inputDepth;
        }

        protected override void compute()
        {
            if (direction == 0)
            {
                for (int i = 0; i < Outputs.Length; i++)
                {
                    for (int j = 0; j < splitDepth; j++)
                    {
                        int k = splitDepth * i + j;
                        if (Inputs[0].Values.Length > k)
                        {
                            Outputs[i].Values.Set(j, Inputs[0].Values[k]);
                        }
                        else
                        {
                            Outputs[i].Values.Set(j, BitValue.Error);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Inputs.Length; i++)
                {
                    for (int j = 0; j < Inputs[i].Values.Length; j++)
                    {
                        int k = splitDepth * i + j;
                        if (Outputs[0].Values.Length > k)
                        {
                            Outputs[0].Values.Set(k, Inputs[i].Values[j]);
                        }
                        else
                        {
                            Debug.WriteLine("Splitter overflow");
                        }
                    }
                }
            }
        }

        protected override void draw(Painter painter)
        {
            painter.DrawLine(Color.DarkGray, 10, Vec2.Zero, new Vec2(1, 1));

            painter.DrawLine(Color.DarkGray, 10, new Vec2(1, 1), new Vec2(1, 1 + splitCount - 1));

            painter.DrawLine(Color.DarkGray, 10, new Vec2(1 - 1 / 20f, 1), new Vec2(2, 1));
            painter.DrawStringC(getRange(0), Color.Black, new Vec2(1.4f, 1), 4);
            for (int i = 1; i < splitCount; i++)
            {
                painter.DrawLine(Color.DarkGray, 10, new Vec2(1, 1 + i), new Vec2(2, 1 + i));
                painter.DrawStringC(getRange(i), Color.Black, new Vec2(1.4f, 1 + i), 4);
            }

            if (direction == 0)
            {
                painter.DrawStringC("I/O", Color.Black, new Vec2(1f, .5f), 2);
            }
            else
            {
                painter.DrawStringC("O/I", Color.Black, new Vec2(1f, .5f), 2);
            }
        }

        private string getRange(int i)
        {
            if(splitDepth == 1)
            {
                return i.ToString();
            }
            else
            {
                if(i == 0)
                {
                    return "0-" + (splitDepth - 1);
                }
                else
                {
                    int startIndex = i * splitDepth;
                    int endIndex = startIndex + splitDepth - 1;
                    return startIndex + "-" + endIndex;
                }
            }
        }

        public override CircuitObject Copy()
        {
            Splitter splitter = new Splitter(StartPoint, splitCount, splitDepth, direction);

            return splitter;
        }

        int splitCount = 1;
        [CircuitProperty(1, 32, true)]
        public int SplitCount
        {
            get
            {
                return splitCount;
            }
            set
            {
                splitCount = value;
                ResetIO();
            }
        }

        int splitDepth = 1;
        [CircuitProperty(1, 32, true)]
        public int SplitDepth
        {
            get
            {
                return splitDepth;
            }
            set
            {
                splitDepth = value;
                ResetIO();
            }
        }

        /// <summary>
        /// Resets the Inputs and Outputs to match an updated splitCount and/or bitDepth
        /// </summary>
        void ResetIO()
        {
            HitBox = new BoxCollider(0, 0, 2, splitCount);
            if (direction == 0)
            {
                //Inputs length is 1
                Outputs = new GatePin[splitCount];
                for (int i = 0; i < splitCount; i++)
                {
                    Outputs[i] = new GatePin(this, new Vec2(2, i + 1));
                    Outputs[i].Values = new BitArray(splitDepth);
                }
            }
            else
            {
                //Outputs length is 1
                Outputs[0].Values = new BitArray(splitDepth * splitCount);
                Inputs = new GatePin[splitCount];
                for (int i = 0; i < splitCount; i++)
                {
                    Inputs[i] = new GatePin(this, new Vec2(2, i + 1));
                    Inputs[i].Values = new BitArray(splitDepth);
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
            if (direction == 0)
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
