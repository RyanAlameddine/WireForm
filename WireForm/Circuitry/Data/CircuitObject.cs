using System;
using System.Collections.Generic;
using System.Linq;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Data
{
    /// <summary>
    /// An object which sits on the board and interacts with other objects on the board.
    /// E.g. Gate, WireLine
    /// </summary>
    public abstract class CircuitObject : BoardObject
    {

        public abstract void AddConnections(Dictionary<Vec2, List<DrawableObject>> connections);

        public abstract void RemoveConnections(Dictionary<Vec2, List<DrawableObject>> connections);
    }
}
