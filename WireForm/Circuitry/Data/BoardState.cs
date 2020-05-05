using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;

namespace Wireform.Circuitry.Data
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

        public void Propogate()
        {
            Queue<Gate> sources = new Queue<Gate>();
            foreach (Gate gate in gates)
            {
                if (gate.Inputs.Length == 0)
                {
                    sources.Enqueue(gate);
                }
            }
            FlowPropagator.PropogateBits(this, sources);
        }

        public BoardState Copy()
        {
            BoardState state = new BoardState();

            foreach (WireLine wire in wires)
            {
                WireLine newWire = (WireLine)wire.Copy();
                state.wires.Add(newWire);
                newWire.AddConnections(state.Connections);

            }

            foreach (Gate gate in gates)
            {
                Gate newGate = (Gate)gate.Copy();
                state.gates.Add(newGate);
                newGate.AddConnections(state.Connections);
            }
            return state;
        }
    }
}
