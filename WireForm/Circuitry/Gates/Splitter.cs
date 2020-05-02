using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Gates.Logic
{
    class Splitter : DynamicGate
    {
        [JsonConstructor]
        public Splitter(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-1, -1, 2, 1), new Vec2(-1, -1), new Vec2(1, 0), 1, 1)
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-1, -1))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0))
            };
        }

        public Splitter(Vec2 Position, Direction direction, int splitCount, int inputDepth, Split splitDirection)
            : this(Position, direction)
        {
            SplitDirection = splitDirection;
            SplitCount = splitCount;
            SplitDepth = inputDepth;
        }

        protected override void Compute()
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

        protected override void Draw(PainterScope painter)
        {
            painter.DrawLine(Color.DarkGray, 10, new Vec2(-1, -1), Vec2.Zero);

            painter.DrawLine(Color.DarkGray, 10, new Vec2(0 - 1 / 20f, 0), new Vec2(1, 0));
            painter.DrawStringC(GetRange(0), Color.Black, new Vec2(.4f, 0), 4);
            
            base.Draw(painter);

            if (splitDirection == 0)
            {
                painter.DrawStringC("I", Color.Black,  new Vec2(0f, -.5f), 3);
                painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 3);
                painter.DrawStringC("O", Color.Black, new Vec2(.8f, -.5f), 3);
            }
            else
            {
                painter.DrawStringC("O", Color.Black,  new Vec2(0f, -.5f), 3);
                painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 3);
                painter.DrawStringC("I", Color.Black, new Vec2(.8f, -.5f), 3);
            }
        }

        private string GetRange(int i)
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

        /// <summary>
        /// Resets the Inputs and Outputs to match an updated splitCount and/or bitDepth
        /// </summary>
        void ResetIO()
        {
            switch (splitDirection)
            {
                case Split.Expand:
                    InputCount  = 1;
                    OutputCount = splitCount;
                    break;
                case Split.Contract:
                    OutputCount = 1;
                    InputCount  = splitCount;
                    break;
            }
            LocalHitbox = new BoxCollider(-1, -1, 2, splitCount);
            return;
        }

        protected override Vec2[] GenerateInputPositions()
        {
            Vec2[] positions = new Vec2[InputCount];
            //Single input
            if (splitDirection == Split.Expand)
            {
                positions[0] = inputCenterLocal;
            }
            //Many input
            else if(SplitDirection == Split.Contract)
            {
                positions[0] = outputCenterLocal;
                for (int i = 1; i < positions.Length; i++) positions[i] = positions[i - 1] + new Vec2(0, 1);
            }
            
            return positions;
        }

        protected override Vec2[] GenerateOutputPositions()
        {
            Vec2[] positions = new Vec2[OutputCount];
            //Single output
            if (splitDirection == Split.Contract)
            {
                positions[0] = inputCenterLocal;
            }
            //Many output
            else
            {
                positions[0] = outputCenterLocal;
                for (int i = 1; i < positions.Length; i++) positions[i] = positions[i - 1] + new Vec2(0, 1);
            }

            return positions;
        }

        protected override void GenerateInputs()
        {
            base.GenerateInputs();
            //Single input
            if (splitDirection == Split.Expand) Inputs[0].Values = new BitArray(splitCount * splitDepth);
            //Many input
            else if (SplitDirection == Split.Contract) foreach (var input in Inputs) input.Values = new BitArray(splitDepth);
        }

        protected override void GenerateOutputs()
        {
            base.GenerateOutputs();
            //Single output
            if (splitDirection == Split.Contract) Outputs[0].Values = new BitArray(splitCount * splitDepth);
            //Many output
            else if (SplitDirection == Split.Expand) foreach (var output in Outputs) output.Values = new BitArray(splitDepth);

        }

        protected override void ExtensionLine(PainterScope painter, int gatePinCount, Vec2 centerLocal)
        {
            painter.DrawLine(Color.DarkGray, 10, Vec2.Zero, new Vec2(0, gatePinCount - 1));

            for (int i = 1; i < splitCount; i++)
            {
                painter.DrawLine(Color.DarkGray, 10, new Vec2(0, i), new Vec2(1, i));
                painter.DrawStringC(GetRange(i), Color.Black, new Vec2(.4f, i), 4);
            }
        }

        int splitCount = 1;
        [CircuitProperty(1, 32, true)]
        protected int SplitCount
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
        protected int SplitDepth
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
        /// The way in which the splitter expands or contracts inputs or outputs
        /// </summary>
        private Split splitDirection = Split.Expand;
        [CircuitAction("Flip", 'f')]
        [CircuitProperty(0, 1, true, new[] { "Expand", "Contract" })]
        protected Split SplitDirection
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
                    ResetIO();
                }
            }
        }

        [HideCircuitAttributes]
        public override int InputCount { get => base.InputCount; set => base.InputCount = value; }

        public override CircuitObject Copy()
        {
            Splitter splitter = new Splitter(StartPoint, Direction, splitCount, splitDepth, splitDirection);

            return splitter;
        }

        public enum Split
        {
            Expand,
            Contract
        }
    }
}
