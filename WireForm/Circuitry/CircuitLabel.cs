using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.CircuitAttributes.Utils.StringValidators;
using Wireform.Circuitry.Data;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry
{
    /// <summary>
    /// A text box on the circuit board.
    /// </summary>
    public class CircuitLabel : BoardObject
    {
        [JsonIgnore]
        private Vec2 size = Vec2.Zero;

        [JsonIgnore]
        public override BoxCollider HitBox => new BoxCollider(StartPoint.X, StartPoint.Y, size.X, size.Y);

        public CircuitLabel(Vec2 StartPoint)
        {
            Gridded = false;
            this.StartPoint = StartPoint;
        }

        [CircuitPropertyText(typeof(StringValidators.NotNullOrEmpty))]
        public string Text { get; set; } = "|";


        [CircuitPropertyDropdown(1, 10, false)]
        public int Scale { get; set; } = 4;

        public override void Delete(BoardState state)
        {
            state.Extras.Remove(this);
        }

        public override async Task Draw(PainterScope painter, BoardState state)
        {
            await painter.DrawString(Text, Color.Black, StartPoint, Scale / 10f);

            //update hitbox
            size = MathHelper.ViewportToLocalPoint(await painter.MeasureString(Text, Scale / 10f), painter.Zoom);
        }

        public override BoardObject Copy()
        {
            return new CircuitLabel(StartPoint) { Text = Text, Scale = Scale };
        }
    }
}
