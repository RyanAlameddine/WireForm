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

            Stack<WireLine> visitedWires = new Stack<WireLine>();
            visitedWires.Push(null);
            visitedWires.Push(null);

            /// Gate and the amount of times it has been 'visited' 
            /// (how many times an input has been updated)  
            /// and whether or not it's last visit was a pushover 
            /// (pushing to back of queue to check again if requirements are fulfilled)
            Dictionary<Gate, int> visitedGates = new Dictionary<Gate, int>();

            for (var source = sources.Peek(); sources.Count > 0;)
            {
                source = sources.Dequeue();

                int visitCount = 1;
                if (visitedGates.ContainsKey(source))
                {
                    visitCount = ++visitedGates[source];
                }
                else
                {
                    visitedGates.Add(source, visitCount);
                        
                }
                

                //if(visitCount == source.Inputs.Length)
                //{

                //}
                //else if(visitCount > source.Inputs.Length)
                //{
                //    //throw new Exception();
                //}
                //else
                //{
                //    visitedGates[source] = (!visitedGates[source].pushover, visitedGates[source].visited);
                //    sources.Enqueue(source);
                //}//ALLOW VISITED WIRES TO BE RE-PROCESSED INSTEAD OF BEING FORCED TO ONLY VISIT ONCE (ERROR IF INFINITE LOOP)
                

                //Compute and Propogate
                source.Compute();
                foreach (var output in source.Outputs)
                {
                    List<Gate> changedGates = new List<Gate>();
                    PropagateWires(state, visitedWires, output.StartPoint, output.Value);
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
                        if (!visitedGates.ContainsKey(gate))
                        {
                            foreach (GatePin input in gate.Inputs)
                            {
                                input.Value = BitValue.Nothing;
                            }
                            foreach (GatePin output in gate.Outputs)
                            {
                                output.Value = BitValue.Nothing;
                            }
                        }
                    }
                }
            }
        }

        static void PropagateWires(BoardState state, Stack<WireLine> visitedWires, Vec2 startPoint, BitValue value)
        {
            if (!state.Connections.ContainsKey(startPoint))
            {
                return;
            }

            foreach (BoardObject circuitObject in state.Connections[startPoint])
            {
                WireLine wire = circuitObject as WireLine;
                if (wire != null)
                {
                    var topWire = visitedWires.Peek();
                    if (visitedWires.Contains(wire))
                    {
                        continue;
                    }

                    int stackPointer = visitedWires.Count - 1;
                    visitedWires.Push(wire);
                    

                    wire.Data.bitValue = value;
                    if (wire.StartPoint == startPoint)
                    {
                        PropagateWires(state, visitedWires, wire.EndPoint, value);
                    }
                    else if (wire.EndPoint == startPoint)
                    {
                        PropagateWires(state, visitedWires, wire.StartPoint, value);
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
                            //changedGates.Add(pin.Parent);
                        }
                    }

                    continue;
                }

                throw new NotImplementedException();

            }
        }


        static void PropogateWire(BoardState state, Stack<WireLine> visitedWires, List<Gate> changedGates, Vec2 position, BitValue value)
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
                    var topWire = visitedWires.Peek();
                    if (topWire == wire)
                    {
                        continue;
                    }

                    int stackPointer = visitedWires.Count - 1;
                    visitedWires.Push(wire);
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
