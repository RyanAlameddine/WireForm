using System.Collections.Generic;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.MathUtils;

namespace Wireform
{
    public static class Extensions
    {
        /// <summary>
        /// Adds BoardObject to connections
        /// </summary>
        public static void Attach(this Dictionary<Vec2, List<BoardObject>> connections, BoardObject boardObject)
        {
            if (!connections.ContainsKey(boardObject.StartPoint))
            {
                connections.Add(boardObject.StartPoint, new List<BoardObject>());
            }
            connections[boardObject.StartPoint].Add(boardObject);
        }

        /// <summary>
        /// Removes BoardObject from connections
        /// </summary>
        public static void Detatch(this Dictionary<Vec2, List<BoardObject>> connections, BoardObject boardObject)
        {
            connections[boardObject.StartPoint].Remove(boardObject);
        }

        /// <summary>
        /// Removes all mentioned CircuitObjects from state.wires and state.gate
        /// and removes connections for all of those circuitObjects
        /// </summary>
        public static void DetatchAll(this BoardState state, HashSet<CircuitObject> circuitObjects)
        {
            foreach (CircuitObject circuitObject in circuitObjects)
            {
                if (circuitObject is WireLine wire) state.wires.Remove(wire);
                else if (circuitObject is Gate gate) state.gates.Remove(gate);
                circuitObject.RemoveConnections(state.Connections);
            }
        }

        /// <summary>
        /// Puts all mentioned CircuitObjects from state.wires and state.gate
        /// and adds connections for all of those circuitObjects
        /// </summary>
        public static void AttachAll(this BoardState state, HashSet<CircuitObject> circuitObjects)
        {
            List<WireLine> toAdd = new List<WireLine>();
            List<WireLine> toRemove = new List<WireLine>();
            foreach (CircuitObject circuitObject in circuitObjects)
            {
                if (circuitObject is WireLine wire)
                {
                    List<WireLine> newWires = wire.InsertAndAttach(state.wires, state.Connections);
                    toAdd.AddRange(newWires);
                    toRemove.Add(wire);
                }
                else if (circuitObject is Gate gate)
                {
                    state.gates.Add(gate);
                    circuitObject.AddConnections(state.Connections);
                }
            }

            foreach (var wire in toRemove) circuitObjects.Remove(wire);
            foreach (var wire in toAdd   ) circuitObjects.Add   (wire);
        }

        public static Vec2 GetMultiplier(this Direction direction)
        {
            return direction switch
            {
                Direction.Up    => new Vec2(-1, -1),
                Direction.Down  => new Vec2( 1, -1),
                Direction.Left  => new Vec2(-1,  1),
                Direction.Right => new Vec2( 1,  1),
                _ => throw new System.Exception("How did I get here"),
            };
        }

        public static void SetDepth(this GatePin[] pins, int bitDepth)
        {
            for(int i = 0; i < pins.Length; i++)
            {
                pins[i].Values = new BitArray(bitDepth);
            }
        }
    }
}
