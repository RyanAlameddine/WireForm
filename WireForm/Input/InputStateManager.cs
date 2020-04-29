using System;
using System.Collections.Generic;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.GraphicsUtils;
using Wireform.Input.States.Selection;
using Wireform.Input.States.Wire;

namespace Wireform.Input
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

        /// <summary>
        /// If the current state is clean, inserts the new state (loaded from StandardToolState) and returns true.
        /// If not, return false.
        /// </summary>
        public bool ChangeTool(Tools newState)
        {
            return ChangeTool(StandardToolStates[newState]);
        }

        /// <summary>
        /// If the current state is clean, inserts the new state and returns true.
        /// If not, return false.
        /// </summary>
        public bool ChangeTool(InputState newState)
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

        public void Draw(BoardState currentState, PainterScope painter)
        {
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
        /// </summary>
        public Func<StateControls, bool> PlaceNewGate(Gate newGate)
        {
            return (stateControls =>
            {
                if (!state.IsClean()) return false;
                var newState = new MovingSelectionState(stateControls.LocalMousePosition, new HashSet<CircuitObject>() { newGate }, newGate, stateControls.State, false);

                stateControls.CircuitPropertiesOutput = newState.GetUpdatedCircuitProperties();

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
