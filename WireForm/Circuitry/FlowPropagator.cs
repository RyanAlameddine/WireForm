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

            PatternStack<WireValuePair> patternStack = new PatternStack<WireValuePair>();
            HashSet<WireLine> visitedWires = new HashSet<WireLine>();

            while (sources.Count > 0)
            {
                var source = sources.Dequeue();
                
                //Compute and Propogate
                source.Compute();
                foreach (var output in source.Outputs)
                {
                    PropagateWires(state, patternStack, visitedWires, output.StartPoint, output.Values);
                }
            }
        }

        static void PropagateWires(BoardState state, PatternStack<WireValuePair> patternStack, HashSet<WireLine> visitedWires, Vec2 startPoint, BitValue[] values)
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
                    if (!patternStack.IsEmpty() && patternStack.Peek().Wire == wire)
                    {
                        continue;
                    }
                    var index = patternStack.HeadIndex;
                    bool patternMatched = patternStack.Push(new WireValuePair(wire, values));
                    visitedWires.Add(wire);
                    wire.Data.BitValues = values;

                    if (patternMatched)
                    {
                        //Debug.WriteLine("Matched");

                        var matchedNode = patternStack.matchedStartNode;
                        bool allMatch = true;
                        foreach (var node in patternStack.CurrentPattern)
                        {
                            if(node.Values != matchedNode.Value.Values)
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
                            foreach (var node in patternStack.CurrentPattern)
                            {
                                for (int i = 0; i < node.Wire.Data.BitValues.Length; i++)
                                {
                                    node.Wire.Data.BitValues[i] = BitValue.Error;
                                }
                            }
                            return;
                        }
                    }
                    else
                    {

                        if (wire.StartPoint == startPoint)
                        {
                            PropagateWires(state, patternStack, visitedWires, wire.EndPoint, values);
                            patternStack.Pop(patternStack.HeadIndex - index);
                        }
                        else if (wire.EndPoint == startPoint)
                        {
                            PropagateWires(state, patternStack, visitedWires, wire.StartPoint, values);
                            patternStack.Pop(patternStack.HeadIndex - index);
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
                            pin.Values = values;

                            pin.Parent.Compute();
                            foreach (GatePin output in pin.Parent.Outputs)
                            {
                                PropagateWires(state, patternStack, visitedWires, output.StartPoint, output.Values);
                            }
                        }
                    }

                    continue;
                }

                throw new NotImplementedException();

            }

            foreach(WireLine wire in state.wires)
            {
                if (!visitedWires.Contains(wire))
                {
                    for(int i = 0; i < wire.Data.BitValues.Length; i++)
                    {
                        wire.Data.BitValues[i] = BitValue.Nothing;
                    }
                }
            }
        }


        static void PropogateWire(BoardState state, Stack<WireLine> visitedWires, List<Gate> changedGates, Vec2 position, BitValue[] value)
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
                    wire.Data.BitValues = value;
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
                            pin.Values = value;
                            changedGates.Add(pin.Parent);
                        }
                    }

                    continue;
                }

                throw new NotImplementedException();

            }
        }
    }

    internal class WireValuePair
    {
        public WireLine Wire;
        public BitValue[] Values;

        public WireValuePair(WireLine wire, BitValue[] values)
        {
            Wire = wire;
            Values = values;
        }

        public override string ToString()
        {
            return Wire.ToString();
        }

        public override bool Equals(object obj)
        {
            WireValuePair pair = obj as WireValuePair;
            if(pair == null)
            {
                return false;
            }
            return (Wire.StartPoint == pair.Wire.StartPoint) && (Wire.EndPoint == pair.Wire.EndPoint);
        }

        public override int GetHashCode()
        {
            return 684914040 + EqualityComparer<WireLine>.Default.GetHashCode(Wire);
        }
    }
}
