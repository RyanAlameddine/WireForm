using System;
using System.Collections.Generic;
using System.Text;

namespace WireformInput
{
    /// <summary>
    /// An interface which represents a function RunInputEvent which handles input and output of an input function.
    /// When implementing RunInputEvent, there are a couple steps that should take place.
    /// First, StateControls should be created using all the current state and input information.
    /// Next, the inputEvent should be executed with the stateControls.
    /// Next, the stateControls.CircuitPropertiesOutput and stateControls.CircuitActionOutput should be dealt with.
    /// Finally, if the inputEvent returned true, refresh the drawing panel.
    /// </summary>
    public interface IEventRunner
    {
        /// <summary>
        /// Function for the UI layer to interact with the input manager.
        /// The inputEvent should be a function in the InputStateManager.
        /// </summary>
        void RunInputEvent(Func<StateControls, bool> inputEvent);
    }
}
