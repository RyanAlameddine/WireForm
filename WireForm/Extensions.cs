using System.Collections.Generic;
using WireForm.Circuitry.Data;
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
    }
}
