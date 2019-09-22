using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Gates;

namespace WireForm
{
    public class SaveManager
    {
        public static void Save(string path, FlowPropogator propogator)
        {
            string output = JsonConvert.SerializeObject(propogator, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                }) ;
            Debug.WriteLine(output);
            File.WriteAllText(path, output);
        }
        
        public static void Load(string json, out FlowPropogator propogator)
        {
            propogator = JsonConvert.DeserializeObject<FlowPropogator>(json, 
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                });
            propogator.Connections = new Dictionary<Vec2, List<CircuitConnector>>();
            for (int i = 0; i < propogator.wires.Count; i++)
            {
                if (propogator.wires[i].StartPoint.Y == propogator.wires[i].EndPoint.Y)
                {
                    propogator.wires[i] = new WireLine(propogator.wires[i].StartPoint, propogator.wires[i].EndPoint, true);
                }
                WireLine.AddConnections(propogator.wires[i], propogator.Connections);
            }

            foreach(Gate gate in propogator.gates)
            {
                foreach(GatePin input in gate.Inputs)
                {
                    input.Parent = gate;
                    input.LocalPoint = input.LocalPoint;
                }
                foreach(GatePin output in gate.Outputs)
                {
                    output.Parent = gate;
                    output.LocalPoint = output.LocalPoint;
                }
                gate.AddConnections(propogator.Connections);
            }
        }
    }
}
