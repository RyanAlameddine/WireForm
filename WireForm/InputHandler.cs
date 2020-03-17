﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates;
using WireForm.Circuitry.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm
{
    /// <summary>
    /// NOTE: This is the LEAST well organized system in the entire project, so I apologize if you have to directly code in here.
    /// This will be COMPLETELY reworked to a cleaner state machine in a later version.
    /// </summary>
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
        /// Gates/Wires which have been selected
        /// </summary>
        public HashSet<CircuitObject> selections = new HashSet<CircuitObject>();
        /// <summary>
        /// Gates/Wires which have already been selected, but are being added onto by a mouse drag with additiveSelection=true
        /// </summary>
        private HashSet<CircuitObject> preSelections = new HashSet<CircuitObject>();
        public HashSet<CircuitObject> clipBoard = new HashSet<CircuitObject>();

        /// <summary>
        /// Gates/Wires held by mouse
        /// </summary>
        private CircuitObject currentcircuitObject;
        /// <summary>
        /// Original position of current gate held by mouse. Will be null if the gate was just created
        /// </summary>
        private Vec2? OGPosition = null;
        /// <summary>
        /// Original hitbox of the selected gates being moved by the mouse. Only used for drawing
        /// </summary>
        public HashSet<BoxCollider> resetBoxes = new HashSet<BoxCollider>();
        /// <summary>
        /// true if the gate/wire has been moved during this drag session
        /// </summary>
        private bool circuitObjectMoved = false;
        private bool additiveSelection = false;

        bool mouseLeftDown = false;
        bool mouseRightDown = false;

        WireLine currentLine = new WireLine(new Vec2(), new Vec2(), false);
        WireLine secondaryCurrentLine;

        public InputHandler()
        {
            tool = Tool.WirePainter;
        }

        public void Undo(StateStack stateStack)
        {
            stateStack.Reverse();
            selections.Clear();
        }

        public void Redo(StateStack stateStack)
        {
            stateStack.Advance();
            selections.Clear();
        }

        public void Copy()
        {
            clipBoard.Clear();
            foreach (var selection in selections)
            {
                clipBoard.Add(selection.Copy());
            }
        }

        public void Cut(StateStack stateStack)
        {
            clipBoard.Clear();
            foreach (var selection in selections)
            {
                selection.Delete(stateStack.CurrentState);
                clipBoard.Add(selection.Copy());
            }
            selections.Clear();
            OGPosition = null;
        }

        public void Paste()
        {
            mouseLeftDown = true;
            selections.Clear();
            OGPosition = null;
            HashSet<CircuitObject> newClipBoard = new HashSet<CircuitObject>();

            foreach (var obj in clipBoard)
            {
                newClipBoard.Add(obj.Copy());
                selections.Add(obj);

                obj.StartPoint += new Vec2(1, 1);

                WireLine asWire = obj as WireLine;
                if (asWire != null)
                {
                    asWire.EndPoint += new Vec2(1, 1);
                }

                currentcircuitObject = obj;
            }
            clipBoard = newClipBoard;
        }


        /// <param name="position">Mouse location</param>
        /// <param name="additiveSelection">Should selection operations clear the selection list before adding more. This bool is usually synonomous to whether or not the shift key is pressed</param>
        /// <param name="gate">The gate to be created from this click</param>
        /// <returns></returns>
        public bool MouseDown(StateStack stateStack, Vec2 position, Form1 form, MouseButtons button, ContextMenuStrip gateMenu, bool additiveSelection, int? gateIndex)
        {
            BoardState state = stateStack.CurrentState;
            this.additiveSelection = additiveSelection;
            bool toRefresh = false;
            ///Position of the mouse in local coordinates rounded to the nearest grid point
            Vec2 mousePointGridded = ((position + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();
            ///Position of the mouse in local coordinates unrounded
            Vec2 mousePointAbsolute = position * (1 / GraphicsManager.SizeScale);

            if (button == MouseButtons.Left)
            {
                if (mouseLeftDown) return false;
                mouseLeftDown = true;
            }
            else if (button == MouseButtons.Right)
            {
                if (mouseRightDown) return false;
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
                    state.wires.Add(secondaryCurrentLine);
                    state.wires.Add(currentLine);
                    toRefresh = true;
                }
                else if (!mouseLeftDown && button == MouseButtons.Right)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    //Erase Lines
                    for (int i = 0; i < state.wires.Count; i++)
                    {
                        if (mousePointGridded.IsContainedIn(state.wires[i]))
                        {
                            WireLine.RemovePointFromWire(mousePointGridded, state.Connections, state.wires, i);
                            
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
                    circuitObjectMoved = false;
                    //Create new Gate
                    if(gateIndex != null)
                    {
                        selections.Clear();
                        currentcircuitObject = GateEnum.NewGate((int) gateIndex, mousePointGridded);
                        selections.Add(currentcircuitObject);
                        if (currentcircuitObject.HitBox.GetIntersections(state, true, out var intersectBoxes, out _))
                        {
                            intersectionBoxes = intersectBoxes;
                        }
                    }
                    //Select circuitObject with mouse
                    else if (new BoxCollider(mousePointGridded.X, mousePointGridded.Y, 0, 0).GetIntersections(state, true, out _, out var gates, false))
                    {
                        //In the undefined case of multiple hits from a single mouse click, only deal with the first
                        CircuitObject clickedcircuitObject = null;
                        foreach (var v in gates)
                        {
                            clickedcircuitObject = v;
                        }

                        OGPosition = clickedcircuitObject.StartPoint;
                        resetBoxes.Clear();

                        if (!selections.Contains(clickedcircuitObject))
                        {
                            if (!additiveSelection)
                            {
                                selections.Clear();
                            }
                            selections.Add(clickedcircuitObject);
                        } else if (additiveSelection)
                        {
                            selections.Remove(clickedcircuitObject);
                            clickedcircuitObject = null;
                            currentcircuitObject = null;
                            OGPosition = null;
                            return true;
                        }

                        foreach(var selection in selections)
                        {
                            resetBoxes.Add(selection.HitBox.Copy());

                            Gate asGate = selection as Gate;
                            WireLine asWire = selection as WireLine;
                            if(asGate != null)
                            {
                                state.gates.Remove(asGate);
                            }
                            else if(asWire != null)
                            {
                                state.wires.Remove(asWire);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                            selection.RemoveConnections(state.Connections);
                        }

                        currentcircuitObject = clickedcircuitObject;
                    }
                    //Draw selection box
                    else
                    {
                        if (!additiveSelection)
                        {
                            selections.Clear();
                            OGPosition = null;
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
                else if (mouseRightDown)
                {
                    if (new BoxCollider(mousePointGridded.X, mousePointGridded.Y, 0, 0).GetIntersections(state, true, out _, out var circuitObjects, false))
                    {
                        CircuitObject clickedcircuitObject = null;
                        foreach (var v in circuitObjects)
                        {
                            clickedcircuitObject = v;
                        }

                        var actions = CircuitActionAttribute.GetActions(clickedcircuitObject, stateStack, form.drawingPanel);

                        ///Add refreshing to the actions if necessary
                        gateMenu.Items.Clear();
                        for (int i = 0; i < actions.Count; i++)
                        {
                            gateMenu.Items.Add(actions[i].attribute.Name, null, actions[i].action);
                        }

                        gateMenu.Show(form, (Point)position);
                        toRefresh = true;
                    }
                }
            }

            return toRefresh;
        }

        public void MouseUp(StateStack stateStack, MouseButtons button)
        {
            BoardState state = stateStack.CurrentState;
            //Tool - WirePainter
            if (tool == Tool.WirePainter)
            {
                if (button == MouseButtons.Left)
                {
                    if (!mouseLeftDown)
                    {
                        return;
                    }
                    mouseLeftDown = false;
                    //Validate Wires
                    state.wires.Remove(secondaryCurrentLine);
                    currentLine.Validate(state.wires, state.Connections);
                    state.wires.Add(secondaryCurrentLine);
                    secondaryCurrentLine.Validate(state.wires, state.Connections);
                    stateStack.RegisterChange($"Created wire from {currentLine.StartPoint}-{secondaryCurrentLine.EndPoint}");
                }
                else if (button == MouseButtons.Right)
                {
                    if (!mouseRightDown)
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }
                    mouseRightDown = false;
                    stateStack.RegisterChange("Erased wires");
                }
            }
            //Tool - GateController
            else if (tool == Tool.GateController)
            {
                mouseRightDown = false;
                if (button == MouseButtons.Left)
                {
                    mouseLeftDown = false;
                    //Controlling mouse box
                    if (currentcircuitObject == null)
                    {
                        mouseBox = null;
                    }
                    //Controlling gates
                    else
                    {
                        bool intersected = false;
                        foreach(var selection in selections)
                        {
                            if(!(selection is WireLine) && selection.HitBox.GetIntersections(state, false, out _, out _))
                            {
                                intersected = true;
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
                                currentcircuitObject = null;
                            }
                            else
                            {
                                //Pre-existing gate - move gates back to how they were originally

                                Vec2 toOffset = currentcircuitObject.StartPoint - (Vec2)OGPosition;

                                foreach (var selection in selections)
                                {
                                    selection.StartPoint -= toOffset;

                                    Gate asGate = selection as Gate;
                                    WireLine asWire = selection as WireLine;
                                    if (asGate != null)
                                    {
                                        state.gates.Add(asGate);
                                    }
                                    else if (asWire != null)
                                    {
                                        asWire.EndPoint -= toOffset;
                                        state.wires.Add(asWire);
                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }
                                    selection.AddConnections(state.Connections);

                                }

                                OGPosition = null;
                                resetBoxes.Clear();

                                intersectionBoxes.Clear();
                                currentcircuitObject = null;
                            }
                        }
                        //Gate location valid
                        else
                        {
                            List<WireLine> toRemove = new List<WireLine>();
                            List<WireLine> toAdd = new List<WireLine>();
                            foreach (var selection in selections)
                            {
                                Gate asGate = selection as Gate;
                                WireLine asWire = selection as WireLine;
                                if (asGate != null)
                                {
                                    state.gates.Add(asGate);
                                    selection.AddConnections(state.Connections);
                                }
                                else if (asWire != null)
                                {
                                    List<WireLine> newWires = asWire.Validate(state.wires, state.Connections);
                                    toAdd.AddRange(newWires);
                                    toRemove.Add(asWire);
                                    
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }

                            }

                            foreach (var x in toRemove)
                            {
                                selections.Remove(x);
                            }
                            foreach (var x in toAdd)
                            {
                                selections.Add(x);
                            }
                            

                            if (!circuitObjectMoved && !additiveSelection)
                            {
                                selections.Clear();
                                selections.Add(currentcircuitObject);
                            }

                            if (circuitObjectMoved)
                            {
                                string message;

                                if (OGPosition == null)
                                {
                                    message = $"Created Gate at {selections.First().StartPoint}";
                                }
                                else
                                {
                                    if (selections.Count == 0)
                                    {
                                        message = "Moved selections";
                                    }
                                    else if (selections.First() is WireLine)
                                    {
                                        message = $"Moved wire from {OGPosition}-{selections.First().StartPoint}";
                                    }
                                    else if(selections.First() is Gate)
                                    {
                                        message = $"Moved gate from {OGPosition}-{selections.First().StartPoint}";
                                    }
                                    else
                                    {
                                        message = "Moved selections";
                                    }
                                }
                                stateStack.RegisterChange(message);
                            }

                            OGPosition = null;
                            resetBoxes.Clear();

                            currentcircuitObject = null;
                        }
                        circuitObjectMoved = false;
                    }
                }
            }
        }

        public bool MouseMove(Vec2 position, BoardState propogator)
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
                        currentLine.IsHorizontal = false;
                        secondaryCurrentLine.IsHorizontal = true;
                    }
                    if (currentLine.StartPoint.Y == currentLine.EndPoint.Y)
                    {
                        currentLine.IsHorizontal = true;
                        secondaryCurrentLine.IsHorizontal = false;
                    }

                    if (currentLine.IsHorizontal)
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
                    if (mousePointGridded != currentLine.EndPoint)
                    {
                        for (int i = 0; i < propogator.wires.Count; i++)
                        {
                            if (mousePointGridded.IsContainedIn(propogator.wires[i]))
                            {
                                toRefresh = true;
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
                    if(currentcircuitObject == null)
                    {
                        if (mouseBox == null) return false;
                        mouseBox.Width = mousePointAbsolute.X - mouseBox.X;
                        mouseBox.Height = mousePointAbsolute.Y - mouseBox.Y;

                        //Find all selections
                        mouseBox.GetNormalized().GetIntersections(propogator, true, out _, out var gates);
                        selections = gates;
                        if(additiveSelection)
                        {
                            selections.UnionWith(preSelections);
                        }

                        toRefresh = true;
                    }
                    //Move current Gate
                    else if (mousePointGridded != currentcircuitObject.StartPoint)
                    {
                        toRefresh = true;

                        Vec2 offset = mousePointGridded - currentcircuitObject.StartPoint;

                        //Offset selections by mouse movement and draw intersection boxes
                        if (offset != Vec2.Zero)
                        {
                            circuitObjectMoved = true;
                            intersectionBoxes.Clear();
                            foreach (var selection in selections)
                            {
                                selection.StartPoint += offset;
                                WireLine asWire = selection as WireLine;
                                if (asWire != null)
                                {
                                    asWire.EndPoint += offset;
                                }
                                else
                                {
                                    if (selection.HitBox.GetIntersections(propogator, false, out var intersects, out _))
                                    {
                                        intersectionBoxes.UnionWith(intersects);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return toRefresh;
        }

        /// <summary>
        /// Check through selection list to confirm that everything still exists
        /// </summary>
        public bool RefreshSelections(BoardState state)
        {
            int count = selections.Count;
            bool nullcco = currentcircuitObject == null;
            selections.RemoveWhere((x) =>
            {
                var gate = x as Gate;
                var wire = x as WireLine;
                if (wire != null)
                {
                    return !state.wires.Contains(wire);
                }
                else if (gate != null)
                {
                    return !state.gates.Contains(gate);
                }
                else
                {
                    throw new Exception("Invalid object selected");
                }
            });
            if (!selections.Contains(currentcircuitObject))
            {
                if(selections.Count == 0)
                {
                    currentcircuitObject = null;
                }
                else if(!nullcco)
                {
                    currentcircuitObject = selections.First();
                }
            }

            return count != selections.Count;
        }

        public bool KeyDown(StateStack stateStack, KeyEventArgs e, Form1 form)
        {
            BoardState state = stateStack.CurrentState;

            //object hotkeys
            bool hit = false;
            if (currentcircuitObject == null)
            {
                foreach (CircuitObject selection in selections)
                {
                    var actions = CircuitActionAttribute.GetActions(selection, stateStack, form.drawingPanel);
                    foreach (var action in actions)
                    {
                        if (action.attribute.Hotkey == e.KeyCode)
                        {
                            hit = true;
                            action.action.Invoke(this, null);

                        }
                    }
                }
            }
            bool toRefresh = hit ? RefreshSelections(state) : false;


            return toRefresh;
        }
    }
    public enum Tool
    {
        WirePainter,
        GateController
    }
}
