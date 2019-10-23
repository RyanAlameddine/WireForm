using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.MathUtils;

namespace WireForm.Circuitry
{
    public class BoardState
    {
        [JsonIgnore]
        public Dictionary<Vec2, List<BoardObject>> Connections { get; set; }
        public List<WireLine> wires { get; set; }
        public List<Gate> gates { get; set; }
        public BoardState()
        {
            Connections = new Dictionary<Vec2, List<BoardObject>>();
            wires = new List<WireLine>();
            gates = new List<Gate>();
        }

        public BoardState Copy()
        {
            BoardState state = new BoardState();
            foreach (KeyValuePair<Vec2, List<BoardObject>> pair in Connections)
            {
                List<BoardObject> copiedObjects = new List<BoardObject>();
                foreach(BoardObject obj in pair.Value)
                {
                    var circuit = obj as CircuitObject;
                    if(circuit == null)
                    {
                        circuit = ((GatePin)obj).Parent;
                    }

                    copiedObjects.Add(circuit.Copy());
                }
                state.Connections.Add(pair.Key, copiedObjects);
            }

            foreach (WireLine wire in wires)
            {
                state.wires.Add((WireLine) wire.Copy());
            }

            foreach (Gate gate in gates)
            {
                state.gates.Add((Gate) gate.Copy());
            }
            return state;
        }
    }
}
