using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates.Logic
{
    class Splitter : Gate
    {
        [JsonConstructor]
        public Splitter(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-1, -1, 2, 1))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-1, -1))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0))
            };
        }

        public Splitter(Vec2 Position, Direction direction, int splitCount, int inputDepth, int splitDirection)
            : this(Position, direction)
        {
            SplitDirection = splitDirection;
            SplitCount = splitCount;
            SplitDepth = inputDepth;
        }

        protected override void compute()
        {
            if (splitDirection == 0)
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
            painter.DrawLine(Color.DarkGray, 10, new Vec2(-1, -1), Vec2.Zero);

            painter.DrawLine(Color.DarkGray, 10, Vec2.Zero, new Vec2(0, splitCount - 1));

            painter.DrawLine(Color.DarkGray, 10, new Vec2(0 - 1 / 20f, 0), new Vec2(1, 0));
            painter.DrawStringC(getRange(0), Color.Black, new Vec2(.4f, 0), 4);
            for (int i = 1; i < splitCount; i++)
            {
                painter.DrawLine(Color.DarkGray, 10, new Vec2(0, i), new Vec2(1, i));
                painter.DrawStringC(getRange(i), Color.Black, new Vec2(.4f, i), 4);
            }

            if (splitDirection == 0)
            {
                painter.DrawStringC("I", Color.Black, new Vec2(0f, -.5f), 2);
                painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 2);
                painter.DrawStringC("O", Color.Black, new Vec2(.8f, -.5f), 2);
            }
            else
            {
                painter.DrawStringC("O", Color.Black, new Vec2(0f, -.5f), 2);
                painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 2);
                painter.DrawStringC("I", Color.Black, new Vec2(.8f, -.5f), 2);
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
            Splitter splitter = new Splitter(StartPoint, Direction, splitCount, splitDepth, splitDirection);

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
            LocalHitbox = new BoxCollider(-1, -1, 2, splitCount);
            if (splitDirection == 0)
            {
                //Inputs length is 1
                Outputs = new GatePin[splitCount];
                for (int i = 0; i < splitCount; i++)
                {
                    Outputs[i] = new GatePin(this, new Vec2(1, i));
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
                    Inputs[i] = new GatePin(this, new Vec2(1, i));
                    Inputs[i].Values = new BitArray(splitDepth);
                }
            }
        }

        private int splitDirection = 0;
        [CircuitProperty(0, 1, true, new[] { "Expand", "Contract" })]
        public int SplitDirection
        {
            get
            {
                return splitDirection;
            }
            set
            {
                if (splitDirection != value)
                {
                    splitDirection = value;
                    var temp = Outputs;
                    Outputs = Inputs;
                    Inputs = temp;
                }
            }
        }

        [CircuitAction("Toggle", System.Windows.Forms.Keys.T)]
        public void Toggle()
        {
            if (splitDirection == 0)
            {
                SplitDirection = 1;
            }
            else
            {
                SplitDirection = 0;
            }
        }
    }
}
