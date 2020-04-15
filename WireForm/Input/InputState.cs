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
    /// <summary>
    /// Base class for all input states. Contains a set of functions involving mouse input, keyboard input, etc.
    /// which all take in and output input information through InputControls
    /// </summary>
    internal abstract class InputState
    {
        public virtual void Draw(BoardState currentState, PainterScope painter) { }

        //Mouse Operations
        public virtual InputReturns MouseLeftDown (InputControls inputControls) => (false, this);
        public virtual InputReturns MouseRightDown(InputControls inputControls) => (false, this);
        public virtual InputReturns MouseMove     (InputControls inputControls) => (false, this);
        public virtual InputReturns MouseLeftUp   (InputControls inputControls) => (false, this);
        public virtual InputReturns MouseRightUp  (InputControls inputControls) => (false, this);

        //Keyboard Operations
        public virtual InputReturns KeyDown(InputControls inputControls) => (false, this);

        //History Operations
        public virtual InputReturns Undo(InputControls inputControls) => (false, this);
        public virtual InputReturns Redo(InputControls inputControls) => (false, this);
                                                                         
        //Clipboard operations
        public virtual InputReturns Copy (InputControls inputControls, HashSet<CircuitObject> clipBoard) => (false, this);
        public virtual InputReturns Cut  (InputControls inputControls, HashSet<CircuitObject> clipBoard) => (false, this);
        public virtual InputReturns Paste(InputControls inputControls, HashSet<CircuitObject> clipBoard) => (false, this);
    }

    internal struct InputReturns
    {
        public readonly bool toRefresh;
        public readonly InputState state;

        public InputReturns(bool toRefresh, InputState state)
        {
            this.toRefresh = toRefresh;
            this.state = state;
        }

        public static implicit operator InputReturns((bool toRefresh, InputState state) returnValue)
        {
            return new InputReturns(returnValue.toRefresh, returnValue.state);
        }
    }
}
