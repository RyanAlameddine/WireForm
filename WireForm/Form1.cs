using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm
{

    public partial class Form1 : Form
    {
        WirePainter painter = new WirePainter();
        InputHandler inputHandler = new InputHandler();
        StateStack stateStack = new StateStack();

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolBox.SelectedIndex = 0;
            gateBox.DataSource = Enum.GetValues(GateEnum.GatesEnum);
        }

        #region Input
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool toRefresh = inputHandler.MouseDown(stateStack, (Vec2) e.Location, this, e.Button, GateMenu, ModifierKeys.HasFlag(Keys.Shift), null);

            if (toRefresh)
            {
                SettingsUpdate();
                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            inputHandler.MouseUp(stateStack, e.Button);

            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            bool toRefresh = inputHandler.MouseMove((Vec2) e.Location, stateStack.CurrentState);

            if(toRefresh) Refresh();
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '+' || e.KeyChar == '=')
            {
                GraphicsManager.SizeScale *= 1.1f;
            }
            if (e.KeyChar == '-')
            {
                GraphicsManager.SizeScale *= .9f;
            }
            if (e.KeyChar == 'p')
            {
                Queue<Gate> sources = new Queue<Gate>();
                foreach (Gate gate in stateStack.CurrentState.gates)
                {
                    if (gate.Inputs.Length == 0)
                    {
                        sources.Enqueue(gate);
                    }
                }
                FlowPropagator.Propogate(stateStack.CurrentState, sources);
            }
            Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //If W is pressed, change to Wire tool
            if(e.KeyCode == Keys.W)
            {
                toolBox.SelectedIndex = 0;
                toolBox_SelectedIndexChanged(this, new EventArgs());
            }
            if (e.KeyCode == Keys.G)
            {
                toolBox.SelectedIndex = 1; 
                toolBox_SelectedIndexChanged(this, new EventArgs());
            }

            if (inputHandler.KeyDown(stateStack, e, this))
            {
                Refresh();
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta / 40;
            GraphicsManager.SizeScale += delta;
            if (GraphicsManager.SizeScale > 70)
            {
                GraphicsManager.SizeScale = 70;
            }
            Refresh();
        }

        private void GatePicBox_MouseClick(object sender, MouseEventArgs e)
        {
            inputHandler.MouseDown(stateStack, (Vec2)e.Location, this, e.Button, GateMenu, false, gateBox.SelectedIndex);
            Refresh();
        }
        #endregion Input

        #region Graphics
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            GraphicsManager.Paint(e.Graphics, painter, new Vec2(Width, Height), inputHandler.intersectionBoxes, inputHandler.selections, inputHandler.mouseBox, inputHandler.resetBoxes, stateStack.CurrentState);
        }

        private void GateBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            gatePicBox.Refresh();
        }

        private void GatePicBox_Paint(object sender, PaintEventArgs e)
        {
            Gate newGate = GateEnum.NewGate(gateBox.SelectedIndex, new Vec2(4, 2.5f));
            var temp = GraphicsManager.SizeScale;
            newGate.Draw(new Painter(e.Graphics, 15));
        }
        #endregion Graphics

        #region CircuitProperties
        HashSet<CircuitObject> oldSelections;
        List<CircuitProp> circuitProperties;
        private void SettingsUpdate()
        {
            if (!inputHandler.selections.Equals(oldSelections))
            {
                oldSelections = new HashSet<CircuitObject>(inputHandler.selections);
                SelectionSettings.Items.Clear();
                SelectionSettingValue.Items.Clear();
                
                foreach(CircuitObject obj in oldSelections)
                {
                    circuitProperties = CircuitPropertyAttribute.GetProperties(obj, stateStack, this);
                    foreach (var property in circuitProperties)
                    {
                        SelectionSettings.Items.Add(property.Name);
                    }
                }
            }
        }

        int prevSelectedIndex = 0;
        private void SelectionSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionSettingValue.Items.Clear();
            if (SelectionSettings.SelectedIndex == -1) { return; }

            var prop = circuitProperties[SelectionSettings.SelectedIndex];
            var value = prop.Get();

            for (int i = 0; i <= prop.valueRange.max - prop.valueRange.min; i++)
            {
                SelectionSettingValue.Items.Add(prop.valueNames[i]);
            }
            prevSelectedIndex = value;
            SelectionSettingValue.SelectedIndex = value - prop.valueRange.min;
        }

        private void SelectionSettingsValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            var prop = circuitProperties[SelectionSettings.SelectedIndex];

            int newVal = SelectionSettingValue.SelectedIndex + prop.valueRange.min;
            if (newVal == prevSelectedIndex) { return; }
            prevSelectedIndex = newVal;
            prop.Set(newVal, stateStack.CurrentState.Connections);
            stateStack.RegisterChange($"Changed {SelectionSettings.SelectedItem} to {newVal}");
            Refresh();
        }
        #endregion CircuitProperties

        #region FormInput
        private void toolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputHandler.tool = (Tool) toolBox.SelectedIndex;
            gateBox.Visible                = inputHandler.tool == Tool.GateController;
            SelectionSettings.Visible      = inputHandler.tool == Tool.GateController;
            SelectionSettingValue.Visible  = inputHandler.tool == Tool.GateController;
            gatePicBox.Visible             = inputHandler.tool == Tool.GateController;

            inputHandler.selections.Clear();
            Refresh();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            stateStack.Clear();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            stateStack.Load(openFileDialog);
        }

        private void save_Click(object sender, EventArgs e)
        {
            stateStack.Save(saveFileDialog);
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            stateStack.SaveAs(saveFileDialog);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            inputHandler.Undo(stateStack);
            Refresh();
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            inputHandler.Redo(stateStack);
            Refresh();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            inputHandler.Copy();
            Refresh(); 
        }

        private void cutButton_Click(object sender, EventArgs e)
        {
            inputHandler.Cut(stateStack);
            Refresh();
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            inputHandler.Paste();
            Refresh();
        }


        #endregion
    }
}
