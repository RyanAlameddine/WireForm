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
using WireForm.MathUtils;

namespace WireForm
{

    public partial class Form1 : Form
    {
        readonly StateStack stateStack = new StateStack();
        readonly InputManager inputManager = new InputManager();

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

        bool runInputEvent(Func<InputControls, bool> inputEvent)
        {
            return runInputEvent(inputEvent, Keys.None, ModifierKeys);
        }
        bool runInputEvent(Func<InputControls, bool> inputEvent, Keys key, Keys modifiers)
        {
            var mousePoint = (Vec2)drawingPanel.PointToClient(Cursor.Position);
            var inputControls = new InputControls(stateStack.CurrentState, mousePoint, key, modifiers, drawingPanel.Refresh, stateStack.RegisterChange, stateStack.Reverse, stateStack.Advance);
            bool toRefresh = inputEvent(inputControls);

            //Process [CircuitActions]
            if (inputControls.CircuitActionsOutput != null)
            {
                GateMenu.Items.Clear();
                for (int i = 0; i < inputControls.CircuitActionsOutput.Count; i++)
                {
                    var act = inputControls.CircuitActionsOutput[i];
                    EventHandler actionEvent = (s, e) =>
                    {
                        act.Invoke(inputControls.State);
                        inputControls.RegisterChange($"Executed action {act.Name} on selection");
                        drawingPanel.Refresh();
                    };
                    GateMenu.Items.Add(act.Name, null, actionEvent);
                }

                GateMenu.Show(this, (Point)mousePoint);
            }

            //Process [CircuitProperties]
            if (inputControls.CircuitPropertiesOutput != null)
            {
                SelectionSettings.Items.Clear();
                SelectionSettingValue.Items.Clear();
                circuitProperties = inputControls.CircuitPropertiesOutput;
                //circuitProperties.AddRange(CircuitPropertyAttribute.GetProperties(obj, stateStack, this
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
            //bool toRefresh = inputHandler.MouseDown(stateStack, (Vec2)e.Location, this, e.Button, GateMenu, ModifierKeys.HasFlag(Keys.Shift), null);

            //bool toRefresh = false;

            if      (e.Button == MouseButtons.Left ) runInputEvent(inputManager.MouseLeftDown);
            else if (e.Button == MouseButtons.Right) runInputEvent(inputManager.MouseRightDown);

            //if (toRefresh)
            //{
            //    //SettingsUpdate();
            //    drawingPanel.Refresh();
            //}
        }

        private void drawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left ) runInputEvent(inputManager.MouseLeftUp);
            else if (e.Button == MouseButtons.Right) runInputEvent(inputManager.MouseRightUp);
        }

        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            runInputEvent(inputManager.MouseMove);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
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

            runInputEvent(inputManager.KeyDown, e.KeyCode, e.Modifiers);
            //MakeControls(out var inputControls);

            //if (inputManager.KeyDown(inputControls, e.KeyCode, e.Modifiers))
            //{
            //    drawingPanel.Refresh();
            //}
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
            //MakeControls(out var inputControls);
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
            GraphicsManager.Paint(e.Graphics, new Vec2(Width, Height), stateStack.CurrentState, inputManager);
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
        private void toolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //inputHandler.tool = (Tool) toolBox.SelectedIndex;
            //gateBox.Visible                = inputHandler.tool == Tool.GateController;
            //SelectionSettings.Visible      = inputHandler.tool == Tool.GateController;
            //SelectionSettingValue.Visible  = inputHandler.tool == Tool.GateController;
            //gatePicBox.Visible             = inputHandler.tool == Tool.GateController;

            //inputHandler.selections.Clear();
            //drawingPanel.Refresh();
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
            runInputEvent(inputManager.Undo);
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            runInputEvent(inputManager.Redo);
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            runInputEvent(inputManager.Copy);
        }

        private void cutButton_Click(object sender, EventArgs e)
        {
            runInputEvent(inputManager.Cut);
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            runInputEvent(inputManager.Paste);
        }
        #endregion
    }
}
