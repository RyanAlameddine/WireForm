using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WireForm.Circuitry;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.MathUtils;

namespace WireForm
{
    public class SaveManager
    {
        public static void Save(string path, BoardState propogator)
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
        
        public static void Load(string json, out BoardState propogator)
        {
            propogator = JsonConvert.DeserializeObject<BoardState>(json, 
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                });
            propogator.Connections = new Dictionary<Vec2, List<BoardObject>>();
            for (int i = 0; i < propogator.wires.Count; i++)
            {
                if (propogator.wires[i].StartPoint.Y == propogator.wires[i].EndPoint.Y)
                {
                    propogator.wires[i] = new WireLine(propogator.wires[i].StartPoint, propogator.wires[i].EndPoint, true);
                }
                propogator.wires[i].AddConnections(propogator.Connections);
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
