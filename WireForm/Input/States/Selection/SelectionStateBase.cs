﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Input.States.Selection
{
    /// <summary>
    /// Base class for selection states which handles drawing of selections
    /// Handles the loading of Circuit Properties
    /// </summary>
    abstract class SelectionStateBase : InputState
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
        /// Returns a list of updated CircuitProperties to be passed into the InputControls
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
        /// Propogates hotkey through all selected CircuitObjects
        /// </summary>
        protected bool ExecuteHotkey(InputControls inputControls)
        {
            bool toRefresh = false;
            foreach(var selection in selections)
            {
                var actions = CircuitActionAttribute.GetActions(selection);
                foreach (var action in actions)
                {
                    if (action.Key == inputControls.Key && action.Modifiers == inputControls.Modifiers)
                    {
                        toRefresh = true;
                        action.Invoke(inputControls.State);
                    }
                }
            }
            string hotkey;
            if(inputControls.Modifiers == Keys.None)
            {
                hotkey = inputControls.Key.ToString();
            }
            else
            {
                string modifiers = inputControls.Modifiers.ToString().Replace(", ", "+");
                hotkey= $"{modifiers}+{inputControls.Key}";
            }
            if(toRefresh) inputControls.RegisterChange($"Executed hotkey {hotkey} on selection(s)");
            return toRefresh;
        }


        /// <summary>
        /// Check through selection list to confirm that everything still exists
        /// If even one item is removed, returns true. Else returns false
        /// </summary>
        public bool RefreshSelections(BoardState state)
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

            return count != selections.Count;
        }

        public override void Draw(BoardState currentState, PainterScope painter)
        {
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
