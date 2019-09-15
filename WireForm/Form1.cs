using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Gates;

namespace WireForm
{
    public partial class Form1 : Form
    {
        Painter painter = new Painter();
        InputHandler handler = new InputHandler();
        FlowPropogator propogator = new FlowPropogator();
        List<WireLine> wires = new List<WireLine>();
        List<Gate> gates = new List<Gate>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gates.Add(new BitSource(new Point(3, 3), propogator.Connections));
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool toRefresh = handler.MouseDown(wires, propogator, e.Location, e.Button);

            if (toRefresh) Refresh();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            handler.MouseUp(wires, propogator);

            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            bool toRefresh = handler.MouseMove(e.Location, wires, propogator);

            if(toRefresh) Refresh();
        }


        Point temp = new Point(1, 1);
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Queue<Gate> sources = new Queue<Gate>();
            foreach(Gate gate in gates)
            {
                if(gate.InputCount == 0)
                {
                    sources.Enqueue(gate);
                }
                gate.Draw(e.Graphics);
            }
            propogator.Propogate(sources, wires);

            foreach (WireLine wireLine in wires) {
                painter.DrawWireLine(e.Graphics, wireLine);
            }

            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Gray, 1), new Rectangle(new Point(x * 50, y * 50), new Size(1, 1)));
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 's')
            {
                SaveManager.Save(Path.Combine(Directory.GetCurrentDirectory(), "lines.json"), wires);
            }
            if(e.KeyChar == 'l')
            {
                SaveManager.Load(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "lines.json")), out var connects, out wires);
                propogator.Connections = connects;
            }
            if(e.KeyChar == 'c')
            {
                wires.Clear();
                propogator.Connections.Clear();
            }
            Refresh();
        }

    }
}
