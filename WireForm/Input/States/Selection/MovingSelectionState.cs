using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
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
        private readonly bool resettable;

        private readonly HashSet<BoxCollider> resetBoxes = new HashSet<BoxCollider>();

        private readonly HashSet<BoxCollider> intersectedBoxes = new HashSet<BoxCollider>();

        /// <param name="resettable">
        /// true if the objects which are being moved already exists, 
        /// and thus has a valid start position
        /// </param>
        public MovingSelectionState(Vec2 mousePosition, HashSet<CircuitObject> selections, CircuitObject selectedObject, BoardState state, bool resettable) : base(selections)
        {            
            this.selectedObject = selectedObject;
            this.resettable = resettable;

            foreach(var selection in selections)
            {
                resetBoxes.Add(selection.HitBox);
            }

            Vec2 localPoint = MathHelper.ViewportToLocalPoint(mousePosition);
            startPosition = selectedObject.StartPoint;
            offset = selectedObject.StartPoint - localPoint;

            state.DetatchAll(selections);
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

        public override InputReturns MouseMove(InputControls inputControls)
        {
            Vec2 newPosition = MathHelper.ViewportToLocalPoint(inputControls.MousePosition) + offset;
            Vec2 gridPoint = newPosition.Round();

            if (gridPoint == selectedObject.StartPoint) return (false, this);

            Vec2 change = gridPoint - selectedObject.StartPoint;
            //Debug.WriteLine(change);

            intersectedBoxes.Clear();
            //Drag all objects to their new positions
            foreach(var selection in selections)
            {
                selection.StartPoint += change;
                
                if (selection is WireLine wire)
                {
                    wire.EndPoint += change;
                }
                //Calculate intersections with other gates
                else
                {
                    if (selection.HitBox.GetIntersections(inputControls.State, false, out var intersects, out _))
                    {
                        intersectedBoxes.UnionWith(intersects);
                    }
                }
            }

            return (true, this);
        }

        public override InputReturns MouseLeftUp(InputControls inputControls)
        {
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
                    selection.StartPoint += totalOffset;

                    if (selection is WireLine wire)
                    {
                        wire.EndPoint += totalOffset;
                    }
                }
                inputControls.State.AttachAll(selections);
            }


            inputControls.State.AttachAll(selections);

            //If it has moved, register movement
            if (intersectedBoxes.Count == 0 && startPosition != selectedObject.StartPoint)
                inputControls.RegisterChange(resettable ? "Moved selections" : "Placed selections");

            return (true, new SelectionToolState(selections));
        }


    }
}