using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformsWireform.Helpers
{
    internal static class CircuitPropertyBoxHelper
    {
        public static void ChangeSelectedProperty(FormsEventRunner inputHandler, ref int? previousValue)
        {
            inputHandler.circuitPropertyValueBox.Items.Clear();
            if (inputHandler.circuitPropertyBox.SelectedIndex == -1) { return; }

            var prop = inputHandler.CircuitProperties[inputHandler.circuitPropertyBox.SelectedItem.ToString()];
            var value = inputHandler.CircuitProperties.InvokeGet(prop.Name);
            previousValue = value;

            for (int i = 0; i <= prop.valueRange.max - prop.valueRange.min; i++)
            {
                inputHandler.circuitPropertyValueBox.Items.Add(prop.valueNames[i]);
            }

            //if value is null, -1, else index
            inputHandler.circuitPropertyValueBox.SelectedIndex = value == null ? -1 : (int)value - prop.valueRange.min;
        }

        public static void ChangeSelectedValue(FormsEventRunner inputHandler, ref int? previousValue)
        {
            var prop = inputHandler.CircuitProperties[inputHandler.circuitPropertyBox.SelectedItem.ToString()];

            int newVal = inputHandler.circuitPropertyValueBox.SelectedIndex + prop.valueRange.min;
            if (newVal == previousValue) { return; }
            previousValue = newVal;
            inputHandler.CircuitProperties.InvokeSet(prop.Name, newVal, inputHandler.stateStack.CurrentState.Connections);
            inputHandler.drawingPanel.Refresh();
            //Refresh();
        }
    }
}
