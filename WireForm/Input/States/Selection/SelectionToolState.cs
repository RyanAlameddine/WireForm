using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Input.States.Selection
{
    /// <summary>
    /// The state where the Selection tool is selected and the program sits idle.
    /// Also controls the right click action which loads CircuitActions.
    /// </summary>
    public class SelectionToolState : SelectionStateBase
    {
        public SelectionToolState() : base(new HashSet<CircuitObject>()) { }
        public SelectionToolState(HashSet<CircuitObject> selections) : base(selections) { }
        
        public override bool IsClean() => true;

        public override InputReturns KeyDown(StateControls stateControls)
        {
            bool toRefresh = ExecuteHotkey(stateControls);
            return (toRefresh, this);
        }

        public override InputReturns MouseLeftDown(StateControls stateControls)
        {
            Vec2 localPoint = MathHelper.ViewportToLocalPoint(stateControls.MousePosition);
            //true if you click a gate
            if (new BoxCollider(localPoint.X, localPoint.Y, 0, 0).GetIntersections(stateControls.State, true, out _, out var circuitObjects, false))
            {
                CircuitObject clickedcircuitObject = null;
                //Select first selected object
                foreach (var v in circuitObjects)
                {
                    clickedcircuitObject = v;
                }
                if (!selections.Contains(clickedcircuitObject))
                {
                    selections.Clear();
                    selections.Add(clickedcircuitObject);
                }

                stateControls.CircuitPropertiesOutput = GetUpdatedCircuitProperties();

                return (true, new MovingSelectionState(stateControls.MousePosition, selections, clickedcircuitObject, stateControls.State, true));
            }
            //Begin dragging selection box
            selections.Clear();
            return (true, new SelectingState(localPoint, selections));
        }

        public override InputReturns MouseRightDown(StateControls stateControls)
        {
            Vec2 localPoint = MathHelper.ViewportToLocalPoint(stateControls.MousePosition);
            //true if you click a gate
            if (new BoxCollider(localPoint.X, localPoint.Y, 0, 0).GetIntersections(stateControls.State, true, out _, out var circuitObjects, false))
            {
                CircuitObject clickedcircuitObject = null;
                //Select first selected object
                foreach (var v in circuitObjects)
                {
                    clickedcircuitObject = v;
                }

                var actions = CircuitActionAttribute.GetActions(clickedcircuitObject);
                stateControls.CircuitActionsOutput = new List<CircuitAct>();
                stateControls.CircuitActionsOutput.AddRange(actions);
                return (true, this);
            }
            return (false, this);
        }


        public override InputReturns Undo(StateControls stateControls)
        {
            selections.Clear();
            stateControls.Reverse();

            return (true, this);
        }

        public override InputReturns Redo(StateControls stateControls)
        {
            selections.Clear();
            stateControls.Advance();

            return (true, this);
        }

        public override InputReturns Copy(StateControls stateControls, HashSet<CircuitObject> clipBoard)
        {
            clipBoard.Clear();
            clipBoard.UnionWith(selections);

            return (false, this);
        }

        public override InputReturns Cut(StateControls stateControls, HashSet<CircuitObject> clipBoard)
        {
            clipBoard.Clear();
            foreach (var selection in selections)
            {
                selection.Delete(stateControls.State);
                clipBoard.Add(selection.Copy());
            }
            selections.Clear();

            if(clipBoard.Count == 0)
            {
                return (false, this);
            }

            stateControls.RegisterChange("Cut selections");
            return (true, this);
        }

        public override InputReturns Paste(StateControls stateControls, HashSet<CircuitObject> clipBoard)
        {
            selections.Clear();

            CircuitObject currentObject = null;
            foreach (var obj in clipBoard)
            {
                var newObj = obj.Copy();
                selections.Add(newObj);

                currentObject = newObj;
            }

            if(selections.Count > 0)
            {
                var newState = new MovingSelectionState(stateControls.MousePosition, selections, currentObject, stateControls.State, false);
                return (true, newState);
            }

            return (false, this);

        }
    }
}
