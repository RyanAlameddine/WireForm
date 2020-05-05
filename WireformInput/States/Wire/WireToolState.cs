using WireformInput.Utils;

namespace WireformInput.States.Wire
{
    /// <summary>
    /// The state where the Wire tool is selected and the program sits idle.
    /// </summary>
    class WireToolState : InputState
    {
        public override bool IsClean() => true;

        public override InputReturns MouseLeftDown (StateControls stateControls) => (true, new DrawingWireState(stateControls.GriddedMousePosition));

        public override InputReturns MouseRightDown(StateControls stateControls) => (true, new RemovingWireState(stateControls.State.Wires, stateControls.GriddedMousePosition, stateControls.State.Connections));

        public override InputReturns Undo(StateControls stateControls)
        {
            stateControls.Reverse();

            return (true, this);
        }

        public override InputReturns Redo(StateControls stateControls)
        {
            stateControls.Advance();

            return (true, this);
        }
    }
}
