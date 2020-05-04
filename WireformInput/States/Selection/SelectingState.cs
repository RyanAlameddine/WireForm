using System.Collections.Generic;
using System.Drawing;
using Wireform.Circuitry.Data;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using WireformInput.Utils;

namespace WireformInput.States.Selection
{
    /// <summary>
    /// The state in which a selection box is being drawn.
    /// </summary>
    class SelectingState : SelectionStateBase
    {
        private readonly HashSet<CircuitObject> additiveSelections;

        /// <summary>
        /// Mouse selection box
        /// </summary>
        private readonly BoxCollider mouseBox;

        public SelectingState(Vec2 position, HashSet<CircuitObject> additiveSelections) : base(new HashSet<CircuitObject>())
        {
            this.additiveSelections = additiveSelections;
            selections.UnionWith(additiveSelections);
            mouseBox = new BoxCollider(position.X, position.Y, 0, 0);


        }

        public override void Draw(BoardState state, PainterScope painter)
        {
            BoxCollider newBox = mouseBox.GetNormalized();
            painter.DrawRectangle(Color.FromArgb(255, 0, 0, 255), 3, newBox.Position, newBox.Bounds);
            painter.FillRectangle(Color.FromArgb(64, 128, 128, 255), newBox.Position, newBox.Bounds);

            base.Draw(state, painter);
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {

            mouseBox.Width = stateControls.LocalMousePosition.X - mouseBox.X;
            mouseBox.Height = stateControls.LocalMousePosition.Y - mouseBox.Y;

            //Update selection box and load intersections into selections
            mouseBox.GetNormalized().GetIntersections(stateControls.State, true, out _, out var newSelections);
            selections.Clear();
            selections.UnionWith(newSelections);
            selections.UnionWith(additiveSelections);
            stateControls.CircuitPropertiesOutput = GetUpdatedCircuitProperties(stateControls.RegisterChange);
            return (true, this);
        }

        public override InputReturns MouseLeftUp(StateControls stateControls)
        {
            return (true, new SelectionToolState(selections));
        }
    }
}
