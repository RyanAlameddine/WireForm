using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Wireform.Circuitry;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Input.States.Selection
{
    /// <summary>
    /// Base class for selection states which handles drawing of selections
    /// Handles the loading of Circuit Properties
    /// </summary>
    public abstract class SelectionStateBase : InputState
    {
        protected readonly HashSet<CircuitObject> selections;

        public SelectionStateBase(HashSet<CircuitObject> selections)
        {
            this.selections = selections;
        }

        /// <summary>
        /// The last object whose [CircuitProperties] were loaded
        /// </summary>
        protected CircuitObject previousObject = null;

        /// <summary>
        /// Returns a list of updated CircuitProperties to be passed into the StateControls
        /// </summary>
        protected List<CircuitProp> GetUpdatedCircuitProperties()
        {
            List<CircuitProp> circuitProperties = null;
            var gates = selections.Where((x) => x is Gate);
            if(gates.Count() == 1)
            {
                var newObject = gates.First();
                if(newObject != previousObject)
                {
                    circuitProperties = CircuitPropertyAttribute.GetProperties(newObject);
                    previousObject = newObject;
                }
            }else if(previousObject != null)
            {
                previousObject = null;
                circuitProperties = new List<CircuitProp>();
            }
            return circuitProperties;
        }

        /// <summary>
        /// Propogates hotkey through all selected CircuitObjects and runs any valid [CircuitProperties]
        /// </summary>
        protected bool ExecuteHotkey(StateControls stateControls)
        {
            bool toRefresh = false;
            List<CircuitAct> actions = new List<CircuitAct>();
            //Find all actions
            foreach(var selection in selections)
            {
                actions.AddRange(CircuitActionAttribute.GetActions(selection, RefreshSelections));
            }
            //Execute matches
            foreach (var action in actions)
            {
                if (action.Hotkey == stateControls.Hotkey && action.Modifiers == stateControls.Modifiers)
                {
                    toRefresh = true;
                    action.Invoke(stateControls.State);
                }
            }

            string hotkey;
            if (stateControls.Modifiers == Modifier.None) hotkey = stateControls.Hotkey + "";
            else hotkey = $"{stateControls.Modifiers.ToString().Replace(", ", "+")}+{stateControls.Hotkey}";

            if(toRefresh) stateControls.RegisterChange($"Executed hotkey {hotkey} on selection(s)");
            return toRefresh;
        }


        /// <summary>
        /// Check through selection list to confirm that everything still exists
        /// </summary>
        public void RefreshSelections(BoardState state)
        {
            int count = selections.Count;
            selections.RemoveWhere((x) =>
            {
                var gate = x as Gate;
                var wire = x as WireLine;
                if (wire != null)
                {
                    return !state.wires.Contains(wire);
                }
                else if (gate != null)
                {
                    return !state.gates.Contains(gate);
                }
                else
                {
                    throw new Exception("Invalid object selected");
                }
            });
        }

        public override void Draw(BoardState currentState, PainterScope painter)
        {
            //Draws selected gates and wires because they are technically no longer on the board
            foreach (CircuitObject selection in selections)
            {
                if (selection is Gate gate)
                {
                    gate.Draw(painter);
                    BoxCollider selectionBox = selection.HitBox;

                    painter.DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.Position, selectionBox.Bounds);

                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selectionBox.Position, new Vec2(.4f, .4f));
                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, new Vec2(selectionBox.X + selectionBox.Width, selectionBox.Y), new Vec2(.4f, .4f));
                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, new Vec2(selectionBox.X, selectionBox.Y + selectionBox.Height), new Vec2(.4f, .4f));
                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, new Vec2(selectionBox.X + selectionBox.Width, selectionBox.Y + selectionBox.Height), new Vec2(.4f, .4f));
                }
                if (selection is WireLine wire)
                {
                    BoxCollider selectionBox = selection.HitBox;

                    painter.DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.Position, selectionBox.Bounds);
                    WirePainter.DrawWireLine(painter, currentState, wire, new[] { Color.FromArgb(255, 0, 128, 128) });
                }
            }
        }
    }
}
