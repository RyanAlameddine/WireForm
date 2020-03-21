using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Input.States.Selection
{
    /// <summary>
    /// The state in which a selection box is being drawn.
    /// </summary>
    class SelectingState : SelectionStateBase
    {
        private readonly List<CircuitObject> additiveSelections;

        private readonly BoxCollider mouseBox;

        public SelectingState(Vec2 position, List<CircuitObject> additiveSelections)
        {
            this.additiveSelections = additiveSelections;
            mouseBox = new BoxCollider(position.X, position.Y, 0, 0);
        }

        public override void Draw(BoardState state, PainterScope painter)
        {
            BoxCollider newBox = mouseBox.GetNormalized();
            painter.DrawRectangle(Color.FromArgb(255, 0, 0, 255), 3, newBox.Position, newBox.Bounds);
            painter.FillRectangle(Color.FromArgb(64, 128, 128, 255), newBox.Position, newBox.Bounds);

            base.Draw(state, painter);
        }

        public override InputReturns MouseMove(Form1 form, Vec2 mousePosition)
        {
            Vec2 localPoint = LocalPoint(mousePosition);

            mouseBox.Width = localPoint.X - mouseBox.X;
            mouseBox.Height = localPoint.Y - mouseBox.Y;

            mouseBox.GetNormalized().GetIntersections(form.stateStack.CurrentState, true, out _, out var selections);
            selections.AddRange(additiveSelections);
            this.selections = selections;
            return (true, this);
        }

        public override InputReturns MouseUp(Form1 form, Vec2 mousePosition)
        {
            return (true, new SelectionToolState(selections));
        }
    }
}
