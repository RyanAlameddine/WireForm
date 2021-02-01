using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Wireform;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using WireformInput.Utils;

namespace WireformInput.States.Selection
{
    /// <summary>
    /// State in which selected objects are being dragged
    /// </summary>
    class MovingSelectionState : SelectionStateBase
    {
        private readonly Vec2 offset;
        private readonly Vec2 startPosition;
        private readonly BoardObject selectedObject;

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
        public MovingSelectionState(Vec2 localPoint, HashSet<BoardObject> selections, BoardObject selectedObject, BoardState state, bool resettable) : base(selections)
        {
            //Make sure all selections start in a gridded position;
            foreach (var selection in selections)
            {
                if(selection.Gridded) selection.SetPosition(selection.StartPoint.Round());
            }

            this.selectedObject = selectedObject;
            this.resettable = resettable;

            foreach(var selection in selections)
            {
                resetBoxes.Add(selection.HitBox);
            }

            //Calculate offset from held location to current Mouse position
            startPosition = selectedObject.StartPoint;
            offset = selectedObject.StartPoint - localPoint;

            if (resettable)
            {
                state.DetatchAll(selections);
            }
            else
            {
                //set startPosition to an invalid start value
                startPosition = new Vec2(float.MinValue, float.MinValue);
            }

            CheckIntersections(state);
        }

        public override async Task Draw(BoardState currentState, PainterScope painter)
        {
            if (resettable)
            {
                if (intersectedBoxes.Count > 0)
                {
                    foreach (var resetBox in resetBoxes)
                    {
                        await painter.FillRectangle(Color.FromArgb(64, 128, 128, 255), resetBox.Position, resetBox.Bounds);
                    }
                }
            }

            await base.Draw(currentState, painter);

            foreach (BoxCollider intersection in intersectedBoxes)
            {
                await painter.FillRectangle(Color.FromArgb(128, 255, 0, 0), intersection.Position, intersection.Bounds);
            }
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {
            Vec2 gridPoint = stateControls.LocalMousePosition + offset;
            Vec2 gridPointR = gridPoint.Round();

            bool delta = gridPoint == selectedObject.StartPoint; //ungridded position unchanged
            bool deltaR = gridPointR == selectedObject.StartPoint.Round(); //gridded position unchanged
            if (selectedObject.Gridded && deltaR || !selectedObject.Gridded && delta) return (false, this);

            Vec2 change = gridPoint - selectedObject.StartPoint;
            Vec2 changeR = gridPointR - selectedObject.StartPoint.Round();

            if(!deltaR)
                intersectedBoxes.Clear();

            //Drag all objects to their new positions
            foreach(var selection in selections)
            {
                if (selectedObject.Gridded) selection.OffsetPosition(changeR);
                else selection.OffsetPosition(selection.Gridded ? changeR : change);
            }

            if(!deltaR)
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
                    if (selection.HitBox.GetIntersections(state, (true, false, false), out var intersects, out _))
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
                Vec2 totalOffsetR = startPosition.Round() - selectedObject.StartPoint.Round();
                foreach (var selection in selections)
                {
                    selection.OffsetPosition(selection.Gridded ? totalOffsetR : totalOffset);
                }
            }


            stateControls.State.AttachAll(selections);

            //If it has moved, register movement
            if (intersectedBoxes.Count == 0 && startPosition != selectedObject.StartPoint)
                stateControls.RegisterChange(resettable ? "Moved selections" : "Placed selections");
            else
                stateControls.State.Propogate();

            return (true, new SelectionToolState(selections));
        }


    }
}