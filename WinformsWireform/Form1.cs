using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Wireform;
using Wireform.Circuitry;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Gates;
using Wireform.Circuitry.Utilities;
using Wireform.GraphicsUtils;
using Wireform.Input;
using Wireform.MathUtils;

namespace WinformsWireform
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
            gateBox.DataSource = GateCollection.GateConstructors.Keys.ToArray();
        }

        #region Input

        bool runInputEvent(Func<StateControls, bool> inputEvent)
        {
            return runInputEvent(inputEvent, Keys.None);
        }
        /// <summary>
        /// Function for the Form to interact with the input manager.
        /// The inputEvent should be a function in the InputStateManager.
        /// </summary>
        bool runInputEvent(Func<StateControls, bool> inputEvent, Keys key)
        {
            var stateControls = MakeControls(key);
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

                GateMenu.Show(this, (Point)drawingPanel.PointToClient(Cursor.Position));
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

        /// <summary>
        /// Helper function which makes StateControls
        /// </summary>
        StateControls MakeControls(Keys key)
        {
            var mousePoint = (Vec2)drawingPanel.PointToClient(Cursor.Position);
            Modifier modifierKeys = Modifier.None;
            if (ModifierKeys.HasFlag(Keys.Control)) modifierKeys |= Modifier.Control;
            if (ModifierKeys.HasFlag(Keys.Shift  )) modifierKeys |= Modifier.Shift  ;
            if (ModifierKeys.HasFlag(Keys.Alt    )) modifierKeys |= Modifier.Alt    ;
            var stateControls = new StateControls(stateStack.CurrentState, mousePoint, key.ToString().ToLower()[0], modifierKeys, drawingPanel.Refresh, stateStack.RegisterChange, stateStack.Reverse, stateStack.Advance);
            return stateControls;
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //If W is pressed, change to Wire tool
            if (keyData == Keys.W)
            {
                toolBox.SelectedIndex = 1;
                toolBox_SelectedIndexChanged(this, new EventArgs());
            }
            else if (keyData == Keys.G)
            {
                toolBox.SelectedIndex = 0;
                toolBox_SelectedIndexChanged(this, new EventArgs());
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            runInputEvent(stateManager.KeyDown, e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            runInputEvent(stateManager.KeyUp, e.KeyCode);
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

        private void GateBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Create new gate
            picBoxGate = GateCollection.CreateGate((string)gateBox.SelectedValue, new Vec2(4, 2.5f));
            gatePicBox.Refresh();
        }

        private void GatePicBox_MouseDown(object sender, MouseEventArgs e)
        {
            //Place created gate onto board
            StateControls stateControls = MakeControls(Keys.None);
            Gate newGate = GateCollection.CreateGate((string)gateBox.SelectedValue, Vec2.Zero);
            stateManager.PlaceNewGate(stateControls, newGate);
        }
        #endregion Input

        #region Graphics
        private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GraphicsManager.Paint(new PainterScope(new WinformsPainter(e.Graphics, GraphicsManager.SizeScale), GraphicsManager.SizeScale), new Vec2(Width, Height), stateStack.CurrentState, stateManager);
        }

        Gate picBoxGate = new BitSource(new Vec2(4, 2.5f), Direction.Right);
        private void GatePicBox_Paint(object sender, PaintEventArgs e)
        {
            picBoxGate.Draw(new PainterScope(new WinformsPainter(e.Graphics, 15), 15));
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
        Tools tool = Tools.SelectionTool;
        private void toolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newTool = (Tools)toolBox.SelectedIndex;
            if (newTool == tool) return;

            tool                          = (Tools)toolBox.SelectedIndex;
            gateBox.Visible               = tool == Tools.SelectionTool;
            SelectionSettings.Visible     = tool == Tools.SelectionTool;
            SelectionSettingValue.Visible = tool == Tools.SelectionTool;
            gatePicBox.Visible            = tool == Tools.SelectionTool;

            stateManager.ChangeTool(tool);

            drawingPanel.Refresh();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            stateStack.Clear();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Json|*.json";
            openFileDialog.Title = "Load your wireForm";
            openFileDialog.ShowDialog();

            var fileName = openFileDialog.FileName;
            stateStack.Load(fileName);
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (!stateStack.Save())
            {
                saveFileDialog.Filter = "Json|*.json";
                saveFileDialog.Title = "Save your wireForm";
                saveFileDialog.ShowDialog();
                var fileName = saveFileDialog.FileName;
                if (fileName == "")
                {
                    return;
                }
                stateStack.SaveAs(fileName);
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Json|*.json";
            saveFileDialog.Title = "Save your wireForm";
            saveFileDialog.ShowDialog();
            var fileName = saveFileDialog.FileName;
            if (fileName == "")
            {
                return;
            }
            stateStack.SaveAs(fileName);
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

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}
