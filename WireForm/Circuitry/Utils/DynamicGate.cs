using Newtonsoft.Json;
using System.Drawing;
using System.Threading.Tasks;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using Wireform.Utils;

namespace Wireform.Circuitry.Utils
{
    /// <summary>
    /// Gate which is automatically able to change the amount of inputs/outputs.
    /// By default, inputs/outputs are dynamic and they are assumed to be placed in a vertical line centered at the given inputCenterLocal/outputCenterLocal.
    /// NOTE: Unlike classes which just extend Gate, the Inputs/Outputs DO NOT need to be implemented manually by the
    /// constructor. DynamicGate's constructor will do it for you.
    /// NOTE: Please call base.draw in your draw function to draw lines which extend out to match the extra inputs.
    /// </summary>
    public abstract class DynamicGate : Gate
    {
        /// <summary>
        /// Center for generated inputs in local coordinates
        /// </summary>
        [JsonIgnore]
        protected Vec2 inputCenterLocal;

        /// <summary>
        /// Center for generated outputs in local coordinates
        /// </summary>
        [JsonIgnore]
        protected Vec2 outputCenterLocal;

        /// <summary>
        /// After instantiating Gate, Generates Inputs and Outputs
        /// </summary>
        protected DynamicGate(Vec2 position, Direction direction, BoxCollider localHitbox, Vec2 inputCenterLocal, Vec2 outputCenterLocal, int inputCount, int outputCount) : base(position, direction, localHitbox)
        {
            this.inputCenterLocal = inputCenterLocal;
            this.outputCenterLocal = outputCenterLocal;

            //Generates inputs and outputs
            this.inputCount = inputCount;
            this.outputCount = outputCount;

            GenerateInputs();
            GenerateOutputs();
        }

        [JsonIgnore]
        private int inputCount;
        /// <summary>
        /// The amount of generated inputs for a dynamic gate.
        /// Setting this will update the Inputs if applicable.
        /// </summary>
        [CircuitDropdownAction("Increment inputs", 'i', true, PropertyOverflow.Clip)]
        [CircuitDropdownAction("Decrement inputs", 'i', Modifier.Shift, false, PropertyOverflow.Clip)]
        [CircuitPropertyDropdown(2, 32, true)]
        public virtual int InputCount
        {
            get => inputCount;
            set
            {
                inputCount = value;
                GenerateInputs();
            }
        }

        [JsonIgnore]
        private int outputCount;
        /// <summary>
        /// The amount of generated outputs for a dynamic gate.
        /// Setting this will update the Outputs if applicable.
        /// NOTE: Circuit property is hidden by default, but overriding it will enable it ([HideCircuitAttributes] is not inherited).
        /// </summary>
        [HideCircuitAttributes]
        [CircuitPropertyDropdown(2, 16, true)]
        public virtual int OutputCount
        {
            get => outputCount;
            set
            {
                outputCount = value;
                GenerateOutputs();
            }
        }

        /// <summary>
        /// Updates the inputs to match inputCount. 
        /// Will automatically be called by the setter on InputCount.
        /// </summary>
        protected virtual void GenerateInputs()
        {
            Inputs = GatePinsFromVec2s(GenerateInputPositions());
        }

        /// <summary>
        /// Updates the outputs to match inputCount. 
        /// Will automatically be called by the setter on OutputCount.
        /// </summary>
        protected virtual void GenerateOutputs()
        {
            Outputs = GatePinsFromVec2s(GenerateOutputPositions());
        }

        /// <summary>
        /// Fills the specified array with new gatepins from the given array of Vec2.
        /// This function is called by GenerateInputs and GenerateOutputs.
        /// </summary>
        protected GatePin[] GatePinsFromVec2s(Vec2[] positions)
        {
            var gatePins = new GatePin[positions.Length];
            for (int i = 0; i < gatePins.Length; i++)
            {
                gatePins[i] = new GatePin(this, positions[i]);
            }
            return gatePins;
        }

        /// <summary>
        /// Returns an array of length inputCount filled with the positions for each input to be generated.
        /// This function is called by GenerateInputs.
        /// </summary>
        protected virtual Vec2[] GenerateInputPositions()
        {
            return StandardGenerationPattern(inputCount, inputCenterLocal);
        }

        /// <summary>
        /// Returns an array of length outputLength filled with the positions for each output to be generated.
        /// This function is called by GenerateOutputs.
        /// </summary>
        protected virtual Vec2[] GenerateOutputPositions()
        {
            return StandardGenerationPattern(outputCount, outputCenterLocal);
        }


        /// <summary>
        /// Standard GatePin generation pattern centered at centerPosition.
        /// Starts at centerPosition+-1 and expands outward as count increases.
        /// This function is called by GenerateOutputPositions and GenerateInputPositions.
        /// </summary>
        protected Vec2[] StandardGenerationPattern(int count, Vec2 centerPosition)
        {
            Vec2[] positions = new Vec2[count];

            //If count = 1, position = centerPosition
            if(count == 1)
            {
                positions[0] = centerPosition;
                return positions;
            }

            //Special pattern for i = 0,1,2
            for (int i = 0; i < positions.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        positions[i] = centerPosition + new Vec2(0, 1);
                        break;
                    case 1:
                        positions[i] = centerPosition + new Vec2(0, -1);
                        break;
                    case 2:
                        positions[i] = centerPosition + new Vec2(0, 0);
                        break;
                }
                if (i > 2) break;
            }

            //Pattern for i>2
            for (int i = 3; i < positions.Length; i++)
            {
                int odd = (i - 1) % 2;
                int offset = (i - 1) / 2 + 1;
                switch (odd)
                {
                    case 0:
                        positions[i] = inputCenterLocal - new Vec2(0, offset);
                        break;
                    case 1:
                        positions[i] = inputCenterLocal + new Vec2(0, offset);
                        break;
                }
            }
            return positions;
        }

        protected override async Task DrawGate(PainterScope painter)
        {
            await ExtensionLine(painter, InputCount , inputCenterLocal );
            await ExtensionLine(painter, OutputCount, outputCenterLocal);
        }

        /// <summary>
        /// Draws the extension lines of the gate to make it clear that extra inputs are a part of the gate.
        /// This function is called by the DynamicGate.draw function.
        /// </summary>
        protected virtual async Task ExtensionLine(PainterScope painter, int gatePinCount, Vec2 centerLocal)
        {
            int highestOffset = gatePinCount / 2;
            int odd = (1 + gatePinCount) % 2 ;
            if (highestOffset > 1)
            {
                await painter.DrawLine(Color.Black, PenWidth, centerLocal + new Vec2(0, 1), centerLocal + new Vec2(0, highestOffset - odd));
                await painter.DrawLine(Color.Black, PenWidth, centerLocal - new Vec2(0, 1), centerLocal - new Vec2(0, highestOffset));
            }
        }
    }
}
