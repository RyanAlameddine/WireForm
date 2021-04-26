using System;
using System.Reflection;
using System.Windows.Forms;
using WinformsWireform.Helpers;
using Wireform;
using Wireform.Circuitry;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using WireformInput;

namespace WinformsWireform
{
    internal partial class WireformForm : Form
    {
        readonly BoardStack stateStack;
        readonly InputStateManager inputStateManager;
        public WireformForm()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, DrawingPanel, new object[] { true });

            stateStack = new BoardStack(new LocalSaver(openFileDialog, saveFileDialog));

            var eventRunner = new FormsEventRunner(stateStack, GateMenu, CircuitPropertyBox, CircuitPropertyValueBox, CircuitPropertyValueTextBox, DrawingPanel, () => ModifierKeys, () => GetKey);
            inputStateManager = new InputStateManager(eventRunner);
            eventRunner.stateManager = inputStateManager;

            toolBox.SelectedIndex = 0;
            //FlowPropagator.DebugStep = DrawingPanel.Refresh;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Create the Create menu
            MenuHelper.CreateGateMenuFromRoot(createToolStripMenuItem, inputStateManager);
        }

        #region Input
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!CircuitPropertyValueTextBox.Focused)
            {
                //If W is pressed, change to Wire tool
                if (keyData.HasFlag(Keys.Control))
                {
                    if (keyData == (Keys.W | Keys.Control))
                    {
                        toolBox.SelectedIndex = 1;
                        ToolBox_SelectedIndexChanged(this, new EventArgs());
                    }
                    //If G is pressed, change to selection tool
                    else if (keyData == (Keys.G | Keys.Control))
                    {
                        toolBox.SelectedIndex = 0;
                        ToolBox_SelectedIndexChanged(this, new EventArgs());
                    }
                    //If E is pressed, change to text tool
                    else if (keyData == (Keys.E | Keys.Control))
                    {
                        toolBox.SelectedIndex = 2;
                        ToolBox_SelectedIndexChanged(this, new EventArgs());
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta / 40;
            inputStateManager.Zoom += delta;
            if (inputStateManager.Zoom > 70)
            {
                inputStateManager.Zoom = 70;
            }
            DrawingPanel.Refresh();
        }
        #endregion Input

        #region Graphics
        private async void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            PainterScope painter = new PainterScope(new WinformsPainter(e.Graphics), inputStateManager.Zoom);
            await inputStateManager.DrawGrid(painter, new Vec2(Width, Height));
            await inputStateManager.Draw(stateStack.CurrentState, painter);
        }
        #endregion Graphics

        #region FormInput
        private void ToolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!inputStateManager.TryChangeTool((Tools)toolBox.SelectedIndex))
            {
                toolBox.SelectedIndex = (int)inputStateManager.SelectedTool;
                return;
            }

            DrawingPanel.Refresh();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            stateStack.Clear();
            Refresh();
        }

        private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            DrawingPanel.Focus();
            if (e.Button == MouseButtons.Left) inputStateManager.MouseLeftDown();
            else if (e.Button == MouseButtons.Right) inputStateManager.MouseRightDown();
        }

        private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) inputStateManager.MouseLeftUp();
            else if (e.Button == MouseButtons.Right) inputStateManager.MouseRightUp();
        }

        private char? GetKey { get; set; }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CircuitPropertyValueTextBox.Focused) { GetKey = e.KeyChar; inputStateManager.KeyDown(); }
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e) => inputStateManager.MouseMove();

        private void UndoButton_Click (object sender, EventArgs e) => inputStateManager.Undo();
        private void RedoButton_Click (object sender, EventArgs e) => inputStateManager.Redo();
        private void CopyButton_Click (object sender, EventArgs e) => inputStateManager.Copy();
        private void CutButton_Click  (object sender, EventArgs e) => inputStateManager.Cut();
        private void PasteButton_Click(object sender, EventArgs e) => inputStateManager.Paste();

        private void OpenButton_Click(object sender, EventArgs e) => stateStack.Load();
        private void Save_Click      (object sender, EventArgs e) => stateStack.Save();
        private void SaveAs_Click    (object sender, EventArgs e) => stateStack.SaveAs();

        private void CloseButton_Click(object sender, EventArgs e) => Close();
        #endregion

        #region CircuitProperties

        int selectedIndex = 0;
        private void CircuitPropertyBox_SelectedIndexChanged(object sender, EventArgs e)
            => CircuitPropertyBoxHelper.ChangeSelectedProperty((FormsEventRunner)inputStateManager.eventRunner, ref selectedIndex);

        private void CircuitPropertyValueBox_SelectedIndexChanged(object sender, EventArgs e)
            => CircuitPropertyBoxHelper.ChangeSelectedValue((FormsEventRunner)inputStateManager.eventRunner, ref selectedIndex);

        private void CircuitPropertyValueTextBox_TextChanged(object sender, EventArgs e)
            => CircuitPropertyBoxHelper.ChangeSelectedValue((FormsEventRunner)inputStateManager.eventRunner, ref selectedIndex);

        private void CircuitPropertyValueTextBox_Validated(object sender, EventArgs e)
           => CircuitPropertyBoxHelper.ValidateText((FormsEventRunner)inputStateManager.eventRunner, ref selectedIndex);

        private void CircuitPropertyValueTextBox_KeyDown(object sender, KeyEventArgs e)
            { if (e.KeyCode == Keys.Enter) { CircuitPropertyValueTextBox_Validated(sender, e); e.SuppressKeyPress = true; } }

        #endregion CircuitProperties
    }
}
