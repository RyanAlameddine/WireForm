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
    public partial class WireformForm : Form
    {
        readonly BoardStack stateStack;
        readonly InputStateManager inputStateManager;
        public WireformForm()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, DrawingPanel, new object[] { true });
            
            stateStack = new BoardStack(new LocalSaveable(openFileDialog, saveFileDialog));

            var eventRunner = new FormsEventRunner(stateStack, GateMenu, CircuitPropertyBox, CircuitPropertyValueBox, DrawingPanel, () => ModifierKeys, () => GetKey);
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
            //If W is pressed, change to Wire tool
            if (keyData == Keys.W)
            {
                toolBox.SelectedIndex = 1;
                ToolBox_SelectedIndexChanged(this, new EventArgs());
            }
            //If G is pressed, change to selection tool
            else if (keyData == Keys.G)
            {
                toolBox.SelectedIndex = 0;
                ToolBox_SelectedIndexChanged(this, new EventArgs());
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }



        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta / 40;
            inputStateManager.SizeScale += delta;
            if (inputStateManager.SizeScale > 70)
            {
                inputStateManager.SizeScale = 70;
            }
            DrawingPanel.Refresh();
        }
        #endregion Input

        #region Graphics
        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            PainterScope painter = new PainterScope(new WinformsPainter(e.Graphics), inputStateManager.SizeScale);
            inputStateManager.Draw(stateStack.CurrentState, painter, new Vec2(Width, Height));
        }
        #endregion Graphics

        #region FormInput
        Tools tool = Tools.SelectionTool;
        private void ToolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!inputStateManager.TryChangeTool(tool))
            {
                toolBox.SelectedIndex = (int)tool;
                return;
            }
            tool                            = (Tools)toolBox.SelectedIndex;
            createToolStripMenuItem.Visible = tool == Tools.SelectionTool;
            CircuitPropertyBox.Visible      = tool == Tools.SelectionTool;
            CircuitPropertyValueBox.Visible = tool == Tools.SelectionTool;
            DrawingPanel.Refresh();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            stateStack.Clear();
            Refresh();
        }

        private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left)  inputStateManager.MouseLeftDown ();
            else if (e.Button == MouseButtons.Right) inputStateManager.MouseRightDown();
        }

        private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left)  inputStateManager.MouseLeftUp ();
            else if (e.Button == MouseButtons.Right) inputStateManager.MouseRightUp();
        }

        private char? GetKey { get; set; }
        private void Form1_KeyPress (object sender, KeyPressEventArgs e) { GetKey = e.KeyChar; inputStateManager.KeyDown(); }
        
        private void Panel_MouseMove(object sender, MouseEventArgs e   ) => inputStateManager.MouseMove();

        private void UndoButton_Click (object sender, EventArgs e) => inputStateManager.Undo ();
        private void RedoButton_Click (object sender, EventArgs e) => inputStateManager.Redo ();
        private void CopyButton_Click (object sender, EventArgs e) => inputStateManager.Copy ();
        private void CutButton_Click  (object sender, EventArgs e) => inputStateManager.Cut  ();
        private void PasteButton_Click(object sender, EventArgs e) => inputStateManager.Paste();

        private void OpenButton_Click(object sender, EventArgs e) => stateStack.Load();
        private void Save_Click      (object sender, EventArgs e) => stateStack.Save();
        private void SaveAs_Click    (object sender, EventArgs e) => stateStack.SaveAs();

        private void CloseButton_Click(object sender, EventArgs e) => Close();
        #endregion

        #region CircuitProperties

        int? previousValue = 0;
        private void CircuitPropertyBox_SelectedIndexChanged(object sender, EventArgs e)
            => CircuitPropertyBoxHelper.ChangeSelectedProperty((FormsEventRunner) inputStateManager.eventRunner, ref previousValue);

        private void CircuitPropertyValueBox_SelectedIndexChanged(object sender, EventArgs e)
            => CircuitPropertyBoxHelper.ChangeSelectedValue((FormsEventRunner) inputStateManager.eventRunner, ref previousValue);
        #endregion CircuitProperties
    }
}
