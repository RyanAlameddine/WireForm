using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.MathUtils;

namespace WireForm.Input
{
    public class StateControls
    {
        /// <summary>
        /// The current board state
        /// </summary>
        public BoardState State { get; }

        /// <summary>
        /// The current mouse position in screen coordinates
        /// </summary>
        public Vec2 MousePosition { get; }

        /// <summary>
        /// A function which will re-draw the current screen
        /// </summary>
        public Action Refresh { get; }

        /// <summary>
        /// The key currently being pressed down
        /// </summary>
        public Keys Key { get; }

        /// <summary>
        /// The key modifiers currently being pressed down
        /// </summary>
        public Keys Modifiers { get; }

        /// <summary>
        /// The function which registers a change to the stateStack
        /// </summary>
        public Action<string> RegisterChange { get; }

        /// <summary>
        /// The function which reverses (undo) a change to the stateStack
        /// </summary>
        public Action Reverse { get; }

        /// <summary>
        /// The function which advances (redo) a change to the stateStack
        /// </summary>
        public Action Advance { get; }


        /// <summary>
        /// The current mouse position in grid coordinates
        /// </summary>
        public Vec2 LocalMousePosition { get; }

        /// <summary>
        /// The current mouse position in grid coordinates rounded to the nearest grid poitn
        /// </summary>
        public Vec2 GriddedMousePosition { get; }
        

        /// <summary>
        /// A list of [CircuitActions] which should be loaded.
        /// null if no update is required
        /// </summary>
        public List<CircuitAct> CircuitActionsOutput { get; set; } = null;

        /// <summary>
        /// A list of [CircuitProperties] which should be loaded.
        /// null if no update is required
        /// </summary>
        public List<CircuitProp> CircuitPropertiesOutput { get; set; } = null;

        public StateControls(BoardState state, Vec2 mousePosition, Keys key, Keys modifiers, Action refresh, Action<string> registerChange, Action reverse, Action advance)
        {
            this.State = state;
            this.MousePosition = mousePosition;
            this.Key = key;
            this.Modifiers = modifiers;
            this.Refresh = refresh;
            this.RegisterChange = registerChange;
            this.Reverse = reverse;
            this.Advance = advance;

            this.LocalMousePosition = MathHelper.ViewportToLocalPoint(mousePosition);
            this.GriddedMousePosition = this.LocalMousePosition.Round();
        }
    }
}
