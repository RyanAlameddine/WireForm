using System;
using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using WireformInput.States.Selection;
using WireformInput.States.Wire;
using WireformInput.Utils;

namespace WireformInput
{
    /// <summary>
    /// A class which manages all input which can communicate with the Wireform.
    /// Each operation function takes in the current StateControls and outputs a boolean 
    /// for whether or not the operation requires a drawing refresh.
    /// </summary>
    public class InputStateManager
    {
        /// <summary>
        /// Returns a set of all the standard Tools and a new instance of each tool state
        /// This collection exists for convienience only, and is never neccessary to use.
        /// However, this is what will be checked when a Tools is passed into ChangeTool(Tools);
        /// </summary>
        public static IReadOnlyDictionary<Tools, InputState> StandardToolStates 
        { 
            get => new Dictionary<Tools, InputState>()
                {
                    { Tools.SelectionTool, new SelectionToolState() },
                    { Tools.WireTool, new WireToolState() },
                };
        }

        InputState state;
        readonly HashSet<CircuitObject> clipBoard;

        public InputStateManager()
        {
            clipBoard = new HashSet<CircuitObject>();

            state = new SelectionToolState();
        }

        private float scale = 50f;
        /// <summary>
        /// The zoom to draw with (small values = zoomed in)
        /// </summary>
        public float SizeScale
        {
            get => scale;
            set { scale = value > 5 ? value : 5; }
        }

        /// <summary>
        /// If the current state is clean, inserts the new state (loaded from StandardToolState) and returns true.
        /// If not, return false.
        /// </summary>
        public bool TryChangeTool(Tools newState)
        {
            return TryChangeTool(StandardToolStates[newState]);
        }

        /// <summary>
        /// If the current state is clean, inserts the new state and returns true.
        /// If not, return false.
        /// </summary>
        private bool TryChangeTool(InputState newState)
        {
            if (state.IsClean())
            {
                state = newState;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(BoardState currentState, PainterScope painter, Vec2 viewportSize)
        {
            //Draw grid
            int step = MathHelper.Ceiling((int)(viewportSize.X / SizeScale) / 50f);
            for (int x = 0; x * SizeScale < viewportSize.X; x += step)
                for (int y = 0; y * SizeScale < viewportSize.Y; y += step)
                    painter.FillRectangleC(Color.DarkBlue, new Vec2(x, y), new Vec2(.05f * step, .05f * step));
            //Draw gates
            foreach (Gate gate in currentState.gates) gate.DrawGate(painter);
            //Draw Wires
            foreach (WireLine wireLine in currentState.wires) WirePainter.DrawWireLine(painter, currentState, wireLine);
            //Draw state-specific info
            state.Draw(currentState, painter);
        }

        /// <summary>
        /// Evaluates the returnValue and updates internal state
        /// </summary>
        protected bool Eval(InputReturns returnValue)
        {
            //if (state != returnValue.state) 
            //    Debug.WriteLine($"{state.GetType().Name}->{returnValue.state.GetType().Name}");
            state = returnValue.state;
            return returnValue.toRefresh;
        }

        //Mouse Operations
        public bool MouseLeftDown (StateControls stateControls) => Eval(state.MouseLeftDown (stateControls));
        public bool MouseRightDown(StateControls stateControls) => Eval(state.MouseRightDown(stateControls));
        public bool MouseMove     (StateControls stateControls) => Eval(state.MouseMove     (stateControls));
        public bool MouseLeftUp   (StateControls stateControls) => Eval(state.MouseLeftUp   (stateControls));
        public bool MouseRightUp  (StateControls stateControls) => Eval(state.MouseRightUp  (stateControls));

        //Keyboard Operations
        public bool KeyDown(StateControls stateControls) => Eval(state.KeyDown(stateControls));
        public bool KeyUp  (StateControls stateControls) => Eval(state.KeyUp  (stateControls));

        //History Operations
        public bool Undo(StateControls stateControls) => Eval(state.Undo(stateControls));
        public bool Redo(StateControls stateControls) => Eval(state.Redo(stateControls));

        //Clipboard operations
        public bool Copy (StateControls stateControls) => Eval(state.Copy (stateControls, clipBoard));
        public bool Cut  (StateControls stateControls) => Eval(state.Cut  (stateControls, clipBoard));
        public bool Paste(StateControls stateControls) => Eval(state.Paste(stateControls, clipBoard));

        //Special operations
        /// <summary>
        /// Special operation that, if the current state IsClean, will change state to MovingSelectionState
        /// and place the newGate into the selections list.
        /// Returns a function which matches the 'takes in the current StateControls and outputs a boolean' pattern.
        /// NOTE: new gates can be greated through the GatesCollection class
        /// </summary>
        public Func<StateControls, bool> PlaceNewGate(Gate newGate)
        {
            return (stateControls =>
            {
                if (!state.IsClean()) return false;
                var newState = new MovingSelectionState(new Vec2(0, 0), new HashSet<CircuitObject>() { newGate }, newGate, stateControls.State, false);

                stateControls.CircuitPropertiesOutput = newState.GetUpdatedCircuitProperties(stateControls.RegisterChange);

                return Eval(new InputReturns(true, newState));
            });
        }
    }

    public enum Tools
    {
        SelectionTool,
        WireTool,
    }
}
