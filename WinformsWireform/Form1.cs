using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Wireform;
using Wireform.Circuitry;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.CircuitAttributes.Utils;
using Wireform.Circuitry.Gates;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.Input;
using Wireform.MathUtils;

namespace WinformsWireform
{

    public partial class Form1 : Form
    {
        readonly BoardStack stateStack;
        readonly InputStateManager stateManager;

        //public static int value = 0;
        public Form1()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, drawingPanel, new object[] { true });

            stateStack = new BoardStack(new LocalSaveable(openFileDialog, saveFileDialog));
            stateManager = new InputStateManager();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolBox.SelectedIndex = 0;
            gateBox.DataSource = GateCollection.GateConstructors.Keys.ToArray();
        }

        #region Input

        void RunInputEvent(Func<StateControls, bool> inputEvent)
        {
            RunInputEvent(inputEvent, Keys.None);
        }
        /// <summary>
        /// Function for the Form to interact with the input manager.
        /// The inputEvent should be a function in the InputStateManager.
        /// </summary>
        void RunInputEvent(Func<StateControls, bool> inputEvent, Keys key)
        {
            var stateControls = MakeControls(key);
            bool toRefresh = inputEvent(stateControls);

            //Process [CircuitActions]
            if (stateControls.CircuitActionsOutput != null)
            {
                GateMenu.Items.Clear();
                foreach(var action in stateControls.CircuitActionsOutput)
                {
                    //Creates a dropdown menu item which, when clicked, will invoke the action and refresh the drawing panel
                    void actionEvent(object s, EventArgs e) => stateControls.CircuitActionsOutput.InvokeActionAndRefresh(stateControls.State, drawingPanel.Refresh, action);
                    var item = new ToolStripMenuItem(action.Name, null, actionEvent)
                    {
                        ShortcutKeyDisplayString = action.Hotkey.GetHotkeyString(action.Modifiers),
                        ShowShortcutKeys = true
                    };
                    GateMenu.Items.Add(item);
                }

                GateMenu.Show(this, drawingPanel.PointToClient(Cursor.Position));
            }

            //Process [CircuitProperties]
            if (stateControls.CircuitPropertiesOutput != null)
            {
                SelectionSettings.Items.Clear();
                SelectionSettingValue.Items.Clear();
                circuitProperties = stateControls.CircuitPropertiesOutput;

                foreach (var property in circuitProperties)
                {
                    SelectionSettings.Items.Add(property.Key);
                }
            }

            if (toRefresh) drawingPanel.Refresh();
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
            var stateControls = new StateControls(stateStack.CurrentState, mousePoint, key.ToString().ToLower()[0], modifierKeys, stateStack.RegisterChange, stateStack.Reverse, stateStack.Advance);
            return stateControls;
        }

        private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left ) RunInputEvent(stateManager.MouseLeftDown);
            else if (e.Button == MouseButtons.Right) RunInputEvent(stateManager.MouseRightDown);
        }

        private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left ) RunInputEvent(stateManager.MouseLeftUp);
            else if (e.Button == MouseButtons.Right) RunInputEvent(stateManager.MouseRightUp);
        }

        private void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            RunInputEvent(stateManager.MouseMove);
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
                ToolBox_SelectedIndexChanged(this, new EventArgs());
            }
            else if (keyData == Keys.G)
            {
                toolBox.SelectedIndex = 0;
                ToolBox_SelectedIndexChanged(this, new EventArgs());
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            RunInputEvent(stateManager.KeyDown, e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            RunInputEvent(stateManager.KeyUp, e.KeyCode);
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
            RunInputEvent(stateManager.PlaceNewGate(newGate));
        }
        #endregion Input

        #region Graphics
        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GraphicsManager.Paint(new PainterScope(new WinformsPainter(e.Graphics, GraphicsManager.SizeScale), GraphicsManager.SizeScale), new Vec2(Width, Height), stateStack.CurrentState, stateManager);
        }

        Gate picBoxGate = new BitSource(new Vec2(4, 2.5f), Direction.Right);
        private void GatePicBox_Paint(object sender, PaintEventArgs e)
        {
            picBoxGate.DrawGate(new PainterScope(new WinformsPainter(e.Graphics, 15), 15));
        }
        #endregion Graphics

        #region CircuitProperties
        CircuitPropertyCollection circuitProperties;

        int? prevSelectedIndex = 0;
        private void SelectionSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionSettingValue.Items.Clear();
            if (SelectionSettings.SelectedIndex == -1) { return; }

            var prop = circuitProperties[SelectionSettings.SelectedItem.ToString()];
            var value = circuitProperties.InvokeGet(prop.Name);
            prevSelectedIndex = value;

            for (int i = 0; i <= prop.valueRange.max - prop.valueRange.min; i++)
            {
                SelectionSettingValue.Items.Add(prop.valueNames[i]);
            }

            //if value is null, -1, else index
            SelectionSettingValue.SelectedIndex = value == null ? -1 : (int)value - prop.valueRange.min;
        }

        private void SelectionSettingsValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            var prop = circuitProperties[SelectionSettings.SelectedItem.ToString()];

            int newVal = SelectionSettingValue.SelectedIndex + prop.valueRange.min;
            if (newVal == prevSelectedIndex) { return; }
            prevSelectedIndex = newVal;
            circuitProperties.InvokeSet(prop.Name, newVal, stateStack.CurrentState.Connections);
            drawingPanel.Refresh();
            //Refresh();
        }
        #endregion CircuitProperties

        #region FormInput
        Tools tool = Tools.SelectionTool;
        private void ToolBox_SelectedIndexChanged(object sender, EventArgs e)
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

        private void NewButton_Click(object sender, EventArgs e)
        {
            stateStack.Clear();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            stateStack.Load();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            stateStack.Save();
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            stateStack.SaveAs();
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            RunInputEvent(stateManager.Undo);
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            RunInputEvent(stateManager.Redo);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            RunInputEvent(stateManager.Copy);
        }

        private void CutButton_Click(object sender, EventArgs e)
        {
            RunInputEvent(stateManager.Cut);
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            RunInputEvent(stateManager.Paste);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}
