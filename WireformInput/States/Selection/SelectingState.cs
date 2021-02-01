using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
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
        private readonly HashSet<BoardObject> additiveSelections;

        /// <summary>
        /// Mouse selection box
        /// </summary>
        private readonly BoxCollider mouseBox;

        public SelectingState(Vec2 position, HashSet<BoardObject> additiveSelections) : base(new HashSet<BoardObject>())
        {
            this.additiveSelections = additiveSelections;
            selections.UnionWith(additiveSelections);
            mouseBox = new BoxCollider(position.X, position.Y, 0, 0);
        }

        public override async Task Draw(BoardState state, PainterScope painter)
        {
            BoxCollider newBox = mouseBox.GetNormalized();
            await painter.DrawRectangle(Color.FromArgb(255, 0, 0, 255), 3, newBox.Position, newBox.Bounds);
            await painter.FillRectangle(Color.FromArgb(64, 128, 128, 255), newBox.Position, newBox.Bounds);

            await base.Draw(state, painter);
        }

        public override InputReturns MouseMove(StateControls stateControls)
        {
            mouseBox.Width  = stateControls.LocalMousePosition.X - mouseBox.X;
            mouseBox.Height = stateControls.LocalMousePosition.Y - mouseBox.Y;

            //Update selection box and load intersections into selections
            mouseBox.GetNormalized().GetIntersections(stateControls.State, (true, true, true), out _, out var newSelections);
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
