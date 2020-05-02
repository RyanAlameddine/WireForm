using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Wireform.Circuitry;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.CircuitAttributes.Utilities;
using Wireform.Circuitry.CircuitAttributes.Utils;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
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

        protected SelectionStateBase(HashSet<CircuitObject> selections)
        {
            this.selections = selections;
        }

        private readonly HashSet<CircuitObject> oldSelections = new HashSet<CircuitObject>();
        /// <summary>
        /// Returns a list of updated CircuitProperties to be passed into the StateControls
        /// </summary>
        public CircuitPropertyCollection GetUpdatedCircuitProperties(Action<string> registerChange)
        {
            if (!selections.SetEquals(oldSelections))
            {
                CircuitPropertyCollection circuitProperties = CircuitPropertyCollection.Empty;
                oldSelections.Clear();
                oldSelections.UnionWith(selections);
                foreach(var selection in selections)
                {
                    circuitProperties.Intersect(CircuitAttributes.GetProperties(selection, registerChange));
                }
                return circuitProperties;
            }

            return null;
        }

        /// <summary>
        /// Propogates hotkey through all selected CircuitObjects and runs any valid [CircuitProperties]
        /// </summary>
        protected bool ExecuteHotkey(StateControls stateControls)
        {
            CircuitActionCollection actions = CircuitActionCollection.Empty;
            //Find all actions
            foreach(var selection in selections)
            {
                actions.UnionWith(CircuitAttributes.GetActions(selection, RefreshSelections, stateControls.RegisterChange));
            }
            //Execute matches
            return actions.InvokeHotkeyActions(stateControls.State, stateControls.Hotkey, stateControls.Modifiers);
        }


        /// <summary>
        /// Check through selection list to confirm that everything still exists
        /// </summary>
        public void RefreshSelections(BoardState state)
        {
            int count = selections.Count;
            selections.RemoveWhere((x) =>
            {
                if (x is WireLine wire)
                {
                    return !state.wires.Contains(wire);
                }
                else if (x is Gate gate)
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
                    gate.DrawGate(painter);
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
