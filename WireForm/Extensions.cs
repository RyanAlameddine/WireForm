using System.Collections.Generic;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.MathUtils;

namespace WireForm
{
    public static class Extensions
    {
        public static void AddConnection(this Dictionary<Vec2, List<BoardObject>> connections, BoardObject circuitObject)
        {
            if (!connections.ContainsKey(circuitObject.StartPoint))
            {
                connections.Add(circuitObject.StartPoint, new List<BoardObject>());
            }
            connections[circuitObject.StartPoint].Add(circuitObject);
        }

        public static void RemoveConnection(this Dictionary<Vec2, List<BoardObject>> connections, BoardObject circuitObject)
        {
            connections[circuitObject.StartPoint].Remove(circuitObject);
        }

        public static Vec2 GetMultiplier(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Vec2(-1, -1);
                case Direction.Down:
                    return new Vec2(1, -1);
                case Direction.Left:
                    return new Vec2(-1, 1);
                case Direction.Right:
                    return new Vec2(1, 1);
                default:
                    throw new System.Exception("How did I get here");
            }
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
