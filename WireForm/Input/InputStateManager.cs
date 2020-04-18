using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.GraphicsUtils;
using WireForm.Input.States.Selection;
using WireForm.MathUtils;

namespace WireForm.Input
{
    public class InputStateManager
    {
        InputState state;
        readonly HashSet<CircuitObject> clipBoard;

        public InputStateManager(InputState startingState = null)
        {
            clipBoard = new HashSet<CircuitObject>();

            if (startingState != null) state = startingState;
            else                       state = new SelectionToolState();
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

        private bool Eval(InputReturns returnValue)
        {
            if (state != returnValue.state) 
                Debug.WriteLine($"{state.GetType().Name}->{returnValue.state.GetType().Name}");
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

        //History Operations
        public bool Undo(StateControls stateControls) => Eval(state.Undo(stateControls));
        public bool Redo(StateControls stateControls) => Eval(state.Redo(stateControls));

        //Clipboard operations
        public bool Copy (StateControls stateControls) => Eval(state.Copy (stateControls, clipBoard));
        public bool Cut  (StateControls stateControls) => Eval(state.Cut  (stateControls, clipBoard));
        public bool Paste(StateControls stateControls) => Eval(state.Paste(stateControls, clipBoard));
    }
}
