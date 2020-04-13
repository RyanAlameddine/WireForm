using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm.Input
{
    internal abstract class InputHandlerState
    {
        public virtual void Draw(BoardState currentState, PainterScope painter) { }

        public virtual InputReturns KeyDown(InputControls controls, KeyEventArgs keyEventArgs) => (false, this);

        public virtual InputReturns MouseLeftDown (InputControls controls) => (false, this);
        public virtual InputReturns MouseRightDown(InputControls controls) => (false, this);
        public virtual InputReturns MouseMove     (InputControls controls) => (false, this);
        public virtual InputReturns MouseLeftUp   (InputControls controls) => (false, this);
        public virtual InputReturns MouseRightUp  (InputControls controls) => (false, this);

        public virtual InputReturns Undo (InputControls controls) => (false, this);
        public virtual InputReturns Redo (InputControls controls) => (false, this);

        public virtual InputReturns Copy (InputControls controls, HashSet<CircuitObject> clipBoard) => (false, this);
        public virtual InputReturns Cut  (InputControls controls, HashSet<CircuitObject> clipBoard) => (false, this);
        public virtual InputReturns Paste(InputControls controls, HashSet<CircuitObject> clipBoard) => (false, this);
    }

    internal struct InputReturns
    {
        public readonly bool toRefresh;
        public readonly InputHandlerState state;

        public InputReturns(bool toRefresh, InputHandlerState state)
        {
            this.toRefresh = toRefresh;
            this.state = state;
        }

        public static implicit operator InputReturns((bool toRefresh, InputHandlerState state) returnValue)
        {
            return new InputReturns(returnValue.toRefresh, returnValue.state);
        }
    }
}
