using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;
using Wireform.Utils;

namespace Wireform.Circuitry
{
    public static class FlowPropagator
    {
#if DEBUG
        /// <summary>
        /// Debug function to call between each step of the circuit
        /// </summary>
        public static Action DebugStep;
#endif

        /// <summary>
        /// Computes each source and propogates down wires from sources (breadth-first search), 
        /// computing all gates found along the way.
        /// </summary>
        public static void PropogateBits(BoardState state, Queue<Gate> sources)
        {
            Debug.WriteLine("propogated");
            //Copy sources into currentGates
            Queue<Gate> gateQueue = new Queue<Gate>(sources);
#if DEBUG
            gateQueue.Enqueue(null);
#endif

            //All gates which have been visited at least once
            HashSet<Gate> visitedGates = new HashSet<Gate>();
            //All wires which have been visited at least once
            HashSet<WireLine> visitedWires = new HashSet<WireLine>();

            //All input gatePins which have been visited at least once
            HashSet<GatePin> visitedInputs = new HashSet<GatePin>();

            //Stores the last 25% of circuit objects which have been visited
            //This is used if there is an overflow and the propogator is stuck oscillating
            //All objects contained here will have their values set to ERROR
            WrappingArray<BoardObject> updatedObjects = new WrappingArray<BoardObject>(GlobalSettings.PropogationRepetitionOverflow / 4);

            //Each iteration represents one propogation
            for (int i = 0; gateQueue.Count != 0; i++)
            {
                Gate currentGate = gateQueue.Dequeue();
#if DEBUG
                if(currentGate == null) { DebugStep?.Invoke(); if(!(gateQueue.Count == 0)) gateQueue.Enqueue(null); continue; }
#endif

                currentGate.ComputeGate();
                visitedGates.Add(currentGate);

                foreach (GatePin output in currentGate.Outputs)
                {
                    output.Values.CopyTo(out var values);
                    PropogateDownPoint(output.StartPoint, values, state, gateQueue, visitedInputs, visitedWires, updatedObjects);
                }

                //infinite oscillation catch:
                if (i > GlobalSettings.PropogationRepetitionOverflow)
                {
                    foreach (BoardObject boardObject in updatedObjects)
                    {
                        if (boardObject is WireLine wire) wire.Values.SetAll(BitValue.Error);
                        //else if (boardObject is GatePin pin) pin.Values.SetAll(BitValue.Error);
                    }
                    Debug.WriteLine("Infinite oscillation caught!");
                    break;
                }
            }

            //Set unvisited wires to Nothing
            foreach (WireLine wire in state.wires.Where((wire) => !visitedWires.Contains(wire)))
            {
                wire.Values.SetAll(BitValue.Nothing);
            }

            //Sets unvisited pins to Nothing
            foreach (GatePin pin in state.gates.SelectMany((gate) => gate.Inputs).Where((pin) => !visitedInputs.Contains(pin)))
            {
                pin.Values.SetAll(BitValue.Nothing);
            }
        }

        /// <summary>
        /// Propogates down wires instantly from a point, and adds gates to gateQueue
        /// </summary>
        private static void PropogateDownPoint(Vec2 point, BitArray values, BoardState state, Queue<Gate> gateQueue, HashSet<GatePin> visitedInputs, HashSet<WireLine> visitedWires, WrappingArray<BoardObject> updatedObjects)
        {
            HashSet<WireLine> newWires = new HashSet<WireLine>();
            RecursivePropogate(newWires, point);
            visitedWires.UnionWith(newWires);
            return;

            //inner recursive function which builds up the local visited wires collection
            void RecursivePropogate(HashSet<WireLine> vWires, Vec2 position)
            {
                List<BoardObject> boardObjects = state.Connections[position];
                foreach (var boardObject in boardObjects)
                {
                    if (boardObject is GatePin pin)
                    {
                        //if pin is an output pin, nothing to be done
                        if (pin.Parent.Outputs.Contains(pin)) continue;

                        //if pin has been visited and the input value has not changed, nothing to be done
                        if (pin.Values == values && visitedInputs.Contains(pin)) continue;

                        visitedInputs.Add(pin);
                        updatedObjects.Add(pin);

                        //update pin values and add to queue
                        values.CopyTo(out var copiedVal);
                        pin.Values = copiedVal;

                        //Note to self: if something isn't working and it makes no sense and I have no clue why:
                        //It's probably because this check is wrong
                        //if (gateQueue.Contains(pin.Parent)) 
                        //{
                        //    var removed = gateQueue.Where((x) => x != pin.Parent);
                        //    gateQueue = new Queue<Gate>(removed);
                        //}
                        gateQueue.Enqueue(pin.Parent);
                    }
                    else if (boardObject is WireLine wire)
                    {
                        //wire has already been visited this propogation
                        if (vWires.Contains(wire)) continue;

                        values.CopyTo(out var copiedVal);
                        wire.Values = copiedVal;
                        vWires.Add(wire);
                        updatedObjects.Add(wire);
                        //if this point is the start of the wire, return the end and vice versa
                        Vec2 otherPoint = position == wire.StartPoint ? wire.EndPoint : wire.StartPoint;
                        RecursivePropogate(vWires, otherPoint);
                    }
                }
            }
        }
    }
}
