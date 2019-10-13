using System;
using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry;
using WireForm.MathUtils;

namespace WireForm
{
    public static class Extensions
    {
        public static void AddConnection(this Dictionary<Vec2, List<CircuitConnector>> connections, CircuitConnector connector)
        {
            if (!connections.ContainsKey(connector.StartPoint))
            {
                connections.Add(connector.StartPoint, new List<CircuitConnector>());
            }
            connections[connector.StartPoint].Add(connector);
        }

        public static void RemoveConnection(this Dictionary<Vec2, List<CircuitConnector>> connections, CircuitConnector connector)
        {
            connections[connector.StartPoint].Remove(connector);
        }

        public static void AddRange<T>(this HashSet<T> set1, HashSet<T> set2)
        {
            foreach(var t in set2)
            {
                set1.Add(t);
            }
        }

        public static Color BitColor(this BitValue value)
        {
            switch (value)
            {
                case BitValue.Error:
                    return Color.DarkRed;
                case BitValue.Nothing:
                    return Color.DimGray;
                case BitValue.One:
                    return Color.Blue;
                case BitValue.Zero:
                    return Color.DarkBlue;
            }
            throw new NullReferenceException();
        }
    }
}
