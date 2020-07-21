using System;
using System.Windows.Forms;
using Wireform;
using Wireform.Circuitry.CircuitAttributes.Utils;
using Wireform.MathUtils;
using Wireform.Utils;
using WireformInput;

namespace WinformsWireform.Helpers
{
    internal class FormsEventRunner : IEventRunner
    {
        public InputStateManager stateManager;
        public readonly BoardStack        stateStack;

        public readonly ContextMenuStrip  rightClickMenu;
        public readonly ListBox           circuitPropertyBox;
        public readonly ComboBox          circuitPropertyValueBox;
        public readonly TextBox           circuitPropertyTextBox;
        public readonly Panel             drawingPanel;
        public readonly Func<Keys>        getModifiers;
        public readonly Func<char?>       getKey;

        public FormsEventRunner(BoardStack stateStack, ContextMenuStrip rightClickMenu, ListBox circuitPropertyBox, ComboBox circuitPropertyValueBox, TextBox circuitPropertyTextBox, Panel drawingPanel, Func<Keys> getModifiers, Func<char?> getKey)
        {
            this.rightClickMenu = rightClickMenu;
            this.circuitPropertyBox = circuitPropertyBox;
            this.circuitPropertyValueBox = circuitPropertyValueBox;
            this.circuitPropertyTextBox = circuitPropertyTextBox;
            this.drawingPanel = drawingPanel;
            this.stateStack = stateStack;
            this.getModifiers = getModifiers;
            this.getKey = getKey;
        }

        public CircuitPropertyCollection CircuitProperties { get; private set; }

        /// <summary>
        /// Function for the Form to interact with the input manager.
        /// The inputEvent should be a function in the InputStateManager.
        /// </summary>
        /// <param name="key">if a key was not pressed this event, null, else char</param>
        public void RunInputEvent(Func<StateControls, bool> inputEvent)
        {
            char? key = getKey();

            var stateControls = MakeControls(key, getModifiers());
            bool toRefresh = inputEvent(stateControls);

            //Process [CircuitActions]
            if (stateControls.CircuitActionsOutput != null)
            {
                rightClickMenu.Items.Clear();

                foreach (var action in stateControls.CircuitActionsOutput)
                {
                    //Creates a dropdown menu item which, when clicked, will invoke the action and refresh the drawing panel
                    void actionEvent(object s, EventArgs e) => stateControls.CircuitActionsOutput.InvokeActionAndRefresh(stateControls.State, drawingPanel.Refresh, action);
                    var item = new ToolStripMenuItem(action.Name, null, actionEvent)
                    {
                        ShortcutKeyDisplayString = action.Hotkey.GetHotkeyString(action.Modifiers),
                        ShowShortcutKeys = true
                    };
                    rightClickMenu.Items.Add(item);
                }

                rightClickMenu.Show(drawingPanel, drawingPanel.PointToClient(Cursor.Position));
            }

            //Process [CircuitProperties]
            if (stateControls.CircuitPropertiesOutput != null)
            {
                circuitPropertyBox.Items.Clear();
                circuitPropertyTextBox.Visible = false;
                circuitPropertyValueBox.Visible = true;
                circuitPropertyValueBox.Items.Clear();
                CircuitProperties = stateControls.CircuitPropertiesOutput;

                foreach (var property in CircuitProperties)
                {
                    circuitPropertyBox.Items.Add(property.Key);
                }
            }

            if (toRefresh) drawingPanel.Refresh();
        }

        /// <summary>
        /// Helper function which makes StateControls
        /// </summary>
        public StateControls MakeControls(char? keyChar, Keys ModifierKeys)
        {
            var mousePoint = (Vec2)drawingPanel.PointToClient(Cursor.Position);
            Modifier modifierKeys = Modifier.None;
            if (ModifierKeys.HasFlag(Keys.Control)) modifierKeys |= Modifier.Control;
            if (ModifierKeys.HasFlag(Keys.Shift))   modifierKeys |= Modifier.Shift;
            if (ModifierKeys.HasFlag(Keys.Alt))     modifierKeys |= Modifier.Alt;
            var stateControls = new StateControls(stateStack.CurrentState, mousePoint, stateManager.Zoom, keyChar, modifierKeys, stateStack.RegisterChange, stateStack.Reverse, stateStack.Advance);
            return stateControls;
        }
    }
}
