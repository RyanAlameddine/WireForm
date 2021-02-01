using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;

namespace Wireform.Circuitry.Data
{
    /// <summary>
    /// An object which sits on the circuit board but does not directly interact with it
    /// E.g. GatePin
    /// </summary>
    public abstract class DrawableObject
    {
        public virtual Vec2 StartPoint { get; set; }

        /// <summary>
        /// True if the object's position must conform to the grid lines
        /// </summary>
        public bool Gridded { get; protected set; } = true;

        public abstract Task Draw(PainterScope painter, BoardState state);

        /// <summary>
        /// Move BoardObject so that StartPoint = newPosition;
        /// In the case of WireLines, will also update EndPoint
        /// </summary>
        public virtual void SetPosition(Vec2 position)
        {
            StartPoint = position;
        }

        /// <summary>
        /// Offsets the BoardObject so that StartPoint = StartPoint + offset;
        /// In the case of WireLines, will also update EndPoint
        /// </summary>
        public void OffsetPosition(Vec2 offset)
        {
            SetPosition(StartPoint + offset);
        }
    }
}
