using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using Wireform.Utils;

namespace Wireform.Circuitry.Gates.Logic
{
    [Gate]
    class Splitter : DynamicGate
    {
        [JsonConstructor]
        public Splitter(Vec2 Position, Direction direction)
            : base(Position, direction, new BoxCollider(-1, -1, 2, 1), new Vec2(-1, -1), new Vec2(1, 0), 1, 1) { }

        public Splitter(Vec2 Position, Direction direction, int splitCount, int inputDepth, Split splitDirection)
            : this(Position, direction)
        {
            SplitDirection = splitDirection;
            SplitCount = splitCount;
            SplitDepth = inputDepth;
        }

        protected override void Compute()
        {
            //Split input into lots of outputs
            if (splitDirection == Split.Expand)
            {
                for (int i = 0; i < Outputs.Length; i++)
                {
                    for (int j = 0; j < splitDepth; j++)
                    {
                        int k = splitDepth * i + j;

                        if (Inputs[0].Values.Count > k)
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
            //Combine lots of inputs to one large output
            else
            {
                for (int i = 0; i < Inputs.Length; i++)
                {
                    for (int j = 0; j < Inputs[i].Values.Count; j++)
                    {
                        int k = splitDepth * i + j;
                        if (Outputs[0].Values.Count > k)
                        {
                            Outputs[0].Values.Set(k, Inputs[i].Values[j]);
                        }
                    }
                }
            }
        }

        protected override void Draw(PainterScope painter)
        {
            painter.DrawLine(Color.DarkGray, 10, new Vec2(-1, -1), Vec2.Zero);

            painter.DrawLine(Color.DarkGray, 10, new Vec2(0 - 1 / 20f, 0), new Vec2(1, 0));
            painter.DrawStringC(GetRange(0), Color.Black, new Vec2(.4f, 0), 1/4f);
            
            base.Draw(painter);

            if (splitDirection == 0)
            {
                painter.DrawStringC("I", Color.Black,  new Vec2(0f, -.5f), 1/3f);
                painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 1/3f);
                painter.DrawStringC("O", Color.Black, new Vec2(.8f, -.5f), 1/3f);
            }
            else
            {
                painter.DrawStringC("O", Color.Black,  new Vec2(0f, -.5f), 1/3f);
                painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 1/3f);
                painter.DrawStringC("I", Color.Black, new Vec2(.8f, -.5f), 1/3f);
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
                painter.DrawStringC(GetRange(i), Color.Black, new Vec2(.4f, i), 1/4f);
            }
        }

        int splitCount = 1;
        /// <summary>
        /// The amount of split nodes
        /// </summary>
        [CircuitProperty(1, 32, true)]
        [CircuitPropertyAction("Increment split count", 'i', true, PropertyOverflow.Clip)]
        [CircuitPropertyAction("Decrement split count", 'i', Modifier.Shift, false, PropertyOverflow.Clip)]
        public int SplitCount
        {
            get => splitCount;
            set
            {
                splitCount = value;
                ResetIO();
            }
        }

        int splitDepth = 1;
        /// <summary>
        /// The bitDepth of each split node
        /// </summary>
        [CircuitProperty(1, 32, true)]
        [CircuitPropertyAction("Increment split depth", 'd', true, PropertyOverflow.Clip)]
        [CircuitPropertyAction("Decrement split depth", 'd', Modifier.Shift, false, PropertyOverflow.Clip)]
        public int SplitDepth
        {
            get => splitDepth;
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
        [CircuitPropertyAction("Toggle split direction", 't', true)]
        [CircuitProperty(0, 1, true, new[] { "Expand", "Contract" })]
        public Split SplitDirection
        {
            get => splitDirection;
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
