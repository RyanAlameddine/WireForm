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

namespace WireForm
{
    public partial class Form1 : Form
    {
        Painter painter = new Painter();
        List<WireLine> wireLines = new List<WireLine>();

        Dictionary<Point, List<WireLine>> connections = new Dictionary<Point, List<WireLine>>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        bool mouseDown = false;
        WireLine currentLine;
        WireLine secondaryCurrentLine;

        //bool editing = false;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //Create Line
            currentLine = new WireLine(e.Location.Plus(25).Times(1 / 50f), e.Location.Plus(25).Times(1 / 50f), true);
            secondaryCurrentLine = new WireLine(e.Location.Plus(25).Times(1 / 50f), e.Location.Plus(25).Times(1 / 50f), true);
            mouseDown = true;


            //Register Line to draw
            wireLines.Add(secondaryCurrentLine);
            wireLines.Add(currentLine);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            //If line is pointing to itself, delete
            bool current = true;
            if(currentLine.Start == currentLine.End)
            {
                wireLines.Remove(currentLine);
                current = false;
                Debug.WriteLine("Removed");
            }
            bool secondary = true;
            if (secondaryCurrentLine.Start == secondaryCurrentLine.End)
            {
                secondary = false;
                wireLines.Remove(secondaryCurrentLine);
            }

            if (current)
            {
                if (secondary)
                {
                    wireLines.Remove(secondaryCurrentLine);
                    currentLine.Validate(wireLines, connections);
                    wireLines.Add(secondaryCurrentLine);
                }
                else
                {
                    currentLine.Validate(wireLines, connections);
                }
            }
            if (secondary)
            {
                secondaryCurrentLine.Validate(wireLines, connections);
            }
            
            Debug.WriteLine(wireLines.Count);
            //editing = false;
            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                //Update End point
                currentLine.End = e.Location.Plus(25).Times(1 / 50f);

                //Define how curvature is drawn
                if (currentLine.Start.X == currentLine.End.X)
                {
                    currentLine.XPriority = false;
                    secondaryCurrentLine.XPriority = true;
                }
                if (currentLine.Start.Y == currentLine.End.Y)
                {
                    currentLine.XPriority = true;
                    secondaryCurrentLine.XPriority = false;
                }

                if (currentLine.XPriority)
                {
                    var currentLineNewEnd = new Point(currentLine.End.X, currentLine.Start.Y);
                    secondaryCurrentLine.Start = currentLineNewEnd;
                    secondaryCurrentLine.End = new Point(secondaryCurrentLine.Start.X, currentLine.End.Y);
                    Debug.WriteLine(secondaryCurrentLine.End);
                    currentLine.End = currentLineNewEnd;
                }
                else
                {
                    var currentLineNewEnd = new Point(currentLine.Start.X, currentLine.End.Y);
                    secondaryCurrentLine.Start = currentLineNewEnd;
                    secondaryCurrentLine.End = new Point(currentLine.End.X, secondaryCurrentLine.Start.Y);
                    currentLine.End = currentLineNewEnd;
                }

                //Refresh if updated
                bool toRefresh = false;
                if(wireLines[wireLines.Count - 1].End != currentLine.End)
                {
                    toRefresh = true;
                }
                wireLines[wireLines.Count - 1] = currentLine;
                if (wireLines[wireLines.Count - 2].End != secondaryCurrentLine.End)
                {
                    toRefresh = true;
                }
                wireLines[wireLines.Count - 2] = secondaryCurrentLine;


                if (toRefresh)
                {
                    Refresh();
                }
            }
        }


        Point temp = new Point(1, 1);
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (WireLine wireLine in wireLines) {
                painter.DrawLine(e.Graphics, wireLine);
            }

            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Gray, 1), new Rectangle(new Point(x * 50, y * 50), new Size(1, 1)));
                }
            }
            ColorPropogate(temp, new WireLine(), e.Graphics, new List<WireLine>());
        }

        private void ColorPropogate(Point point, WireLine prevLine, Graphics gfx, List<WireLine> processed)
        {
            if (!connections.ContainsKey(point))
            {
                return;
            }
            List<WireLine> lines = connections[point];
            foreach (WireLine line in lines)
            {
                if (processed.Contains(line))
                {
                    continue;
                }
                gfx.DrawLine(new Pen(Color.Blue, 3), line.Start.Times(50), line.End.Times(50));
                processed.Add(line);
                if (line.Start == prevLine.Start && line.End == prevLine.End)
                {
                    continue;
                }
                ColorPropogate(line.Start, line, gfx, processed);
                ColorPropogate(line.End, line, gfx, processed);
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 's')
            {
                SaveManager.Save(Path.Combine(Directory.GetCurrentDirectory(), "lines.json"), wireLines);
            }
            if(e.KeyChar == 'l')
            {
                SaveManager.Load(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "lines.json")), out connections, out wireLines);
                
            }
            if(e.KeyChar == 'c')
            {
                wireLines.Clear();
                connections.Clear();
            }
            Refresh();
        }

    }
}
