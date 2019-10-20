using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.Gates;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm
{
    public partial class Form1 : Form
    {
        Painter painter = new Painter();
        InputHandler inputHandler = new InputHandler();
        BoardState state = new BoardState();

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
            gateBox.DataSource = Enum.GetValues(typeof(Gates));
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool toRefresh = inputHandler.MouseDown(state, (Vec2) e.Location, e.Button, ModifierKeys.HasFlag(Keys.Shift), null);

            if (toRefresh) Refresh();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            inputHandler.MouseUp(state);

            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            bool toRefresh = inputHandler.MouseMove((Vec2) e.Location, state);

            if(toRefresh) Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            GraphicsManager.PropogateAndPaint(e.Graphics, painter, new Vec2(Width, Height), inputHandler.intersectionBoxes, inputHandler.selections, inputHandler.mouseBox, inputHandler.resetBoxes, state);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 's')
            {
                SaveManager.Save(Path.Combine(Directory.GetCurrentDirectory(), "lines.json"), state);
            }
            if(e.KeyChar == 'l')
            {
                SaveManager.Load(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "lines.json")), out var prop);
                state = prop;
            }
            if(e.KeyChar == '+' || e.KeyChar == '=')
            {
                GraphicsManager.SizeScale *= 1.1f;
            }
            if(e.KeyChar == '-')
            {
                GraphicsManager.SizeScale *= .9f;
            }
            Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(inputHandler.KeyDown(state, e))
            {
                Refresh();
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta/40;
            GraphicsManager.SizeScale += delta;
            if(GraphicsManager.SizeScale > 70)
            {
                GraphicsManager.SizeScale = 70;
            }
            Refresh();
        }

        private void toolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputHandler.tool = (Tool)toolBox.SelectedIndex;
            gateBox.Visible = inputHandler.tool == Tool.GateController;
            gatePicBox.Visible = inputHandler.tool == Tool.GateController;

            inputHandler.selections.Clear();
            Refresh();
        }

        private void GateBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            gatePicBox.Refresh();
        }

        private void GatePicBox_Paint(object sender, PaintEventArgs e)
        {
            Enum.TryParse<Gates>(gateBox.SelectedValue.ToString(), out var gate);
            Gate newGate = inputHandler.NewGate(gate, new Vec2(4, 2.5f));
            var temp = GraphicsManager.SizeScale;
            GraphicsManager.SizeScale = 15;
            newGate.Draw(e.Graphics);

            GraphicsManager.SizeScale = temp;
        }

        private void GatePicBox_MouseClick(object sender, MouseEventArgs e)
        {
            Enum.TryParse<Gates>(gateBox.SelectedValue.ToString(), out var gate);
            inputHandler.MouseDown(state, (Vec2)e.Location, e.Button, false, gate);
            Refresh();
        }

        public static string debug1Value = "0";
        public static string debug2Value = "0";
        private void debugger1_TextChanged(object sender, EventArgs e)
        {
            if (debugger1.Text != "")
            {
                debug1Value = debugger1.Text;
            }
            Refresh();
        }

        private void debugger2_TextChanged(object sender, EventArgs e)
        {
            if (debugger2.Text != "")
            {
                debug2Value = debugger2.Text;
                Refresh();
            }
        }
    }
}
