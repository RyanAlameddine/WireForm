using System;
using System.Collections.Generic;
using System.Linq;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Data
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

        [CircuitAction("Delete", System.Windows.Forms.Keys.Delete)]
        public abstract void Delete(BoardState state);
    }
}
