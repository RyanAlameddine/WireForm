using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Data.Bits;
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
                    BitValue[] bits = new BitValue[Outputs[i].Values.Count];
                    for (int j = 0; j < splitDepth; j++)
                    {
                        int k = splitDepth * i + j;

                        if (Inputs[0].Values.Count > k)
                        {
                            bits[j] = Inputs[0].Values[k];
                        }
                        else
                        {
                            bits[j] = BitValue.Error;
                        }
                    }
                    Outputs[i].Values = bits;
                }
            }
            //Combine lots of inputs to one large output
            else
            {
                BitValue[] bits = new BitValue[Outputs[0].Values.Count];
                for (int i = 0; i < Inputs.Length; i++)
                {
                    for (int j = 0; j < Inputs[i].Values.Count; j++)
                    {
                        int k = splitDepth * i + j;
                        if (Outputs[0].Values.Count > k)
                        {
                            bits[k] = Inputs[i].Values[j];
                        }
                        else
                        {
                            Debug.WriteLine("Splitter overflow");
                        }
                    }
                }
                Outputs[0].Values = bits;
            }
        }

        protected override async Task DrawGate(PainterScope painter)
        {
            await painter.DrawLine(Color.DarkGray, 10, new Vec2(-1, -1), Vec2.Zero);

            await painter.DrawLine(Color.DarkGray, 10, new Vec2(0 - 1 / 20f, 0), new Vec2(1, 0));
            await painter.DrawStringC(GetRange(0), Color.Black, new Vec2(.4f, 0), 1/4f);
            
            await base.DrawGate(painter);

            if (splitDirection == 0)
            {
                await painter.DrawStringC("I", Color.Black,  new Vec2(0f, -.5f), 1/3f);
                await painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 1/3f);
                await painter.DrawStringC("O", Color.Black, new Vec2(.8f, -.5f), 1/3f);
            }
            else
            {
                await painter.DrawStringC("O", Color.Black,  new Vec2(0f, -.5f), 1/3f);
                await painter.DrawStringC("/", Color.Black, new Vec2(.4f, -.5f), 1/3f);
                await painter.DrawStringC("I", Color.Black, new Vec2(.8f, -.5f), 1/3f);
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

        protected override async Task ExtensionLine(PainterScope painter, int gatePinCount, Vec2 centerLocal)
        {
            await painter.DrawLine(Color.DarkGray, 10, Vec2.Zero, new Vec2(0, gatePinCount - 1));

            for (int i = 1; i < splitCount; i++)
            {
                await painter.DrawLine(Color.DarkGray, 10, new Vec2(0, i), new Vec2(1, i));
                await painter.DrawStringC(GetRange(i), Color.Black, new Vec2(.4f, i), 1/4f);
            }
        }

        int splitCount = 1;
        /// <summary>
        /// The amount of split nodes
        /// </summary>
        [CircuitPropertyDropdown(1, 32, true)]
        [CircuitDropdownAction("Increment split count", 'i', true, PropertyOverflow.Clip)]
        [CircuitDropdownAction("Decrement split count", 'i', Modifier.Shift, false, PropertyOverflow.Clip)]
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
        [CircuitPropertyDropdown(1, 32, true)]
        [CircuitDropdownAction("Increment split depth", 'd', true, PropertyOverflow.Clip)]
        [CircuitDropdownAction("Decrement split depth", 'd', Modifier.Shift, false, PropertyOverflow.Clip)]
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
        [CircuitDropdownAction("Toggle split direction", 's', true)]
        [CircuitPropertyDropdown(0, 1, true, new[] { "Expand", "Contract" })]
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

        public override int Flipped { get => base.Flipped; set => base.Flipped = value; }

        [HideCircuitAttributes]
        public override int InputCount { get => base.InputCount; set => base.InputCount = value; }

        public override BoardObject Copy()
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
