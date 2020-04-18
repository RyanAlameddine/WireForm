using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.MathUtils;

namespace WireForm.Input.States.Wire
{
    class RemovingWireState : InputState
    {
        public RemovingWireState(List<WireLine> wires, Vec2 griddedMousePoint, Dictionary<Vec2, List<BoardObject>> connections)
        {
            removeWire(wires, griddedMousePoint, connections);
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {
            return removeWire(stateControls.State.wires, stateControls.GriddedMousePosition, stateControls.State.Connections);
        }

        public override InputReturns MouseRightUp(StateControls stateControls)
        {
            stateControls.RegisterChange("Deleted Wires");
            return (false, new WireToolState());
        }

        private InputReturns removeWire(List<WireLine> wires, Vec2 griddedMousePoint, Dictionary<Vec2, List<BoardObject>> connections)
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
