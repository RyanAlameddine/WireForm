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
        InputHandler handler = new InputHandler();
        FlowPropogator propogator = new FlowPropogator();
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolBox.SelectedIndex = 0;
            gateBox.DataSource = Enum.GetValues(typeof(Gates));
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Enum.TryParse<Gates>(gateBox.SelectedValue.ToString(), out var gate);
            bool toRefresh = handler.MouseDown(propogator, (Vec2) e.Location, e.Button, gate);

            if (toRefresh) Refresh();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            handler.MouseUp(propogator);

            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            bool toRefresh = handler.MouseMove((Vec2) e.Location, propogator);

            if(toRefresh) Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Queue<Gate> sources = new Queue<Gate>();
            foreach(Gate gate in propogator.gates)
            {
                if(gate.Inputs.Length == 0)
                {
                    sources.Enqueue(gate);
                }
            }
            propogator.Propogate(sources);

            foreach (Gate gate in propogator.gates)
            {
                gate.Draw(e.Graphics);
            }
            if(handler.currentGate != null) handler.currentGate.Draw(e.Graphics);

            foreach (WireLine wireLine in propogator.wires) {
                painter.DrawWireLine(e.Graphics, wireLine);
            }

            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    e.Graphics._DrawRectangle(Color.Gray, 1, x, y, .02f, .02f);
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 's')
            {
                SaveManager.Save(Path.Combine(Directory.GetCurrentDirectory(), "lines.json"), propogator);
            }
            if(e.KeyChar == 'l')
            {
                SaveManager.Load(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "lines.json")), out var prop);
                propogator = prop;
            }
            if(e.KeyChar == 'c')
            {
                propogator = new FlowPropogator();
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
            handler.tool = (Tool)toolBox.SelectedIndex;
            gateBox.Visible = handler.tool == Tool.GateController;
        }
    }
}
