using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.Gates;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm
{
    public class InputHandler
    {
        bool mouseLeftDown = false;
        bool mouseRightDown = false;
        WireLine currentLine = new WireLine(new Vec2(), new Vec2(), false);
        WireLine secondaryCurrentLine;

        public Gate currentGate { get; set; }

        public Tool tool { get; set; }

        public InputHandler()
        {
            tool = Tool.WirePainter;
        }

        public bool MouseDown(FlowPropogator propogator, Vec2 position, MouseButtons button, Gates gate)
        {
            bool toRefresh = false;
            Vec2 mousePoint = ((position + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();

            if (button == MouseButtons.Left)
            {
                mouseLeftDown = true;
            }
            else if (button == MouseButtons.Right)
            {
                mouseRightDown = true;
            }
            
            if (tool == Tool.WirePainter)
            {
                if (button == MouseButtons.Left)
                {
                    //Create Line
                    currentLine = new WireLine(mousePoint, mousePoint, true);
                    secondaryCurrentLine = new WireLine(mousePoint, mousePoint, false);

                    //Register Line to draw
                    propogator.wires.Add(secondaryCurrentLine);
                    propogator.wires.Add(currentLine);
                    toRefresh = true;
                }
                else if (button == MouseButtons.Right)
                {
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
                if (mouseLeftDown)
                {
                    currentGate = newGate(gate, position);
                    toRefresh = true;
                }
            }

            return toRefresh;
        }

        public void MouseUp(FlowPropogator propogator)
        {

            if (tool == Tool.WirePainter)
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
                if (mouseLeftDown)
                {
                    mouseLeftDown = false;

                    propogator.gates.Add(currentGate);
                    currentGate.RefreshLocation();
                    currentGate.AddConnections(propogator.Connections);
                }
            }
        }

        public bool MouseMove(Vec2 position, FlowPropogator propogator)
        {
            //Refresh if updated
            bool toRefresh = false;

            //Update End point
            Vec2 mousePoint = ((position + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();

            if (tool == Tool.WirePainter)
            {
                if (mouseLeftDown)
                {
                    toRefresh = mousePoint != secondaryCurrentLine.EndPoint;
                    currentLine.EndPoint = mousePoint;

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
                        var currentLineNewEnd = new Vec2(currentLine.EndPoint.X, currentLine.StartPoint.Y);
                        secondaryCurrentLine.StartPoint = currentLineNewEnd;
                        secondaryCurrentLine.EndPoint = new Vec2(secondaryCurrentLine.StartPoint.X, currentLine.EndPoint.Y);
                        currentLine.EndPoint = currentLineNewEnd;
                    }
                    else
                    {
                        var currentLineNewEnd = new Vec2(currentLine.StartPoint.X, currentLine.EndPoint.Y);
                        secondaryCurrentLine.StartPoint = currentLineNewEnd;
                        secondaryCurrentLine.EndPoint = new Vec2(currentLine.EndPoint.X, secondaryCurrentLine.StartPoint.Y);
                        currentLine.EndPoint = currentLineNewEnd;
                    }
                }

                if (mouseRightDown)
                {
                    toRefresh = mousePoint != currentLine.EndPoint;
                    if (toRefresh)
                    {
                        for (int i = 0; i < propogator.wires.Count; i++)
                        {
                            if (mousePoint.IsContainedIn(propogator.wires[i]))
                            {
                                WireLine.RemovePointFromWire(mousePoint, propogator.Connections, propogator.wires, i);

                                i = -1;
                            }
                        }
                    }
                }
            }
            else if (tool == Tool.GateController)
            {
                if (mouseLeftDown)
                {
                    if(mousePoint != currentGate.Position)
                    {
                        toRefresh = true;
                    }
                    currentGate.Position = mousePoint;
                    currentGate.RefreshLocation();
                }
            }

            return toRefresh;
        }


        private Gate newGate(Gates gate, Vec2 Position)
        {
            switch (gate)
            {
                case Gates.BitSource:
                    return new BitSource(Position);
                case Gates.AndGate:
                    return new AndGate(Position);
                case Gates.NotGate:
                    return new NotGate(Position);
            }
            throw new System.Exception("Gate doesn't exists");
        }
    }
    public enum Tool
    {
        WirePainter,
        GateController
    }
}
