using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
            if(currentLine.WireStart == currentLine.WireEnd)
            {
                wireLines.Remove(currentLine);
                current = false;
                Debug.WriteLine("Removed");
            }
            bool secondary = true;
            if (secondaryCurrentLine.WireStart == secondaryCurrentLine.WireEnd)
            {
                secondary = false;
                wireLines.Remove(secondaryCurrentLine);
            }

            if (current)
            {
                if (secondary)
                {
                    wireLines.Remove(secondaryCurrentLine);
                    currentLine.IncludeWire(wireLines);
                    wireLines.Add(secondaryCurrentLine);
                }
                else
                {
                    currentLine.IncludeWire(wireLines);
                }
            }
            if (secondary)
            {
                secondaryCurrentLine.IncludeWire(wireLines);
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
                currentLine.WireEnd = e.Location.Plus(25).Times(1 / 50f);

                //Define how curvature is drawn
                if (currentLine.WireStart.X == currentLine.WireEnd.X)
                {
                    currentLine.XPriority = false;
                    secondaryCurrentLine.XPriority = true;
                }
                if (currentLine.WireStart.Y == currentLine.WireEnd.Y)
                {
                    currentLine.XPriority = true;
                    secondaryCurrentLine.XPriority = false;
                }

                if (currentLine.XPriority)
                {
                    var currentLineNewEnd = new Point(currentLine.WireEnd.X, currentLine.WireStart.Y);
                    secondaryCurrentLine.WireStart = currentLineNewEnd;
                    secondaryCurrentLine.WireEnd = new Point(secondaryCurrentLine.WireStart.X, currentLine.WireEnd.Y);
                    Debug.WriteLine(secondaryCurrentLine.WireEnd);
                    currentLine.WireEnd = currentLineNewEnd;
                }
                else
                {
                    var currentLineNewEnd = new Point(currentLine.WireStart.X, currentLine.WireEnd.Y);
                    secondaryCurrentLine.WireStart = currentLineNewEnd;
                    secondaryCurrentLine.WireEnd = new Point(currentLine.WireEnd.X, secondaryCurrentLine.WireStart.Y);
                    currentLine.WireEnd = currentLineNewEnd;
                }

                //Refresh if updated
                bool toRefresh = false;
                if(wireLines[wireLines.Count - 1].WireEnd != currentLine.WireEnd)
                {
                    toRefresh = true;
                }
                wireLines[wireLines.Count - 1] = currentLine;
                if (wireLines[wireLines.Count - 2].WireEnd != secondaryCurrentLine.WireEnd)
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

        }
    }
}
