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

        bool mouseLeftDown = false;
        bool mouseRightDown = false;
        WireLine currentLine = new WireLine(Point.Empty, Point.Empty, false);
        WireLine secondaryCurrentLine;

        //bool editing = false;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Point mousePoint = e.Location.Plus(25).Times(1 / 50f);
            if (e.Button == MouseButtons.Left)
            {
                //Create Line
                currentLine = new WireLine(mousePoint, mousePoint, true);
                secondaryCurrentLine = new WireLine(mousePoint, mousePoint, true);
                mouseLeftDown = true;


                //Register Line to draw
                wireLines.Add(secondaryCurrentLine);
                wireLines.Add(currentLine);
            }
            else if(e.Button == MouseButtons.Right)
            {
                mouseRightDown = true;
                for (int i = 0; i < wireLines.Count; i++)
                {
                    if (mousePoint.IsContainedIn(wireLines[i]))
                    {
                        WireLine.RemovePointFromWire(mousePoint, connections, wireLines, i);

                        i = -1;
                    }
                }
                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseRightDown = false;
            if (!mouseLeftDown)
            {
                return;
            }

            mouseLeftDown = false;
            //If line is pointing to itself, delete

            wireLines.Remove(secondaryCurrentLine);
            currentLine.Validate(wireLines, connections);
            wireLines.Add(secondaryCurrentLine);
            secondaryCurrentLine.Validate(wireLines, connections);
            
            Debug.WriteLine(wireLines.Count);
            //editing = false;
            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //Refresh if updated
            bool toRefresh = false;

            //Update End point
            Point newLocation = e.Location.Plus(25).Times(1 / 50f);
            toRefresh = newLocation != currentLine.End;

            if (mouseLeftDown)
            {
                currentLine.End = newLocation;

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
                    currentLine.End = currentLineNewEnd;
                }
                else
                {
                    var currentLineNewEnd = new Point(currentLine.Start.X, currentLine.End.Y);
                    secondaryCurrentLine.Start = currentLineNewEnd;
                    secondaryCurrentLine.End = new Point(currentLine.End.X, secondaryCurrentLine.Start.Y);
                    currentLine.End = currentLineNewEnd;
                }


                if (toRefresh)
                {
                    Refresh();
                }
            }

            if (mouseRightDown)
            {
                if (toRefresh)
                {
                    for (int i = 0; i < wireLines.Count; i++)
                    {
                        if (newLocation.IsContainedIn(wireLines[i]))
                        {
                            WireLine.RemovePointFromWire(newLocation, connections, wireLines, i);

                            i = -1;
                        }
                    }
                    Refresh();
                }
            }
        }


        Point temp = new Point(1, 1);
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (WireLine wireLine in wireLines) {
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
