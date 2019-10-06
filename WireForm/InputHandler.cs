using System.Collections.Generic;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.Gates;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm
{
    public class InputHandler
    {
        bool mouseLeftDown = false;
        bool mouseRightDown = false;
        WireLine currentLine = new WireLine(new Vec2(), new Vec2(), false);
        WireLine secondaryCurrentLine;

        public Gate currentGate { get; set; }
        public List<BoxCollider> intersectionBoxes = new List<BoxCollider>();
        public List<BoxCollider> selections = new List<BoxCollider>();

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
            
            //Tool - WirePainter
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
                    //Erase Lines
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
            //Tool - GateController
            else if (tool == Tool.GateController)
            {
                //Create Gate
                if (mouseLeftDown)
                {
                    if(GetIntersections(new BoxCollider(mousePoint.X, mousePoint.Y, 0, 0), propogator, out _, true))

                    currentGate = newGate(gate, mousePoint);
                    if (GetIntersections(currentGate.HitBox, propogator, out var intersectBoxes))
                    {
                        intersectionBoxes = intersectBoxes;
                    }
                    toRefresh = true;
                }
            }

            return toRefresh;
        }

        public void MouseUp(FlowPropogator propogator)
        {
            //Tool - WirePainter
            if (tool == Tool.WirePainter)
            {
                mouseRightDown = false;
                if (!mouseLeftDown)
                {
                    return;
                }

                mouseLeftDown = false;
                
                //Validate Wires

                propogator.wires.Remove(secondaryCurrentLine);
                currentLine.Validate(propogator.wires, propogator.Connections);
                propogator.wires.Add(secondaryCurrentLine);
                secondaryCurrentLine.Validate(propogator.wires, propogator.Connections);
            }
            //Tool - GateController
            else if (tool == Tool.GateController)
            {
                
                if (mouseLeftDown)
                {
                    //Validate Gate
                    mouseLeftDown = false;

                    if (GetIntersections(currentGate.HitBox, propogator, out var intersections))
                    {
                        intersectionBoxes.Clear();
                        currentGate = null;
                        return;
                    }
                    else
                    {
                        propogator.gates.Add(currentGate);

                        currentGate.AddConnections(propogator.Connections);
                        currentGate = null;
                    }
                }
            }
        }

        public bool MouseMove(Vec2 position, FlowPropogator propogator)
        {
            //Refresh if updated
            bool toRefresh = false;

            //Update End point
            Vec2 mousePoint = ((position + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();

            //Tool - WirePainter
            if (tool == Tool.WirePainter)
            {
                if (mouseLeftDown)
                {
                    //Update WireLine
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
                    //Remove wires
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
            //Tool - GateController
            else if (tool == Tool.GateController)
            {
                //Move current Gate
                if (mouseLeftDown)
                {
                    if (mousePoint != currentGate.Position)
                    {
                        toRefresh = true;
                        currentGate.Position = mousePoint;

                        if (GetIntersections(currentGate.HitBox, propogator, out var intersections, out _))
                        {
                            intersectionBoxes = intersections;
                        }
                        else
                        {
                            intersectionBoxes.Clear();
                        }
                    }
                }
            }

            return toRefresh;
        }

        /// <summary>
        /// Gets all the Gate intersections a certain BoxCollider hits. If only2D == true, ignores all intersections which are not two-dimensional
        /// </summary>
        /// <param name="intersectBoxes">The rectangles for the intersections</param>
        /// <returns>Did the BoxCollider intersect with anything</returns>
        private bool GetIntersections(BoxCollider hitBox, FlowPropogator propogator, out List<BoxCollider> intersectBoxes, out List<Gate> intersectedGates, bool only2D = true)
        {
            intersectBoxes = new List<BoxCollider>();
            intersectedGates = new List<Gate>();
            foreach(Gate gate in propogator.gates)
            {
                BoxCollider collider = gate.HitBox;
                if(hitBox.Intersects(collider, out var intersection))
                {
                    if (only2D && (intersection.Width == 0 || intersection.Height == 0)) continue;
                    intersectedGates.Add(gate);
                    intersectBoxes.Add(intersection);
                }
            }

            if(intersectBoxes.Count == 0)
            {
                return false;
            }
            return true;
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
                case Gates.OrGate:
                    return new OrGate(Position);
                case Gates.XorGate:
                    return new XorGate(Position);
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
