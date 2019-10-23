using System;
using System.Collections.Generic;
using System.Linq;
using WireForm.Circuitry.CircuitObjectActions;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry
{
    /// <summary>
    /// An object which sits on the board and interacts with other objects on the board
    /// </summary>
    public abstract class CircuitObject : BoardObject, IDeletable
    {
        public abstract BoxCollider HitBox { get; set; }

        public abstract void AddConnections(Dictionary<Vec2, List<BoardObject>> connections);
        
        public abstract void RemoveConnections(Dictionary<Vec2, List<BoardObject>> connections);
        public abstract CircuitObject Copy();

        public abstract void Delete(BoardState state);
    }
}
