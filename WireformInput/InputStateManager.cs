using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using WireformInput.States.Selection;
using WireformInput.States.Text;
using WireformInput.States.Wire;
using WireformInput.Utils;

namespace WireformInput
{
    /// <summary>
    /// A class which manages all input which can communicate with the Wireform.
    /// Each operation function calls upon the IEventRunner to run the specified input event.
    /// </summary>
    public class InputStateManager
    {
        /// <summary>
        /// Returns a set of all the standard Tools and a new instance of each tool state
        /// This collection exists for convienience only, and is never neccessary to use.
        /// However, this is what will be checked when a Tools is passed into ChangeTool(Tools);
        /// </summary>
        public static Dictionary<Tools, Func<InputState>> ToolStates { get; set; } 
            = new Dictionary<Tools, Func<InputState>>()
                {
                    { Tools.SelectionTool, () => new SelectionToolState() },
                    { Tools.WireTool,      () => new WireToolState     () },
                    { Tools.TextTool,      () => new TextToolState     () },
                };

        public Tools SelectedTool { get; private set; }

        InputState state;
        readonly HashSet<BoardObject> clipBoard;
        public IEventRunner eventRunner;

        public InputStateManager(IEventRunner eventRunner)
        {
            clipBoard = new HashSet<BoardObject>();

            state = new SelectionToolState();
            SelectedTool = Tools.SelectionTool;

            this.eventRunner = eventRunner;
        }

        private float scale = 50f;
        /// <summary>
        /// The zoom to draw with (small values = zoomed in)
        /// </summary>
        public float Zoom
        {
            get => scale;
            set { scale = value > 5 ? value : 5; }
        }

        /// <summary>
        /// If the current state is clean, inserts the new state (loaded from <see cref="ToolStates"/>) and returns true.
        /// If not, return false.
        /// </summary>
        public bool TryChangeTool(Tools newState)
        {
            return TryChangeTool(ToolStates[newState]());
        }

        /// <summary>
        /// Tries to change the current state.
        /// If state is not clean, returns false, else true.
        /// </summary>
        private bool TryChangeTool(InputState newState)
        {
            if (!state.IsClean()) return false;

            Eval(state.CleanupState);
            state = newState;
            return true;
        }

        public async Task Draw(BoardState currentState, PainterScope painter)
        {
            //Draw board objects
            foreach (BoardObject obj in currentState.BoardObjects) await obj.Draw(painter, currentState);
            //Draw state-specific info
            await state.Draw(currentState, painter);
        }

        public async Task DrawGrid(PainterScope painter, Vec2 viewportSize)
        {
            //Draw grid
            int step = MathHelper.Ceiling((int)(viewportSize.X / Zoom) / 50f);
            for (int x = 0; x * Zoom < viewportSize.X; x += step)
                for (int y = 0; y * Zoom < viewportSize.Y; y += step)
                    await painter.FillRectangleC(Color.DarkBlue, new Vec2(x, y), new Vec2(.05f * step, .05f * step));
        }

        /// <summary>
        /// Evaluates the returnValue and updates internal state
        /// </summary>
        protected void Eval(Func<StateControls, InputReturns> inputEvent)
            => eventRunner.RunInputEvent((stateControls) =>
            {
                //if (state != returnValue.state) 
                //    Debug.WriteLine($"{state.GetType().Name}->{returnValue.state.GetType().Name}");
                InputReturns returnValue = inputEvent(stateControls);
                state = returnValue.state;
                return returnValue.toRefresh;
            });
            
        //Mouse Operations
        public void MouseLeftDown () => Eval(state.MouseLeftDown );
        public void MouseRightDown() => Eval(state.MouseRightDown);
        public void MouseMove     () => Eval(state.MouseMove     );
        public void MouseLeftUp   () => Eval(state.MouseLeftUp   );
        public void MouseRightUp  () => Eval(state.MouseRightUp  );

        //Keyboard Operations
        public void KeyDown() => Eval(state.KeyDown);
        public void KeyUp  () => Eval(state.KeyUp  );

        //History Operations
        public void Undo() => Eval(state.Undo);
        public void Redo() => Eval(state.Redo);

        //Clipboard operations
        public void Copy () => Eval((stateControls) => state.Copy (stateControls, clipBoard));
        public void Cut  () => Eval((stateControls) => state.Cut  (stateControls, clipBoard));
        public void Paste() => Eval((stateControls) => state.Paste(stateControls, clipBoard));

        //Special operations
        /// <summary>
        /// Special operation that, if the current state IsClean, will change state to MovingSelectionState
        /// and place the newGate into the selections list.
        /// NOTE: new gates can be greated through the GatesCollection class
        /// </summary>
        public void PlaceNewGate(Gate newGate)
        {
            Eval((stateControls) =>
            {
                if (state.IsClean()) {
                    var newState = new MovingSelectionState(new Vec2(0, 0), new HashSet<BoardObject>() { newGate }, newGate, stateControls.State, false);

                    TryChangeTool(newState);

                    stateControls.CircuitPropertiesOutput = newState.GetUpdatedCircuitProperties(stateControls.RegisterChange);

                    return new InputReturns(true, newState);
                }
                return new InputReturns(false, state);
            });
        }
    }

    public enum Tools
    {
        SelectionTool,
        WireTool,
        TextTool,
    }
}
