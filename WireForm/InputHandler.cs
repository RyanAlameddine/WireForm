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

        Tool tool { get; set; }

        public InputHandler()
        {
            tool = Tool.Painter;
        }

        public bool MouseDown(FlowPropogator propogator, Point position, MouseButtons button)
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
                    propogator.wires.Add(secondaryCurrentLine);
                    propogator.wires.Add(currentLine);
                    toRefresh = true;
                }
                else if (button == MouseButtons.Right)
                {
                    mouseRightDown = true;
                    for (int i = 0; i < propogator.wires.Count; i++)
                    {
                        if (mousePoint.IsContainedIn(propogator.wires[i]))
                        {
                            WireLine.RemovePointFromWire(mousePoint, propogator.Connections, propogator.wires, i);

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

        public void MouseUp(FlowPropogator propogator)
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

                propogator.wires.Remove(secondaryCurrentLine);
                currentLine.Validate(propogator.wires, propogator.Connections);
                propogator.wires.Add(secondaryCurrentLine);
                secondaryCurrentLine.Validate(propogator.wires, propogator.Connections);
            }
            else if (tool == Tool.GateController)
            {

            }
        }

        public bool MouseMove(Point position, FlowPropogator propogator)
        {
            //Refresh if updated
            bool toRefresh = false;

            //Update End point
            Point newLocation = position.Plus(25).Times(1 / 50f);

            if (tool == Tool.Painter)
            {
                if (mouseLeftDown)
                {
                    toRefresh = newLocation != secondaryCurrentLine.EndPoint;
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
                        for (int i = 0; i < propogator.wires.Count; i++)
                        {
                            if (newLocation.IsContainedIn(propogator.wires[i]))
                            {
                                WireLine.RemovePointFromWire(newLocation, propogator.Connections, propogator.wires, i);

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
    public enum Tool
    {
        Painter,
        GateController
    }
}
