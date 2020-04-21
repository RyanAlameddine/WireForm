using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            Vec2 localPoint = stateControls.LocalMousePosition;
            //if the shift key is held down, additive selection is activated. This will allow you to
            //edit the current set of selections (add/remove selections) without starting over.
            bool additiveSelection = stateControls.Modifiers.HasFlag(Keys.Shift);
            //true if you click a gate
            if (new BoxCollider(localPoint.X, localPoint.Y, 0, 0).GetIntersections(stateControls.State, true, out _, out var circuitObjects, false))
            {
                CircuitObject clickedcircuitObject = circuitObjects.First();
                //If you click something which is not already selected
                if (!selections.Contains(clickedcircuitObject))
                {
                    if (!additiveSelection) selections.Clear();
                    selections.Add(clickedcircuitObject);
                }
                //If you click something which is already selected with additiveSelection enabled, remove it
                else if (additiveSelection) selections.Remove(clickedcircuitObject);

                //Load [CircuitProperties] for clicked object
                if(selections.Count == 1) stateControls.CircuitPropertiesOutput = GetUpdatedCircuitProperties();

                return (true, new MovingSelectionState(stateControls.MousePosition, selections, clickedcircuitObject, stateControls.State, true));
            }
            //Begin dragging selection box
            //Clear selections if additiveSelection is not activated
            if (!additiveSelection) selections.Clear();
            return (true, new SelectingState(localPoint, selections));
        }

        public override InputReturns MouseRightDown(StateControls stateControls)
        {
            Vec2 localPoint = MathHelper.ViewportToLocalPoint(stateControls.MousePosition);
            //true if you click a gate
            if (new BoxCollider(localPoint.X, localPoint.Y, 0, 0).GetIntersections(stateControls.State, true, out _, out var circuitObjects, false))
            {
                CircuitObject clickedcircuitObject = circuitObjects.First();
                selections.Clear();
                selections.Add(clickedcircuitObject);
                //Load [CircuitActions]
                var actions = CircuitActionAttribute.GetActions(clickedcircuitObject, RefreshSelections);
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
            //if no objects can be copied, return
            if (selections.Count == 0) return (false, this);

            clipBoard.Clear();
            clipBoard.UnionWith(selections);

            return (false, this);
        }

        public override InputReturns Cut(StateControls stateControls, HashSet<CircuitObject> clipBoard)
        {
            //if no objects can be cut, return
            if (selections.Count == 0) return (false, this);

            clipBoard.Clear();
            foreach (var selection in selections)
            {
                selection.Delete(stateControls.State);
                clipBoard.Add(selection.Copy());
            }
            selections.Clear();

            stateControls.RegisterChange("Cut selections");
            return (true, this);
        }

        public override InputReturns Paste(StateControls stateControls, HashSet<CircuitObject> clipBoard)
        {
            selections.Clear();

            CircuitObject currentObject = null;
            //Average position of all added objects
            Vec2 averagePosition = Vec2.Zero;
            foreach (var obj in clipBoard)
            {
                var newObj = obj.Copy();
                selections.Add(newObj);

                if (currentObject == null) currentObject = newObj;

                averagePosition += newObj.StartPoint;
            }

            averagePosition /= selections.Count;

            //Moves object so that they are centered around the mouse for ease of selection
            foreach (var obj in selections)
            {
                Vec2 offset = obj.StartPoint - averagePosition + stateControls.LocalMousePosition;
                obj.SetPosition(offset);
            }

            //New objects pasted and are now being held
            if (selections.Count > 0) return (true, new MovingSelectionState(stateControls.MousePosition, selections, currentObject, stateControls.State, false));

            return (false, this);

        }
    }
}
