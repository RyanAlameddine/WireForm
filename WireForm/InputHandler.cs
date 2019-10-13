using System.Collections.Generic;
using System.Diagnostics;
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

        /// <summary>
        /// Tool selected (linked to dropdown on Form1)
        /// </summary>
        public Tool tool { get; set; }

        /// <summary>
        /// Mouse selection box
        /// </summary>
        public BoxCollider mouseBox = null;

        /// <summary>
        /// Intersections of the currentGate with gates already on the board
        /// </summary>
        public HashSet<BoxCollider> intersectionBoxes = new HashSet<BoxCollider>();


        /// <summary>
        /// Gates which have been selected
        /// </summary>
        public HashSet<Gate> selections = new HashSet<Gate>();
        /// <summary>
        /// Gates which have already been selected, but are being added onto by a mouse drag with additiveSelection=true
        /// </summary>
        private HashSet<Gate> preSelections = new HashSet<Gate>();
        /// <summary>
        /// Gate held by mouse
        /// </summary>
        private Gate currentGate;
        /// <summary>
        /// Original position of current gate held by mouse. Will be null if the gate was just created
        /// </summary>
        private Vec2? OGPosition = null;
        /// <summary>
        /// true if the gate has been moved during this drag session
        /// </summary>
        private bool gateMoved = false;
        private bool additiveSelection = false;

        bool mouseLeftDown = false;
        bool mouseRightDown = false;

        WireLine currentLine = new WireLine(new Vec2(), new Vec2(), false);
        WireLine secondaryCurrentLine;

        public InputHandler()
        {
            tool = Tool.WirePainter;
        }


        /// <param name="position">Mouse location</param>
        /// <param name="additiveSelection">Should selection operations clear the selection list before adding more. This bool is usually synonomous to whether or not the shift key is pressed</param>
        /// <param name="gate">The gate to be created from this click</param>
        /// <returns></returns>
        public bool MouseDown(FlowPropogator propogator, Vec2 position, MouseButtons button, bool additiveSelection, Gates? gate)
        {
            this.additiveSelection = additiveSelection;
            bool toRefresh = false;
            ///Position of the mouse in local coordinates rounded to the nearest grid point
            Vec2 mousePointGridded = ((position + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();
            ///Position of the mouse in local coordinates unrounded
            Vec2 mousePointAbsolute = position * (1 / GraphicsManager.SizeScale);

            if (button == MouseButtons.Left)
            {
                mouseLeftDown = true;
            }
            else if (button == MouseButtons.Right)
            {
                mouseRightDown = true;
            }
            
            ///Tool - WirePainter
            if (tool == Tool.WirePainter)
            {
                if (button == MouseButtons.Left)
                {
                    //Create Line
                    currentLine = new WireLine(mousePointGridded, mousePointGridded, true);
                    secondaryCurrentLine = new WireLine(mousePointGridded, mousePointGridded, false);

                    //Register Line to draw
                    propogator.wires.Add(secondaryCurrentLine);
                    propogator.wires.Add(currentLine);
                    toRefresh = true;
                }
                else if (!mouseLeftDown && button == MouseButtons.Right)
                {
                    //Erase Lines
                    for (int i = 0; i < propogator.wires.Count; i++)
                    {
                        if (mousePointGridded.IsContainedIn(propogator.wires[i]))
                        {
                            WireLine.RemovePointFromWire(mousePointGridded, propogator.Connections, propogator.wires, i);

                            i = -1;
                        }
                    }
                    toRefresh = true;
                }
            }
            ///Tool - GateController
            else if (tool == Tool.GateController)
            {
                if (mouseLeftDown)
                {
                    gateMoved = false;
                    //Already holding gate
                    if (currentGate != null)
                    {

                    }
                    //Create new Gate
                    else if(gate != null)
                    {
                        selections.Clear();
                        currentGate = NewGate((Gates) gate, mousePointGridded);
                        selections.Add(currentGate);
                        if (GetIntersections(currentGate.HitBox, propogator, out var intersectBoxes, out _))
                        {
                            intersectionBoxes = intersectBoxes;
                        }
                    }
                    //Select gate with mouse
                    else if (GetIntersections(new BoxCollider(mousePointGridded.X, mousePointGridded.Y, 0, 0), propogator, out _, out var gates, false))
                    {
                        //In the undefined case of multiple hits from a single mouse click, only deal with the first
                        Gate clickedGate = null;
                        foreach (var v in gates)
                        {
                            clickedGate = v;
                        }

                        OGPosition = clickedGate.Position;

                        if (!selections.Contains(clickedGate))
                        {
                            if (!additiveSelection)
                            {
                                selections.Clear();
                            }
                            selections.Add(clickedGate);
                        } else if (additiveSelection)
                        {
                            selections.Remove(clickedGate);
                            return true;
                        }

                        foreach(var selection in selections)
                        {
                            propogator.gates.Remove(selection);
                            selection.RemoveConnections(propogator.Connections);
                        }

                        currentGate = clickedGate;
                    }
                    //Draw selection box
                    else
                    {
                        if (!additiveSelection)
                        {
                            selections.Clear();
                        }
                        else
                        {
                            preSelections.Clear();
                            foreach(var selection in selections)
                            {
                                preSelections.Add(selection);
                            }
                        }
                        mouseBox = new BoxCollider(mousePointAbsolute.X, mousePointAbsolute.Y, 0, 0);
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
                mouseRightDown = false;
                if (mouseLeftDown)
                {
                    mouseLeftDown = false;
                    //Controlling mouse box
                    if (currentGate == null)
                    {
                        mouseBox = null;
                    }
                    //Controlling gates
                    else
                    {
                        bool intersected = GetIntersections(currentGate.HitBox, propogator, out _, out _);
                        if (!intersected)
                        {
                            foreach(var selection in selections)
                            {
                                if(GetIntersections(selection.HitBox, propogator, out _, out _))
                                {
                                    intersected = true;
                                }
                            }
                        }

                        //Gate location invalid
                        if (intersected)
                        {
                            //New gate - delete gate
                            if (OGPosition == null)
                            {
                                selections.Clear();
                                intersectionBoxes.Clear();
                                currentGate = null;
                            }
                            else
                            {
                                //Pre-existing gate - move gates back to how they were originally

                                Vec2 toOffset = currentGate.Position - (Vec2)OGPosition;

                                foreach (var selection in selections)
                                {
                                    selection.Position -= toOffset;

                                    propogator.gates.Add(selection);
                                    selection.AddConnections(propogator.Connections);

                                }

                                OGPosition = null;

                                intersectionBoxes.Clear();
                                currentGate = null;
                            }
                        }
                        //Gate location valid
                        else
                        { 
                            foreach (var selection in selections)
                            {
                                propogator.gates.Add(selection);
                                selection.AddConnections(propogator.Connections);
                            }
                            if (!gateMoved && !additiveSelection)
                            {
                                selections.Clear();
                                selections.Add(currentGate);
                            }

                            //selections.Clear();
                            OGPosition = null;

                            currentGate = null;
                        }
                        gateMoved = false;
                    }
                }
            }
        }

        public bool MouseMove(Vec2 position, FlowPropogator propogator)
        {
            //Refresh if updated
            bool toRefresh = false;

            ///Position of the mouse in local coordinates rounded to the nearest grid point
            Vec2 mousePointGridded = ((position + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();
            ///Position of the mouse in local coordinates unrounded
            Vec2 mousePointAbsolute = position * (1 / GraphicsManager.SizeScale);


            //Tool - WirePainter
            if (tool == Tool.WirePainter)
            {
                if (mouseLeftDown)
                {
                    //Update WireLine
                    toRefresh = mousePointGridded != secondaryCurrentLine.EndPoint;
                    currentLine.EndPoint = mousePointGridded;

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
                else if (mouseRightDown)
                {
                    //Remove wires
                    toRefresh = mousePointGridded != currentLine.EndPoint;
                    if (toRefresh)
                    {
                        for (int i = 0; i < propogator.wires.Count; i++)
                        {
                            if (mousePointGridded.IsContainedIn(propogator.wires[i]))
                            {
                                WireLine.RemovePointFromWire(mousePointGridded, propogator.Connections, propogator.wires, i);

                                i = -1;
                            }
                        }
                    }
                }
            }
            //Tool - GateController
            else if (tool == Tool.GateController)
            {
                if (mouseLeftDown)
                {
                    //Drag mouse box
                    if(currentGate == null)
                    {
                        if (mouseBox == null) return false;
                        mouseBox.Width = mousePointAbsolute.X - mouseBox.X;
                        mouseBox.Height = mousePointAbsolute.Y - mouseBox.Y;

                        //Find all selections
                        GetIntersections(mouseBox.GetNormalized(), propogator, out _, out var gates);
                        selections = gates;
                        if(additiveSelection)
                        {
                            selections.AddRange(preSelections);
                        }

                        toRefresh = true;
                    }
                    //Move current Gate
                    else if (mousePointGridded != currentGate.Position)
                    {
                        toRefresh = true;

                        Vec2 offset = mousePointGridded - currentGate.Position;

                        //Offset selections by mouse movement and draw intersection boxes
                        if (offset != Vec2.Zero)
                        {
                            gateMoved = true;
                            intersectionBoxes.Clear();
                            foreach (var selection in selections)
                            {
                                selection.Position += offset;
                                if (GetIntersections(selection.HitBox, propogator, out var intersects, out _))
                                {
                                    intersectionBoxes.AddRange(intersects);
                                }
                            }
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
        private bool GetIntersections(BoxCollider hitBox, FlowPropogator propogator, out HashSet<BoxCollider> intersectBoxes, out HashSet<Gate> intersectedGates, bool only2D = true)
        {
            intersectBoxes = new HashSet<BoxCollider>();
            intersectedGates = new HashSet<Gate>();
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

        public Gate NewGate(Gates gate, Vec2 Position)
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
