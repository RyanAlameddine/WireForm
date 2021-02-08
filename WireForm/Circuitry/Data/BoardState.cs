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
        public Dictionary<Vec2, List<DrawableObject>> Connections { get; set; }

        /// <summary>
        /// A Union of the Wires, Gates, and Extras collections
        /// </summary>
        [JsonIgnore]
        public IEnumerable<BoardObject> BoardObjects
        {
            get => ((IEnumerable<BoardObject>)Wires).Union(Gates).Union(Extras);
        }

        public List<WireLine> Wires { get; set; }
        public List<Gate> Gates { get; set; }
        public List<BoardObject> Extras { get; set; }
        public BoardState()
        {
            Connections = new Dictionary<Vec2, List<DrawableObject>>();
            Wires = new List<WireLine>();
            Gates = new List<Gate>();
            Extras = new List<BoardObject>();
        }

        public void Propogate()
        {
            Queue<Gate> sources = new Queue<Gate>();
            foreach (Gate gate in Gates)
            {
                if (gate.Inputs.Length == 0)
                {
                    sources.Enqueue(gate);
                }
            }
            FlowPropagator.PropagateBits(this, sources);
        }

        public BoardState Copy()
        {
            BoardState state = new BoardState();

            foreach (WireLine wire in Wires)
            {
                WireLine newWire = (WireLine)wire.Copy();
                state.Wires.Add(newWire);
                newWire.AddConnections(state.Connections);

            }

            foreach (Gate gate in Gates)
            {
                Gate newGate = (Gate)gate.Copy();
                state.Gates.Add(newGate);
                newGate.AddConnections(state.Connections);
            }

            foreach (BoardObject obj in Extras)
            {
                BoardObject newObj = obj.Copy();
                state.Extras.Add(newObj);
            }

            return state;
        }
    }
}
