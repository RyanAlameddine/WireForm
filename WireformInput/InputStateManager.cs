using System;
using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using WireformInput.States.Selection;
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
        public IEventRunner eventRunner;

        public InputStateManager(IEventRunner eventRunner)
        {
            clipBoard = new HashSet<CircuitObject>();

            state = new SelectionToolState();

            this.eventRunner = eventRunner;
        }

        private float scale = 50f;
        /// <summary>
        /// The zoom to draw with (small values = zoomed in)
        /// </summary>
        public float SizeScale
        {
            get => scale;
            set { scale = value > 5 ? value : 5; }
        }

        /// <summary>
        /// If the current state is clean, inserts the new state (loaded from StandardToolState) and returns true.
        /// If not, return false.
        /// </summary>
        public bool TryChangeTool(Tools newState)
        {
            return TryChangeTool(StandardToolStates[newState]);
        }

        /// <summary>
        /// If the current state is clean, inserts the new state and returns true.
        /// If not, return false.
        /// </summary>
        private bool TryChangeTool(InputState newState)
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

        public void Draw(BoardState currentState, PainterScope painter, Vec2 viewportSize)
        {
            //Draw grid
            int step = MathHelper.Ceiling((int)(viewportSize.X / SizeScale) / 50f);
            for (int x = 0; x * SizeScale < viewportSize.X; x += step)
                for (int y = 0; y * SizeScale < viewportSize.Y; y += step)
                    painter.FillRectangleC(Color.DarkBlue, new Vec2(x, y), new Vec2(.05f * step, .05f * step));
            //Draw gates
            foreach (Gate gate in currentState.Gates) gate.Draw(painter, currentState);
            //Draw Wires
            foreach (WireLine wireLine in currentState.Wires) wireLine.Draw(painter, currentState);
            //Draw state-specific info
            state.Draw(currentState, painter);
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
                if (!state.IsClean()) return new InputReturns(false, state);
                var newState = new MovingSelectionState(new Vec2(0, 0), new HashSet<CircuitObject>() { newGate }, newGate, stateControls.State, false);

                stateControls.CircuitPropertiesOutput = newState.GetUpdatedCircuitProperties(stateControls.RegisterChange);

                return new InputReturns(true, newState);
            });
        }
    }

    public enum Tools
    {
        SelectionTool,
        WireTool,
    }
}
