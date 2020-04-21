using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Input.States.Selection
{
    /// <summary>
    /// State in which selected objects are being dragged
    /// </summary>
    class MovingSelectionState : SelectionStateBase
    {
        private readonly Vec2 offset;
        private readonly Vec2 startPosition;
        private readonly CircuitObject selectedObject;

        /// <summary>
        /// false if the object has just been created (like from a paste operation) and has no place to reset to if
        /// placed in an invalid location
        /// </summary>
        private readonly bool resettable;

        /// <summary>
        /// Blue boxes notifying the user where their selections will reset to if placed in an invalid location
        /// </summary>
        private readonly HashSet<BoxCollider> resetBoxes = new HashSet<BoxCollider>();

        /// <summary>
        /// Red boxes notifying the user where their selections are in an invalid location
        /// </summary>
        private readonly HashSet<BoxCollider> intersectedBoxes = new HashSet<BoxCollider>();

        /// <param name="resettable">
        /// true if the objects which are being moved already exists, 
        /// and thus has a valid start position
        /// </param>
        public MovingSelectionState(Vec2 mousePosition, HashSet<CircuitObject> selections, CircuitObject selectedObject, BoardState state, bool resettable) : base(selections)
        {
            //Make sure all selections start in a gridded position;
            foreach (var selection in selections)
            {
                selection.SetPosition(selection.StartPoint.Round());
            }

            this.selectedObject = selectedObject;
            this.resettable = resettable;

            foreach(var selection in selections)
            {
                resetBoxes.Add(selection.HitBox);
            }

            //Calculate offset from held location to current Mouse position
            Vec2 localPoint = MathHelper.ViewportToLocalPoint(mousePosition);
            startPosition = selectedObject.StartPoint;
            offset = selectedObject.StartPoint - localPoint;

            if(resettable) state.DetatchAll(selections);

            CheckIntersections(state);
        }

        public override void Draw(BoardState currentState, PainterScope painter)
        {
            if (resettable)
            {
                if (intersectedBoxes.Count > 0)
                {
                    foreach (var resetBox in resetBoxes)
                    {
                        painter.FillRectangle(Color.FromArgb(64, 128, 128, 255), resetBox.Position, resetBox.Bounds);
                    }
                }
            }

            base.Draw(currentState, painter);

            foreach (BoxCollider intersection in intersectedBoxes)
            {
                painter.FillRectangle(Color.FromArgb(128, 255, 0, 0), intersection.Position, intersection.Bounds);
            }
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {
            Vec2 newPosition = MathHelper.ViewportToLocalPoint(stateControls.MousePosition) + offset;
            Vec2 gridPoint = newPosition.Round();

            if (gridPoint == selectedObject.StartPoint) return (false, this);

            Vec2 change = gridPoint - selectedObject.StartPoint;

            intersectedBoxes.Clear();
            //Drag all objects to their new positions
            foreach(var selection in selections)
            {
                selection.OffsetPosition(change);
            }

            CheckIntersections(stateControls.State);

            return (true, this);
        }

        private void CheckIntersections(BoardState state)
        {
            foreach(var selection in selections)
            {
                //Calculate intersections with other gates
                if (selection is Gate)
                {
                    if (selection.HitBox.GetIntersections(state, false, out var intersects, out _))
                    {
                        intersectedBoxes.UnionWith(intersects);
                    }
                }
            }
        }

        public override InputReturns MouseLeftUp(StateControls stateControls)
        {
            CheckIntersections(stateControls.State);
            if (intersectedBoxes.Count > 0)
            {
                //If the objects were just created and are placed in an invalid state, they have nowhere
                //to be reset to and thus should be destroyed
                if (!resettable)
                {
                    selections.Clear();
                    return (true, new SelectionToolState(selections));
                }

                Vec2 totalOffset = startPosition - selectedObject.StartPoint;
                foreach (var selection in selections)
                {
                    selection.OffsetPosition(totalOffset);
                }
            }


            stateControls.State.AttachAll(selections);

            //If it has moved, register movement
            if (intersectedBoxes.Count == 0 && startPosition != selectedObject.StartPoint)
                stateControls.RegisterChange(resettable ? "Moved selections" : "Placed selections");

            return (true, new SelectionToolState(selections));
        }


    }
}