using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;

namespace Wireform
{
    internal static class SaveManager
    {
        public static string Serialize(BoardState state)
        {
            string output = JsonConvert.SerializeObject(state, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                }) ;
            return output;
        }
        
        public static void Load(string json, out BoardState state)
        {
            state = JsonConvert.DeserializeObject<BoardState>(json, 
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                });
            state.Connections = new Dictionary<Vec2, List<DrawableObject>>();
            for (int i = 0; i < state.Wires.Count; i++)
            {
                if (state.Wires[i].StartPoint.Y == state.Wires[i].EndPoint.Y)
                {
                    state.Wires[i] = new WireLine(state.Wires[i].StartPoint, state.Wires[i].EndPoint, true);
                }
                state.Wires[i].AddConnections(state.Connections);
            }

            foreach(Gate gate in state.Gates)
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
