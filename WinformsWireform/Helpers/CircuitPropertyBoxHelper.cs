using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsWireform.Helpers
{
    internal static class CircuitPropertyBoxHelper
    {
        public static void ChangeSelectedProperty(FormsEventRunner inputHandler, ref int selectedIndex)
        {
            inputHandler.circuitPropertyValueBox.Items.Clear();
            if (inputHandler.circuitPropertyBox.SelectedIndex == -1) { return; }

            var prop = inputHandler.CircuitProperties[inputHandler.circuitPropertyBox.SelectedItem.ToString()];
            var value = inputHandler.CircuitProperties.InvokeGet(prop.Name);

            for (int i = 0; i <= prop.valueRange.max - prop.valueRange.min; i++)
            {
                inputHandler.circuitPropertyValueBox.Items.Add(prop.valueNames[i]);
            }

            //if value is null, -1, else index
            selectedIndex = value == null ? -1 : Array.IndexOf(prop.valueNames, value);
            inputHandler.circuitPropertyValueBox.SelectedIndex = selectedIndex;


            if (!prop.RepresentsInt)
            {
                inputHandler.circuitPropertyTextBox.Visible = true;
                inputHandler.circuitPropertyTextBox.Text = value;
            }
            else
            {
                inputHandler.circuitPropertyTextBox.Visible = false;
            }
            inputHandler.circuitPropertyValueBox.Visible = !inputHandler.circuitPropertyTextBox.Visible;
        }

        public static void ChangeSelectedValue(FormsEventRunner inputHandler, ref int selectedIndex)
        {
            var prop = inputHandler.CircuitProperties[inputHandler.circuitPropertyBox.SelectedItem.ToString()];
            if (prop.RepresentsInt)
            {
                int newIndex = inputHandler.circuitPropertyValueBox.SelectedIndex;
                if (newIndex == selectedIndex) { return; }
                selectedIndex = newIndex;

                inputHandler.CircuitProperties.InvokeSet(prop.Name, prop.valueNames[selectedIndex], inputHandler.stateStack.CurrentState.Connections);
            }
            else
            {
                inputHandler.CircuitProperties.InvokeSet(prop.Name, inputHandler.circuitPropertyTextBox.Text, inputHandler.stateStack.CurrentState.Connections);
            }
            inputHandler.drawingPanel.Refresh();
        }

        public static void ValidateText(FormsEventRunner inputHandler, ref int selectedIndex)
        {
            if (inputHandler.circuitPropertyBox.SelectedItem == null) return;
            var prop = inputHandler.CircuitProperties[inputHandler.circuitPropertyBox.SelectedItem.ToString()];
            inputHandler.CircuitProperties.InvokeSet(prop.Name, inputHandler.circuitPropertyTextBox.Text, inputHandler.stateStack.CurrentState.Connections);
            inputHandler.circuitPropertyTextBox.Text = inputHandler.CircuitProperties.InvokeGet(prop.Name);
        }
    }
}
