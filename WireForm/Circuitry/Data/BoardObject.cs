using System;
using System.Collections.Generic;
using System.Text;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Data
{
    /// <summary>
    /// An object which sits on the circuit board and can be interacted with
    /// E.g. WireLine, Gate
    /// </summary>
    public abstract class BoardObject : DrawableObject
    {
        public abstract BoxCollider HitBox { get; }

        public abstract BoardObject Copy();

        [CircuitAction("Delete", 'x')]
        public abstract void Delete(BoardState state);
    }
}
