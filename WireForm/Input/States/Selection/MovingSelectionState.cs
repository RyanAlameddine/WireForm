using System.Collections.Generic;
using WireForm.Circuitry.Data;
using WireForm.MathUtils;

namespace WireForm.Input.States.Selection
{
    class MovingSelectionState : SelectionStateBase
    {
        public readonly Vec2 offset;
        public readonly CircuitObject selectedObject;

        public MovingSelectionState(Vec2 mousePosition, List<CircuitObject> selections, CircuitObject selectedObject)
        {
            Vec2 localPoint = LocalPoint(mousePosition);
            this.selections = selections;
            this.selectedObject = selectedObject;
            offset = selectedObject.StartPoint - localPoint;
        }
    }
}