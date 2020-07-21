using System.Collections.Generic;
using System.Linq;
using Wireform.Circuitry.CircuitAttributes.Utils;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using Wireform.Utils;
using WireformInput.Utils;

namespace WireformInput.States.Selection
{
    /// <summary>
    /// The state where the Selection tool is selected and the program sits idle.
    /// Also controls the right click action which loads CircuitActions.
    /// </summary>
    public class SelectionToolState : SelectionStateBase
    {
        public SelectionToolState() : base(new HashSet<BoardObject>()) { }
        public SelectionToolState(HashSet<BoardObject> selections) : base(selections) { }
        
        public override bool IsClean() => true;

        public override InputReturns KeyDown(StateControls stateControls)
        {
            bool toRefresh = ExecuteHotkey(stateControls.State, stateControls.PressedKeyLower, stateControls.Modifiers, stateControls.RegisterChange, out var circuitProperties);
            stateControls.CircuitPropertiesOutput = circuitProperties;
            return (toRefresh, this);
        }

        public override InputReturns MouseLeftDown(StateControls stateControls)
        {
            Vec2 localPoint = stateControls.LocalMousePosition;
            //if the shift key is held down, additive selection is activated. This will allow you to
            //edit the current set of selections (add/remove selections) without starting over.
            bool additiveSelection = stateControls.Modifiers.HasFlag(Modifier.Shift);
            //true if you click a gate
            if (new BoxCollider(localPoint.X, localPoint.Y, 0, 0).GetIntersections(stateControls.State, (true, true, true), out _, out var boardObjects, false))
            {
                BoardObject clickedBoardObject = boardObjects.First();
                //If you click something which is not already selected
                if (!selections.Contains(clickedBoardObject))
                {
                    if (!additiveSelection) selections.Clear();
                    selections.Add(clickedBoardObject);
                }
                //If you click something which is already selected with additiveSelection enabled, remove it
                else if (additiveSelection) selections.Remove(clickedBoardObject);

                //Load [CircuitProperties] for clicked object
                stateControls.CircuitPropertiesOutput = GetUpdatedCircuitProperties(stateControls.RegisterChange);

                return (true, new MovingSelectionState(stateControls.LocalMousePosition, selections, clickedBoardObject, stateControls.State, true));
            }
            //Begin dragging selection box
            //Clear selections if additiveSelection is not activated
            if (!additiveSelection)
            {
                selections.Clear();
                stateControls.CircuitPropertiesOutput = CircuitPropertyCollection.Empty;
            }
            return (true, new SelectingState(localPoint, selections));
        }

        public override InputReturns MouseRightDown(StateControls stateControls)
        {
            //true if you click a gate
            if (new BoxCollider(stateControls.LocalMousePosition.X, stateControls.LocalMousePosition.Y, 0, 0).GetIntersections(stateControls.State, (true, true, true), out _, out var boardObjects, false))
            {
                BoardObject clickedBoardObject = boardObjects.First();
                selections.Clear();
                selections.Add(clickedBoardObject);
                //Load [CircuitActions]
                var actions = CircuitAttributes.GetActions(clickedBoardObject, RefreshSelections, stateControls.RegisterChange);
                stateControls.CircuitActionsOutput = actions;
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

        public override InputReturns Copy(StateControls stateControls, HashSet<BoardObject> clipBoard)
        {
            //if no objects can be copied, return
            if (selections.Count == 0) return (false, this);

            clipBoard.Clear();
            clipBoard.UnionWith(selections);

            return (false, this);
        }

        public override InputReturns Cut(StateControls stateControls, HashSet<BoardObject> clipBoard)
        {
            //if no objects can be cut, return
            if (selections.Count == 0) return (false, this);

            clipBoard.Clear();
            foreach (var selection in selections)
            {
                selection.Delete(stateControls.State);
                clipBoard.Add((CircuitObject) selection.Copy());
            }
            selections.Clear();

            stateControls.RegisterChange("Cut selections");
            return (true, this);
        }

        public override InputReturns Paste(StateControls stateControls, HashSet<BoardObject> clipBoard)
        {
            selections.Clear();

            CircuitObject currentObject = null;
            //Average position of all added objects
            Vec2 averagePosition = Vec2.Zero;
            foreach (var obj in clipBoard)
            {
                var newObj = (CircuitObject) obj.Copy();
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

            stateControls.CircuitPropertiesOutput = GetUpdatedCircuitProperties(stateControls.RegisterChange);
            //New objects pasted and are now being held
            if (selections.Count > 0) return (true, new MovingSelectionState(stateControls.LocalMousePosition, selections, currentObject, stateControls.State, false));

            return (false, this);
        }
    }
}
