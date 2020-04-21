using System;
using System.Collections.Generic;
using System.Linq;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Data
{
    /// <summary>
    /// An object which sits on the board and interacts with other objects on the board
    /// </summary>
    public abstract class CircuitObject : BoardObject
    {

        public abstract BoxCollider HitBox { get; }

        public abstract void AddConnections(Dictionary<Vec2, List<BoardObject>> connections);

        public abstract void RemoveConnections(Dictionary<Vec2, List<BoardObject>> connections);
        public abstract CircuitObject Copy();

        [CircuitAction("Delete", 'x')]
        public abstract void Delete(BoardState state);
    }
}
