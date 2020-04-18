using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates;
using WireForm.Circuitry.Utilities;
using WireForm.GraphicsUtils;
using WireForm.Input;
using WireForm.Input.States.Selection;
using WireForm.Input.States.Wire;
using WireForm.MathUtils;

namespace WireForm
{

    public partial class Form1 : Form
    {
        readonly BoardStack stateStack = new BoardStack();
        readonly InputStateManager stateManager = new InputStateManager();

        //public static int value = 0;
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, drawingPanel, new object[] { true });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolBox.SelectedIndex = 0;
            gateBox.DataSource = Enum.GetValues(GateEnum.GatesEnum);
        }

        #region Input

        bool runInputEvent(Func<StateControls, bool> inputEvent)
        {
            return runInputEvent(inputEvent, Keys.None, ModifierKeys);
        }
        bool runInputEvent(Func<StateControls, bool> inputEvent, Keys key, Keys modifiers)
        {
            var mousePoint = (Vec2)drawingPanel.PointToClient(Cursor.Position);
            var stateControls = new StateControls(stateStack.CurrentState, mousePoint, key, modifiers, drawingPanel.Refresh, stateStack.RegisterChange, stateStack.Reverse, stateStack.Advance);
            bool toRefresh = inputEvent(stateControls);

            //Process [CircuitActions]
            if (stateControls.CircuitActionsOutput != null)
            {
                GateMenu.Items.Clear();
                for (int i = 0; i < stateControls.CircuitActionsOutput.Count; i++)
                {
                    var act = stateControls.CircuitActionsOutput[i];
                    EventHandler actionEvent = (s, e) =>
                    {
                        act.Invoke(stateControls.State);
                        stateControls.RegisterChange($"Executed action {act.Name} on selection");
                        drawingPanel.Refresh();
                    };
                    GateMenu.Items.Add(act.Name, null, actionEvent);
                }

                GateMenu.Show(this, (Point)mousePoint);
            }

            //Process [CircuitProperties]
            if (stateControls.CircuitPropertiesOutput != null)
            {
                SelectionSettings.Items.Clear();
                SelectionSettingValue.Items.Clear();
                circuitProperties = stateControls.CircuitPropertiesOutput;

                foreach (var property in circuitProperties)
                {
                    SelectionSettings.Items.Add(property.Name);
                }
            }

            if (toRefresh) drawingPanel.Refresh();

            return toRefresh;
        }

        private void drawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left ) runInputEvent(stateManager.MouseLeftDown);
            else if (e.Button == MouseButtons.Right) runInputEvent(stateManager.MouseRightDown);
        }

        private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left ) runInputEvent(stateManager.MouseLeftUp);
            else if (e.Button == MouseButtons.Right) runInputEvent(stateManager.MouseRightUp);
        }

        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            runInputEvent(stateManager.MouseMove);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If W is pressed, change to Wire tool
            if (e.KeyChar == 'w')
            {
                toolBox.SelectedIndex = 0;
                toolBox_SelectedIndexChanged(this, new EventArgs());
            }
            if (e.KeyChar == 'g')
            {
                toolBox.SelectedIndex = 1;
                toolBox_SelectedIndexChanged(this, new EventArgs());
            }

            if (e.KeyChar == '+' || e.KeyChar == '=')
            {
                GraphicsManager.SizeScale *= 1.1f;
                drawingPanel.Refresh();
            }
            if (e.KeyChar == '-')
            {
                GraphicsManager.SizeScale *= .9f;
                drawingPanel.Refresh();
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
                drawingPanel.Refresh();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            runInputEvent(stateManager.KeyDown, e.KeyCode, e.Modifiers);
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
            drawingPanel.Refresh();
        }

        private void GatePicBox_MouseClick(object sender, MouseEventArgs e)
        {
            //MakeControls(out var stateControls);
            //inputManager.MouseDown(stateStack, (Vec2)e.Location, this, e.Button, GateMenu, false, gateBox.SelectedIndex);
            drawingPanel_MouseDown(sender, e);
        }
        #endregion Input

        #region Graphics
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GraphicsManager.Paint(e.Graphics, new Vec2(Width, Height), stateStack.CurrentState, stateManager);
        }

        private void GateBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            newGate = GateEnum.NewGate(gateBox.SelectedIndex, new Vec2(4, 2.5f));
            gatePicBox.Refresh();
        }

        Gate newGate = new BitSource(new Vec2(4, 2.5f), Direction.Right);
        private void GatePicBox_Paint(object sender, PaintEventArgs e)
        {
            newGate.Draw(new PainterScope(e.Graphics, 15));
        }
        #endregion Graphics

        #region CircuitProperties
        List<CircuitProp> circuitProperties;

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
            drawingPanel.Refresh();
            //Refresh();
        }
        #endregion CircuitProperties

        #region FormInput
        enum Tool
        {
            WirePainter,
            GateController,
        }
        Tool tool = Tool.GateController;
        private void toolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newTool = (Tool)toolBox.SelectedIndex;
            if (newTool == tool) return;
            tool = (Tool)toolBox.SelectedIndex;
            gateBox.Visible = tool == Tool.GateController;
            SelectionSettings.Visible = tool == Tool.GateController;
            SelectionSettingValue.Visible = tool == Tool.GateController;
            gatePicBox.Visible = tool == Tool.GateController;

            switch (tool)
            {

                case Tool.WirePainter:
                    stateManager.ChangeTool(new WireToolState());
                    break;
                case Tool.GateController:
                    stateManager.ChangeTool(new SelectionToolState());
                    break;
            }

            drawingPanel.Refresh();
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
            runInputEvent(stateManager.Undo);
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            runInputEvent(stateManager.Redo);
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            runInputEvent(stateManager.Copy);
        }

        private void cutButton_Click(object sender, EventArgs e)
        {
            runInputEvent(stateManager.Cut);
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            runInputEvent(stateManager.Paste);
        }
        #endregion
    }
}
