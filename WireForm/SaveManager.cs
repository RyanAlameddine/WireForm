using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.MathUtils;

namespace Wireform
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
        
        public static void Load(string json, out BoardState state)
        {
            state = JsonConvert.DeserializeObject<BoardState>(json, 
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                });
            state.Connections = new Dictionary<Vec2, List<BoardObject>>();
            for (int i = 0; i < state.wires.Count; i++)
            {
                if (state.wires[i].StartPoint.Y == state.wires[i].EndPoint.Y)
                {
                    state.wires[i] = new WireLine(state.wires[i].StartPoint, state.wires[i].EndPoint, true);
                }
                state.wires[i].AddConnections(state.Connections);
            }

            foreach(Gate gate in state.gates)
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
                gate.AddConnections(state.Connections);
            }
        }
    }
}
