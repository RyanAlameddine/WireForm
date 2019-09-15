using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using WireForm.Gates;

namespace WireForm
{
    public class FlowPropogator
    {
        public Dictionary<Point, List<CircuitConnector>> Connections { get; set; }
        public FlowPropogator()
        {
            Connections = new Dictionary<Point, List<CircuitConnector>>();
        }

        /// <summary>
        /// Computes each source and propogates down wires from sources
        /// </summary>
        public void Propogate(Queue<Gate> sources, List<WireLine> wires)
        {
            if(sources == null || sources.Count == 0)
            {
                return;
            }

            HashSet<WireLine> visitedWires = new HashSet<WireLine>();
            HashSet<Gate> visitedGates = new HashSet<Gate>();
            for (var source = sources.Peek(); sources.Count > 0; )
            {
                sources.Dequeue();

                if (visitedGates.Contains(source))
                {
                    continue;
                }
                visitedGates.Add(source);

                //Compute and Proopogate
                source.Compute(null);
                foreach (var output in source.Outputs)
                {
                    List<Gate> changedGates = new List<Gate>();
                    PropogateWire(visitedWires, changedGates, MathHelper.Plus(output.StartPoint, source.Position), output.Value);
                    foreach(Gate gate in changedGates)
                    {
                        sources.Enqueue(gate);
                    }
                }
            }

            foreach(WireLine wireLine in wires)
            {
                if (!visitedWires.Contains(wireLine))
                {
                    wireLine.Data.bitValue = BitValue.Nothing;
                }
            }
        }

        void PropogateWire(HashSet<WireLine> visitedWires, List<Gate> changedGates, Point position, BitValue value)
        {
            if (!Connections.ContainsKey(position))
            {
                return;
            }

            foreach(CircuitConnector connector in Connections[position])
            {
                WireLine wire = connector as WireLine;
                if (wire != null)
                {
                    if (visitedWires.Contains(wire))
                    {
                        continue;
                    }
                    visitedWires.Add(wire);
                    wire.Data.bitValue = value;
                    if (wire.StartPoint == position)
                    {
                        PropogateWire(visitedWires, changedGates, wire.EndPoint, value);
                    }
                    else if (wire.EndPoint == position)
                    {
                        PropogateWire(visitedWires, changedGates, wire.StartPoint, value);
                    }
                    else
                    {
                        throw new Exception("How tf did this happen");
                    }

                    continue;
                }

                GatePin pin = connector as GatePin;
                if (pin != null)
                {
                    changedGates.Add(pin.Parent);

                    continue;
                }

                throw new NotImplementedException();
                
            }
        }
    }
}
