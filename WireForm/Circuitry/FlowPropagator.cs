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

            //bool exhausted = false;

            PatternStack<WireLine> visitedWires = new PatternStack<WireLine>();

            while (sources.Count > 0)
            {
                var source = sources.Dequeue();
                
                //Compute and Propogate
                source.Compute();
                foreach (var output in source.Outputs)
                {
                    PropagateWires(state, visitedWires, output.StartPoint, output.Value);
                }

                //Check for 'error' wires and 'nothing' wires
                //if (sources.Count == 0 && !exhausted)
                //{
                //    exhausted = true;
                //    foreach (WireLine wireLine in state.wires)
                //    {
                //        if (!visitedWires.Contains(wireLine))
                //        {
                //            wireLine.Data.bitValue = BitValue.Nothing;
                //        }
                //    }

                //    foreach (Gate gate in state.gates)
                //    {
                //        if (!visitedGates.ContainsKey(gate))
                //        {
                //            foreach (GatePin input in gate.Inputs)
                //            {
                //                input.Value = BitValue.Nothing;
                //            }
                //            foreach (GatePin output in gate.Outputs)
                //            {
                //                output.Value = BitValue.Nothing;
                //            }
                //        }
                //    }
                //}
            }
        }

        static void PropagateWires(BoardState state, PatternStack<WireLine> visitedWires, Vec2 startPoint, BitValue value)
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
                    if (visitedWires.Peek() == wire)
                    {
                        continue;
                    }
                    var index = visitedWires.HeadIndex;
                    bool patternMatched = visitedWires.Push(wire); F//FIX THIS BECAUSE IF VALUES ARE DIFFERENT WILL NOT RECOGNIZE PATTERN
                    wire.Data.bitValue = value;

                    if (patternMatched)
                    {
                        Debug.WriteLine("Matched");

                        var matchedNode = visitedWires.matchedStartNode;
                        bool allMatch = true;
                        foreach (var node in visitedWires.CurrentPattern)
                        {
                            if(node.value != matchedNode.Value.value)
                            {
                                allMatch = false;
                                break;
                            }

                            matchedNode = matchedNode.Next;
                        }

                        if (allMatch)
                        {
                            return;
                        }
                        else
                        {
                            foreach (var node in visitedWires.CurrentPattern)
                            {
                                node.line.Data.bitValue = BitValue.Error;
                            }
                            return;
                        }
                    }
                    else
                    {

                        if (wire.StartPoint == startPoint)
                        {
                            PropagateWires(state, visitedWires, wire.EndPoint, value);
                            visitedWires.Pop(visitedWires.HeadIndex - index);
                        }
                        else if (wire.EndPoint == startPoint)
                        {
                            PropagateWires(state, visitedWires, wire.StartPoint, value);
                            visitedWires.Pop(visitedWires.HeadIndex - index);
                        }
                        else
                        {
                            throw new Exception("How tf did this happen");
                        }
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

                            pin.Parent.Compute();
                            foreach (GatePin output in pin.Parent.Outputs)
                            {
                                PropagateWires(state, visitedWires, output.StartPoint, output.Value);
                            }
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
