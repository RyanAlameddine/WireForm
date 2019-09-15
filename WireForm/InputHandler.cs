using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireForm
{
    public class InputHandler
    {
        bool mouseLeftDown = false;
        bool mouseRightDown = false;
        WireLine currentLine = new WireLine(Point.Empty, Point.Empty, false);
        WireLine secondaryCurrentLine;

        Tool tool = Tool.Painter;

        public bool MouseDown(List<WireLine> wires, FlowPropogator propogator, Point position, MouseButtons button)
        {
            bool toRefresh = false;
            Point mousePoint = position.Plus(25).Times(1 / 50f);
            if (tool == Tool.Painter)
            {
                if (button == MouseButtons.Left)
                {
                    //Create Line
                    currentLine = new WireLine(mousePoint, mousePoint, true);
                    secondaryCurrentLine = new WireLine(mousePoint, mousePoint, false);
                    mouseLeftDown = true;


                    //Register Line to draw
                    wires.Add(secondaryCurrentLine);
                    wires.Add(currentLine);
                    toRefresh = true;
                }
                else if (button == MouseButtons.Right)
                {
                    mouseRightDown = true;
                    for (int i = 0; i < wires.Count; i++)
                    {
                        if (mousePoint.IsContainedIn(wires[i]))
                        {
                            WireLine.RemovePointFromWire(mousePoint, propogator.Connections, wires, i);

                            i = -1;
                        }
                    }
                    toRefresh = true;
                }
            }
            else if(tool == Tool.GateController)
            {

            }

            return toRefresh;
        } 

        public void MouseUp(List<WireLine> wires, FlowPropogator propogator)
        {
            if (tool == Tool.Painter)
            {
                mouseRightDown = false;
                if (!mouseLeftDown)
                {
                    return;
                }

                mouseLeftDown = false;
                //If line is pointing to itself, delete

                wires.Remove(secondaryCurrentLine);
                currentLine.Validate(wires, propogator.Connections);
                wires.Add(secondaryCurrentLine);
                secondaryCurrentLine.Validate(wires, propogator.Connections);
            }
            else if (tool == Tool.GateController)
            {

            }
        }

        public bool MouseMove(Point position, List<WireLine> wires, FlowPropogator propogator)
        {
            //Refresh if updated
            bool toRefresh = false;

            //Update End point
            Point newLocation = position.Plus(25).Times(1 / 50f);

            if (tool == Tool.Painter)
            {
                if (mouseLeftDown)
                {
                    toRefresh = newLocation != currentLine.EndPoint;
                    currentLine.EndPoint = newLocation;

                    //Define how curvature is drawn
                    if (currentLine.StartPoint.X == currentLine.EndPoint.X)
                    {
                        currentLine.XPriority = false;
                        secondaryCurrentLine.XPriority = true;
                    }
                    if (currentLine.StartPoint.Y == currentLine.EndPoint.Y)
                    {
                        currentLine.XPriority = true;
                        secondaryCurrentLine.XPriority = false;
                    }

                    if (currentLine.XPriority)
                    {
                        var currentLineNewEnd = new Point(currentLine.EndPoint.X, currentLine.StartPoint.Y);
                        secondaryCurrentLine.StartPoint = currentLineNewEnd;
                        secondaryCurrentLine.EndPoint = new Point(secondaryCurrentLine.StartPoint.X, currentLine.EndPoint.Y);
                        currentLine.EndPoint = currentLineNewEnd;
                    }
                    else
                    {
                        var currentLineNewEnd = new Point(currentLine.StartPoint.X, currentLine.EndPoint.Y);
                        secondaryCurrentLine.StartPoint = currentLineNewEnd;
                        secondaryCurrentLine.EndPoint = new Point(currentLine.EndPoint.X, secondaryCurrentLine.StartPoint.Y);
                        currentLine.EndPoint = currentLineNewEnd;
                    }
                }

                if (mouseRightDown)
                {
                    toRefresh = newLocation != currentLine.EndPoint;
                    if (toRefresh)
                    {
                        for (int i = 0; i < wires.Count; i++)
                        {
                            if (newLocation.IsContainedIn(wires[i]))
                            {
                                WireLine.RemovePointFromWire(newLocation, propogator.Connections, wires, i);

                                i = -1;
                            }
                        }
                    }
                }
            }
            else if (tool == Tool.GateController)
            {

            }

            return toRefresh;
        }
    }
    enum Tool
    {
        Painter,
        GateController
    }
}
