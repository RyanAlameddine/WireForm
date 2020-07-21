using System.Collections.Generic;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;
using Wireform.Utils;

namespace Wireform
{
    public static class Extensions
    {
        /// <summary>
        /// Adds BoardObject to connections
        /// </summary>
        public static void Attach(this Dictionary<Vec2, List<DrawableObject>> connections, DrawableObject boardObject)
        {
            if (!connections.ContainsKey(boardObject.StartPoint))
            {
                connections.Add(boardObject.StartPoint, new List<DrawableObject>());
            }
            connections[boardObject.StartPoint].Add(boardObject);
        }

        /// <summary>
        /// Removes BoardObject from connections
        /// </summary>
        public static void Detatch(this Dictionary<Vec2, List<DrawableObject>> connections, DrawableObject boardObject)
        {
            connections[boardObject.StartPoint].Remove(boardObject);
        }

        /// <summary>
        /// Removes all mentioned CircuitObjects from state.wires and state.gate
        /// and removes connections for all of those circuitObjects
        /// </summary>
        public static void DetatchAll(this BoardState state, HashSet<BoardObject> circuitObjects)
        {
            foreach (BoardObject circuitObject in circuitObjects)
            {
                if (circuitObject is WireLine wire)
                {
                    state.Wires.Remove(wire);
                    wire.RemoveConnections(state.Connections);
                }
                else if (circuitObject is Gate gate)
                {
                    state.Gates.Remove(gate);
                    gate.RemoveConnections(state.Connections);
                }
            }
        }

        /// <summary>
        /// Puts all mentioned CircuitObjects from state.wires and state.gate
        /// and adds connections for all of those circuitObjects
        /// </summary>
        public static void AttachAll(this BoardState state, HashSet<BoardObject> circuitObjects)
        {
            List<WireLine> toAdd = new List<WireLine>();
            List<WireLine> toRemove = new List<WireLine>();
            foreach (BoardObject circuitObject in circuitObjects)
            {
                if (circuitObject is WireLine wire)
                {
                    List<WireLine> newWires = wire.InsertAndAttach(state.Wires, state.Connections);
                    toAdd.AddRange(newWires);
                    toRemove.Add(wire);
                }
                else if (circuitObject is Gate gate)
                {
                    state.Gates.Add(gate);
                    gate.AddConnections(state.Connections);
                }
            }

            foreach (var wire in toRemove) circuitObjects.Remove(wire);
            foreach (var wire in toAdd   ) circuitObjects.Add   (wire);
        }

        /// <summary>
        /// Gets drawing multiplier information from direction and flipped bool
        /// </summary>
        public static (int xMult, int yMult, bool flipXY) GetMultiplier(this Direction direction)
        {
            return direction switch
            {
                Direction.Up    => (-1,  1, true ),
                Direction.Down  => ( 1, -1, true ),
                Direction.Left  => (-1, -1, false),
                Direction.Right => ( 1,  1, false),
                //All X values flipped
                Direction.UpFlipped    => ( 1,  1, true ),
                Direction.DownFlipped  => (-1, -1, true ),
                Direction.LeftFlipped  => ( 1, -1, false),
                Direction.RightFlipped => (-1,  1, false),
                _ => throw new System.Exception("How did I get here"),
            };
        }

        public static string GetHotkeyString(this char key, Modifier modifiers)
        {
            string hotkey;
            if (modifiers == Modifier.None) hotkey = key + "";
            else hotkey = $"{modifiers.ToString().Replace(", ", "+")}+{key}";
            return hotkey;
        }
    }
}
