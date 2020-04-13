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
        InputHandlerState state;
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
                Debug.WriteLine($"{state}->{returnValue.state}");
            state = returnValue.state;
            return returnValue.toRefresh;
        }

        public bool KeyDown(InputControls inputControls, KeyEventArgs keyEventArgs)
        {
            return Eval(state.KeyDown(inputControls, keyEventArgs));
        }

        public bool MouseLeftDown(InputControls inputControls, Vec2 mousePosition)
        {
            return Eval(state.MouseLeftDown(inputControls));
        }
        public bool MouseRightDown(InputControls inputControls, Vec2 mousePosition)
        {
            return Eval(state.MouseRightDown(inputControls));
        }

        public bool MouseMove(InputControls inputControls, Vec2 mousePosition)
        {
            return Eval(state.MouseMove(inputControls));
        }

        public bool MouseLeftUp(InputControls inputControls, Vec2 mousePosition)
        {
            return Eval(state.MouseLeftUp(inputControls));
        }
        public bool MouseRightUp(InputControls inputControls, Vec2 mousePosition)
        {
            return Eval(state.MouseRightUp(inputControls));
        }

        public bool Undo(InputControls inputControls)
        {
            return Eval(state.Undo(inputControls));
        }

        public bool Redo(InputControls inputControls)
        {
            return Eval(state.Redo(inputControls));
        }

        public bool Copy(InputControls inputControls)
        {
            return Eval(state.Copy(inputControls, clipBoard));
        }

        public bool Cut(InputControls inputControls)
        {
            return Eval(state.Cut(inputControls, clipBoard));
        }

        public bool Paste(InputControls inputControls)
        {
            return Eval(state.Paste(inputControls, clipBoard));
        }
    }
}
