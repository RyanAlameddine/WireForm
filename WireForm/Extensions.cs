using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    public static class Extensions
    {
        public static void AddConnection(this Dictionary<Point, List<CircuitConnector>> connections, CircuitConnector connector)
        {
            if (!connections.ContainsKey(connector.StartPoint))
            {
                connections.Add(connector.StartPoint, new List<CircuitConnector>());
            }
            connections[connector.StartPoint].Add(connector);
        }

        
    }
}
