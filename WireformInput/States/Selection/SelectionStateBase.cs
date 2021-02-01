using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Wireform.Circuitry;
using Wireform.Circuitry.CircuitAttributes.Utils;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using Wireform.Utils;
using WireformInput.Utils;

namespace WireformInput.States.Selection
{
    /// <summary>
    /// Base class for selection states which handles drawing of selections
    /// Handles the loading of Circuit Properties
    /// </summary>
    public abstract class SelectionStateBase : InputState
    {
        protected readonly HashSet<BoardObject> selections;

        protected SelectionStateBase(HashSet<BoardObject> selections)
        {
            this.selections = selections;
        }

        private readonly HashSet<BoardObject> oldSelections = new HashSet<BoardObject>();
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
        protected bool ExecuteHotkey(BoardState state, char? key, Modifier Modifiers, Action<string> registerChange, out CircuitPropertyCollection circuitProperties)
        {
            CircuitActionCollection actions = CircuitActionCollection.Empty;
            //Find all actions
            foreach(var selection in selections)
            {
                actions.UnionWith(CircuitAttributes.GetActions(selection, RefreshSelections, registerChange));
            }

            char hotkey = (char) key;
            //Execute matches
            bool toRefresh = actions.InvokeHotkeyActions(state, hotkey, Modifiers);
            circuitProperties = GetUpdatedCircuitProperties(registerChange);
            return toRefresh;
        }


        /// <summary>
        /// Check through selection list to confirm that everything still exists
        /// </summary>
        public void RefreshSelections(BoardState state)
        {
            int count = selections.Count;
            selections.RemoveWhere((x) => !state.BoardObjects.Contains(x));
        }

        public override async Task Draw(BoardState currentState, PainterScope painter)
        {
            //Draws selected gates and wires because they are technically no longer on the board
            foreach (BoardObject selection in selections)
            {
                await selection.Draw(painter, currentState);
                
                BoxCollider selectionBox = selection.HitBox;
                await painter.DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.Position, selectionBox.Bounds);
            }
        }
    }
}
