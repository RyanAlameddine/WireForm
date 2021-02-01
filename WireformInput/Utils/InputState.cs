using System.Collections.Generic;
using System.Threading.Tasks;
using Wireform.Circuitry.Data;
using Wireform.GraphicsUtils;

namespace WireformInput.Utils
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

        public virtual Task Draw(BoardState currentState, PainterScope painter) => Task.CompletedTask;

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
        public virtual InputReturns Copy (StateControls stateControls, HashSet<BoardObject> clipBoard) => (false, this);
        public virtual InputReturns Cut  (StateControls stateControls, HashSet<BoardObject> clipBoard) => (false, this);
        public virtual InputReturns Paste(StateControls stateControls, HashSet<BoardObject> clipBoard) => (false, this);

        //Special operations
        /// <summary>
        /// This function is called only if IsClean returned true, and the state manager wishes to change the current
        /// state. It is the only InputState with no return value.
        /// </summary>
        public virtual InputReturns CleanupState(StateControls stateControls) => (false, this);
    }

    public struct InputReturns
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

        public static bool operator ==(InputReturns left, InputReturns right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InputReturns left, InputReturns right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is InputReturns returns &&
                   toRefresh == returns.toRefresh &&
                   EqualityComparer<InputState>.Default.Equals(state, returns.state);
        }

        public override int GetHashCode()
        {
            var hashCode = 1107143965;
            hashCode = hashCode * -1521134295 + toRefresh.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<InputState>.Default.GetHashCode(state);
            return hashCode;
        }
    }
}
