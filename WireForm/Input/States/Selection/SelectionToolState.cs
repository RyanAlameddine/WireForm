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
    class SelectionToolState : SelectionStateBase
    {
        public SelectionToolState(HashSet<CircuitObject> selections) : base(selections) { }

        public override InputReturns MouseLeftDown(InputControls inputControls)
        {
            Vec2 localPoint = MathHelper.ViewportToLocalPoint(inputControls.MousePosition);
            //true if you click a gate
            if (new BoxCollider(localPoint.X, localPoint.Y, 0, 0).GetIntersections(inputControls.State, true, out _, out var circuitObjects, false))
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

                inputControls.CircuitPropertiesOutput = GetUpdatedCircuitProperties();

                return (true, new MovingSelectionState(inputControls.MousePosition, selections, clickedcircuitObject, inputControls.State, true));
            }
            //Begin dragging selection box
            selections.Clear();
            return (true, new SelectingState(localPoint, selections));
        }

        public override InputReturns MouseRightDown(InputControls inputControls)
        {
            Vec2 localPoint = MathHelper.ViewportToLocalPoint(inputControls.MousePosition);
            //true if you click a gate
            if (new BoxCollider(localPoint.X, localPoint.Y, 0, 0).GetIntersections(inputControls.State, true, out _, out var circuitObjects, false))
            {
                CircuitObject clickedcircuitObject = null;
                //Select first selected object
                foreach (var v in circuitObjects)
                {
                    clickedcircuitObject = v;
                }

                var actions = CircuitActionAttribute.GetActions(clickedcircuitObject, inputControls.State, inputControls.RegisterChange, inputControls.Refresh);
                inputControls.CircuitActionsOutput = new List<(CircuitActionAttribute attribute, EventHandler action)>();
                inputControls.CircuitActionsOutput.AddRange(actions);
                return (true, this);
            }
            return (false, this);
        }


        public override InputReturns Undo(InputControls inputControls)
        {
            selections.Clear();
            inputControls.Reverse();

            return (true, this);
        }

        public override InputReturns Redo(InputControls inputControls)
        {
            selections.Clear();
            inputControls.Advance();

            return (true, this);
        }

        public override InputReturns Copy(InputControls inputControls, HashSet<CircuitObject> clipBoard)
        {
            clipBoard.Clear();
            clipBoard.UnionWith(selections);

            return (false, this);
        }

        public override InputReturns Cut(InputControls inputControls, HashSet<CircuitObject> clipBoard)
        {
            clipBoard.Clear();
            foreach (var selection in selections)
            {
                selection.Delete(inputControls.State);
                clipBoard.Add(selection.Copy());
            }
            selections.Clear();

            if(clipBoard.Count == 0)
            {
                return (false, this);
            }

            inputControls.RegisterChange("Cut selections");
            return (true, this);
        }

        public override InputReturns Paste(InputControls inputControls, HashSet<CircuitObject> clipBoard)
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
                var newState = new MovingSelectionState(inputControls.MousePosition, selections, currentObject, inputControls.State, false);
                return (true, newState);
            }

            return (false, this);

        }
    }
}
