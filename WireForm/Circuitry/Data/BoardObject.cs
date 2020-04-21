using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.MathUtils;

namespace Wireform.Circuitry.Data
{
    /// <summary>
    /// An object which sits on the circuit board
    /// </summary>
    public abstract class BoardObject
    {
        public abstract Vec2 StartPoint { get; set; }

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
