using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WinformsWireform.Helpers;
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

        /// <summary>
        /// Helper class for WinformsWireform 
        /// Handles all the processing of input and output data to the stateManager
        /// </summary>
        readonly WinformsInputHandler inputHandler;

        public Form1()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, DrawingPanel, new object[] { true });
            
            stateStack = new BoardStack(new LocalSaveable(openFileDialog, saveFileDialog));
            stateManager = new InputStateManager();

            inputHandler = new WinformsInputHandler(stateManager, stateStack, GateMenu, CircuitPropertyBox, CircuitPropertyValueBox, DrawingPanel, () => ModifierKeys);

            toolBox.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Create the Create menu
            MenuHelper.CreateGateMenuFromRoot(createToolStripMenuItem, inputHandler);
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
            GraphicsManager.SizeScale += delta;
            if (GraphicsManager.SizeScale > 70)
            {
                GraphicsManager.SizeScale = 70;
            }
            DrawingPanel.Refresh();
        }
        #endregion Input

        #region Graphics
        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GraphicsManager.Paint(new PainterScope(new WinformsPainter(e.Graphics, GraphicsManager.SizeScale), GraphicsManager.SizeScale), new Vec2(Width, Height), stateStack.CurrentState, stateManager);
        }
        #endregion Graphics

        #region FormInput
        Tools tool = Tools.SelectionTool;
        private void ToolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!stateManager.TryChangeTool(tool))
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
            if      (e.Button == MouseButtons.Left)  inputHandler.RunInputEvent(stateManager.MouseLeftDown);
            else if (e.Button == MouseButtons.Right) inputHandler.RunInputEvent(stateManager.MouseRightDown);
        }

        private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if      (e.Button == MouseButtons.Left)  inputHandler.RunInputEvent(stateManager.MouseLeftUp);
            else if (e.Button == MouseButtons.Right) inputHandler.RunInputEvent(stateManager.MouseRightUp);
        }

        private void Form1_KeyPress (object sender, KeyPressEventArgs e) => inputHandler.RunInputEvent(stateManager.KeyDown, e.KeyChar);
        private void Panel_MouseMove(object sender, MouseEventArgs    e) => inputHandler.RunInputEvent(stateManager.MouseMove);

        private void UndoButton_Click (object sender, EventArgs e) => inputHandler.RunInputEvent(stateManager.Undo );
        private void RedoButton_Click (object sender, EventArgs e) => inputHandler.RunInputEvent(stateManager.Redo );
        private void CopyButton_Click (object sender, EventArgs e) => inputHandler.RunInputEvent(stateManager.Copy );
        private void CutButton_Click  (object sender, EventArgs e) => inputHandler.RunInputEvent(stateManager.Cut  );
        private void PasteButton_Click(object sender, EventArgs e) => inputHandler.RunInputEvent(stateManager.Paste);

        private void OpenButton_Click(object sender, EventArgs e) => stateStack.Load();
        private void Save_Click      (object sender, EventArgs e) => stateStack.Save();
        private void SaveAs_Click    (object sender, EventArgs e) => stateStack.SaveAs();

        private void CloseButton_Click(object sender, EventArgs e) => Close();
        #endregion

        #region CircuitProperties

        int? previousValue = 0;
        private void CircuitPropertyBox_SelectedIndexChanged(object sender, EventArgs e)
            => CircuitPropertyBoxHelper.ChangeSelectedProperty(inputHandler, ref previousValue);

        private void CircuitPropertyValueBox_SelectedIndexChanged(object sender, EventArgs e)
            => CircuitPropertyBoxHelper.ChangeSelectedValue(inputHandler, ref previousValue);
        #endregion CircuitProperties
    }
}
