using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;
using WireformInput.Utils;

namespace WireformInput.States.Text
{
    /// <summary>
    /// currently unused/unfinished state
    /// </summary>
    class EditingTextState : InputState
    {
        private CircuitLabel circuitLabel;

        public EditingTextState(BoardState state, Vec2 position)
        {
            circuitLabel = new CircuitLabel(position);
        }

        public override InputReturns MouseLeftDown(StateControls stateControls)
        {
            //No need to duplicate logic
            return new TextToolState().MouseLeftDown(stateControls);
        }

        public override InputReturns KeyDown(StateControls stateControls)
        {
            circuitLabel.Text += stateControls.PressedKey;
            return (true, this);
        }
    }
}