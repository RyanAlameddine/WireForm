using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry.Data;
using WireForm.GraphicsUtils;

namespace WireForm.Input.States.Wire
{
    /// <summary>
    /// The state where the Wire tool is selected and the program sits idle.
    /// </summary>
    class WireToolState : InputState
    {
        public override bool IsClean() => true;

        public override InputReturns MouseLeftDown (StateControls stateControls) => (true, new DrawingWireState(stateControls.GriddedMousePosition));

        public override InputReturns MouseRightDown(StateControls stateControls) => (true, new RemovingWireState(stateControls.State.wires, stateControls.GriddedMousePosition, stateControls.State.Connections));
    }
}
