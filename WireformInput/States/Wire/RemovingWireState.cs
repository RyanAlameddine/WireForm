using System.Collections.Generic;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;
using WireformInput.Utils;

namespace WireformInput.States.Wire
{
    /// <summary>
    /// State for removing wires holding the right click button
    /// </summary>
    class RemovingWireState : InputState
    {
        public RemovingWireState(List<WireLine> wires, Vec2 griddedMousePoint, Dictionary<Vec2, List<DrawableObject>> connections)
        {
            removeWire(wires, griddedMousePoint, connections);
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {
            return removeWire(stateControls.State.Wires, stateControls.GriddedMousePosition, stateControls.State.Connections);
        }

        public override InputReturns MouseRightUp(StateControls stateControls)
        {
            stateControls.RegisterChange("Deleted Wires");
            return (true, new WireToolState());
        }

        private InputReturns removeWire(List<WireLine> wires, Vec2 griddedMousePoint, Dictionary<Vec2, List<DrawableObject>> connections)
        {
            for (int i = 0; i < wires.Count; i++)
            {
                if (griddedMousePoint.IsContainedIn(wires[i]))
                {
                    WireLine.RemovePointFromWire(griddedMousePoint, connections, wires, i);

                    return (true, this);
                }
            }

            return (false, this);
        }
    }
}
