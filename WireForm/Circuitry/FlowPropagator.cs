using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utilities;
using Wireform.MathUtils;

namespace Wireform.Circuitry
{
    public 
        class FlowPropagator
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
            HashSet<GatePin> visitedPins = new HashSet<GatePin>();

            while (sources.Count > 0)
            {
                var source = sources.Dequeue();
                
                //Compute and Propogate
                source.Compute();
                foreach (var output in source.Outputs)
                {
                    PropagateWires(state, patternStack, visitedWires, visitedPins, output.StartPoint, output.Values);
                }
            }
            foreach (WireLine wire in state.wires)
            {
                if (!visitedWires.Contains(wire))
                {
                    for (int i = 0; i < wire.Data.BitValues.Length; i++)
                    {
                        wire.Data.BitValues[i] = BitValue.Nothing;
                    }
                }
            }
            foreach (Gate gate in state.gates)
            {
                foreach (GatePin pin in gate.Inputs)
                {
                    if (!visitedPins.Contains(pin))
                    {
                        for (int i = 0; i < pin.Values.Length; i++)
                        {
                            pin.Values.Set(i, BitValue.Nothing);
                        }
                    }
                }
            }
        }

        static void PropagateWires(BoardState state, PatternStack<WireValuePair> patternStack, HashSet<WireLine> visitedWires, HashSet<GatePin> visitedPins, Vec2 startPoint, BitArray values)
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

                    values.CopyTo(out var copiedData);
                    wire.Data = copiedData;
                    //wire.Data = values;

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
                                for (int i = 0; i < node.Wire.Data.Length; i++)
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
                            PropagateWires(state, patternStack, visitedWires, visitedPins, wire.EndPoint, values);
                            patternStack.Pop(patternStack.HeadIndex - index);
                        }
                        else if (wire.EndPoint == startPoint)
                        {
                            PropagateWires(state, patternStack, visitedWires, visitedPins, wire.StartPoint, values);
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
                            visitedPins.Add(pin);
                            pin.Values = values;

                            pin.Parent.Compute();
                            foreach (GatePin output in pin.Parent.Outputs)
                            {
                                PropagateWires(state, patternStack, visitedWires, visitedPins, output.StartPoint, output.Values);
                            }
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
        public BitArray Values;

        public WireValuePair(WireLine wire, BitArray values)
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
