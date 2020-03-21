using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.Input.States.Selection;
using WireForm.MathUtils;

namespace WireForm.Input
{
    internal sealed class InputManager
    {
        InputState state;

        public InputManager()
        {
            state = new SelectionToolState(new List<CircuitObject>());
        }

        private bool Eval(InputReturns returnValue)
        {
            state = returnValue.state;
            return returnValue.toRefresh;
        }

        public bool MouseDown(BoardState currentState, Vec2 mousePosition, MouseButtons mouseButton)
        {
            return Eval(state.MouseDown(currentState, mousePosition, mouseButton));
        }

        public bool MouseMove(BoardState currentState, Vec2 mousePosition)
        {
            return Eval(state.MouseMove(currentState, mousePosition));
        }

        public bool MouseUp(BoardState currentState, Vec2 mousePosition)
        {
            return Eval(state.MouseUp(currentState, mousePosition));
        }
    }
}
