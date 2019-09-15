using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    public class SaveManager
    {
        public static void Save(string path, List<WireLine> wires)
        {
            string output = JsonConvert.SerializeObject(wires);
            Debug.WriteLine(output);
            File.WriteAllText(path, output);
        }
        
        public static void Load(string json, out Dictionary<Point, List<CircuitConnector>> connections, out List<WireLine> wires)
        {
            wires = JsonConvert.DeserializeObject<List<WireLine>>(json);
            connections = new Dictionary<Point, List<CircuitConnector>>();
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].StartPoint.Y == wires[i].EndPoint.Y)
                {
                    wires[i] = new WireLine(wires[i].StartPoint, wires[i].EndPoint, true);
                }
                WireLine.AddConnections(wires[i], connections);
            }
        }
    }
}
