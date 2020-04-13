using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly HashSet<CircuitObject> additiveSelections;

        private readonly BoxCollider mouseBox;

        public SelectingState(Vec2 position, HashSet<CircuitObject> additiveSelections) : base(additiveSelections)
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

        public override InputReturns MouseMove(InputControls inputControls)
        {
            Vec2 localPoint = MathHelper.ViewportToLocalPoint(inputControls.MousePosition);
            //Debug.WriteLine($"{mouseBox.Position}, {localPoint}");

            mouseBox.Width = localPoint.X - mouseBox.X;
            mouseBox.Height = localPoint.Y - mouseBox.Y;


            mouseBox.GetNormalized().GetIntersections(inputControls.State, true, out _, out var newSelections);
            selections.Clear();
            selections.UnionWith(newSelections);
            selections.UnionWith(additiveSelections);
            return (true, this);
        }

        public override InputReturns MouseLeftUp(InputControls inputControls)
        {
            return (true, new SelectionToolState(selections));
        }
    }
}
