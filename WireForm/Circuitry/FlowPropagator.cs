using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.MathUtils;

namespace WireForm.Circuitry
{
    public static class FlowPropagator
    {
        /// <summary>
        /// Computes each source and propogates down wires from sources
        /// After doing that, it visits each unvisited gate and propogates down wires from their outputs
        /// </summary>
        public static void Propogate(BoardState state, Queue<Gate> sources)
        {
            if (sources == null || sources.Count == 0)
            {
                return;
            }

            Debug.WriteLine("Propogate");

            bool exhausted = false;

            HashSet<WireLine> visitedWires = new HashSet<WireLine>();
            /// Gate and the amount of times it has been 'visited' (how many times an input has been updated)
            Dictionary<Gate, int> visitedGates = new Dictionary<Gate, int>();
            for (var source = sources.Peek(); sources.Count > 0;)
            {
                source = sources.Dequeue();

                if (visitedGates.ContainsKey(source))
                {
                    //THIS CHECK MIGHT NEED TO BE CHANGED
                    if (visitedGates[source] > source.Inputs.Length)
                    {
                        continue;
                    }
                    visitedGates[source]++;
                }
                else
                {
                    visitedGates.Add(source, 1);
                }

                //Compute and Propogate
                source.Compute();
                foreach (var output in source.Outputs)
                {
                    List<Gate> changedGates = new List<Gate>();
                    PropogateWire(state, visitedWires, changedGates, output.StartPoint, output.Value);
                    foreach (Gate gate in changedGates)
                    {
                        sources.Enqueue(gate);
                    }
                }

                //Check for 'error' wires and 'nothing' wires
                if (sources.Count == 0 && !exhausted)
                {
                    exhausted = true;
                    foreach (WireLine wireLine in state.wires)
                    {
                        if (!visitedWires.Contains(wireLine))
                        {
                            wireLine.Data.bitValue = BitValue.Nothing;
                        }
                    }

                    foreach (Gate gate in state.gates)
                    {
                        if (!visitedGates.ContainsKey(gate) || visitedGates[gate] < gate.Inputs.Length)
                        {
                            foreach (GatePin input in gate.Inputs)
                            {
                                input.Value = BitValue.Nothing;
                            }
                            sources.Enqueue(gate);
                        }
                    }
                }
            }


        }

        static void PropogateWire(BoardState state, HashSet<WireLine> visitedWires, List<Gate> changedGates, Vec2 position, BitValue value)
        {
            if (!state.Connections.ContainsKey(position))
            {
                return;
            }

            foreach (BoardObject circuitObject in state.Connections[position])
            {
                WireLine wire = circuitObject as WireLine;
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
                        PropogateWire(state, visitedWires, changedGates, wire.EndPoint, value);
                    }
                    else if (wire.EndPoint == position)
                    {
                        PropogateWire(state, visitedWires, changedGates, wire.StartPoint, value);
                    }
                    else
                    {
                        throw new Exception("How tf did this happen");
                    }

                    continue;
                }

                GatePin pin = circuitObject as GatePin;
                if (pin != null)
                {
                    if (pin.Parent.Inputs != null)
                    {
                        if (pin.Parent.Inputs.Contains(pin))
                        {
                            pin.Value = value;
                            changedGates.Add(pin.Parent);
                        }
                    }

                    continue;
                }

                throw new NotImplementedException();

            }
        }
    }
}
