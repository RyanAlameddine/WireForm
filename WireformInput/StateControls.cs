using System;
using Wireform.Circuitry.CircuitAttributes.Utils;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;
using Wireform.Utils;

namespace WireformInput
{
    public class StateControls
    {
        /// <summary>
        /// The current board state
        /// </summary>
        public BoardState State { get; }

        /// <summary>
        /// The current mouse position in viewport coordinates
        /// </summary>
        public Vec2 MousePosition { get; }

        /// <summary>
        /// If this control is being created for a keyboard event, should contain the lowercase key of note
        /// </summary>
        public char? PressedKeyLower { get; }

        /// <summary>
        /// If this control is being created for a keyboard event, should contain the key of note including the correct case
        /// </summary>
        public char? PressedKey { get; }

        /// <summary>
        /// The key modifiers currently being pressed down
        /// </summary>
        public Modifier Modifiers { get; }

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
        public CircuitActionCollection CircuitActionsOutput { get; set; } = null;

        /// <summary>
        /// A list of [CircuitProperties] which should be loaded.
        /// null if no update is required
        /// </summary>
        public CircuitPropertyCollection CircuitPropertiesOutput { get; set; } = null;

        public StateControls(BoardState state, Vec2 mousePosition, float Zoom, char? pressedKey, Modifier modifiers, Action<string> registerChange, Action reverse, Action advance)
        {
            this.State = state;
            this.MousePosition = mousePosition;
            this.PressedKeyLower = pressedKey == null ? pressedKey : pressedKey.ToString().ToLower()[0];
            this.PressedKey = pressedKey;
            this.Modifiers = modifiers;
            this.RegisterChange = registerChange;
            this.Reverse = reverse;
            this.Advance = advance;

            this.LocalMousePosition = MathHelper.ViewportToLocalPoint(mousePosition, Zoom);
            this.GriddedMousePosition = this.LocalMousePosition.Round();
        }
    }
}
