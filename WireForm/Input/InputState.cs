using System.Collections.Generic;
using Wireform.Circuitry.Data;
using Wireform.GraphicsUtils;

namespace Wireform.Input
{
    /// <summary>
    /// Base class for all input states. Contains a set of functions involving mouse input, keyboard input, etc.
    /// which all take in and output input information through StateControls
    /// </summary>
    public abstract class InputState
    {
        /// <summary>
        /// Returns true if the current state is completely clean, meaning the state manager can manually switch it 
        /// to a new state at will without notifying the current state.
        /// </summary>
        public virtual bool IsClean() => false;

        public virtual void Draw(BoardState currentState, PainterScope painter) { }

        //Mouse Operations
        public virtual InputReturns MouseLeftDown (StateControls stateControls) => (false, this);
        public virtual InputReturns MouseRightDown(StateControls stateControls) => (false, this);
        public virtual InputReturns MouseMove     (StateControls stateControls) => (false, this);
        public virtual InputReturns MouseLeftUp   (StateControls stateControls) => (false, this);
        public virtual InputReturns MouseRightUp  (StateControls stateControls) => (false, this);

        //Keyboard Operations
        public virtual InputReturns KeyDown(StateControls stateControls) => (false, this);
        public virtual InputReturns KeyUp  (StateControls stateControls) => (false, this);

        //History Operations
        public virtual InputReturns Undo(StateControls stateControls) => (false, this);
        public virtual InputReturns Redo(StateControls stateControls) => (false, this);
                                                                         
        //Clipboard operations
        public virtual InputReturns Copy (StateControls stateControls, HashSet<CircuitObject> clipBoard) => (false, this);
        public virtual InputReturns Cut  (StateControls stateControls, HashSet<CircuitObject> clipBoard) => (false, this);
        public virtual InputReturns Paste(StateControls stateControls, HashSet<CircuitObject> clipBoard) => (false, this);
    }

    public  struct InputReturns
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
