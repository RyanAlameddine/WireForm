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
    internal sealed class InputManager
    {
        InputState state;
        readonly HashSet<CircuitObject> clipBoard;

        public InputManager()
        {
            state = new SelectionToolState(new HashSet<CircuitObject>());
            clipBoard = new HashSet<CircuitObject>();
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
        public bool MouseLeftDown (InputControls inputControls) => Eval(state.MouseLeftDown (inputControls));
        public bool MouseRightDown(InputControls inputControls) => Eval(state.MouseRightDown(inputControls));
        public bool MouseMove     (InputControls inputControls) => Eval(state.MouseMove     (inputControls));
        public bool MouseLeftUp   (InputControls inputControls) => Eval(state.MouseLeftUp   (inputControls));
        public bool MouseRightUp  (InputControls inputControls) => Eval(state.MouseRightUp  (inputControls));

        //Keyboard Operations
        public bool KeyDown(InputControls inputControls) => Eval(state.KeyDown(inputControls));

        //History Operations
        public bool Undo(InputControls inputControls) => Eval(state.Undo(inputControls));
        public bool Redo(InputControls inputControls) => Eval(state.Redo(inputControls));

        //Clipboard operations
        public bool Copy (InputControls inputControls) => Eval(state.Copy (inputControls, clipBoard));
        public bool Cut  (InputControls inputControls) => Eval(state.Cut  (inputControls, clipBoard));
        public bool Paste(InputControls inputControls) => Eval(state.Paste(inputControls, clipBoard));
    }
}
