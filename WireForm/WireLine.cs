using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    public class WireLine : CircuitConnector
    {
        public override Vec2 StartPoint { get; set; }
        public Vec2 EndPoint { get; set; }

        [JsonIgnore]
        public bool XPriority { get; set; }

        /// <summary>
        /// Data related to the flow of electricity
        /// </summary>
        public WireData Data { get; set; }

        public WireLine(Vec2 start, Vec2 end, bool XPriority)
        {
            StartPoint = start;
            EndPoint = end;
            this.XPriority = XPriority;
            Data = new WireData(1);
        }

        public void Validate(List<WireLine> wires, Dictionary<Vec2, List<CircuitConnector>> connections)
        {
            wires.Remove(this);
            bool fullyContained = false;

            if (StartPoint == EndPoint)
            {
                //Check to see if this is a request to dot two wires together
                for(int i = 0; i < wires.Count; i++)
                {
                    if (StartPoint.IsContainedIn(wires[i]))
                    {
                        WireLine wire1 = new WireLine(StartPoint, wires[i].EndPoint, wires[i].XPriority);
                        WireLine wire2 = new WireLine(wires[i].StartPoint, StartPoint, wires[i].XPriority);
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            RemoveConnections(wires[i], connections);
                            wires[i] = wire1;
                            wires[i].Validate(wires, connections);
                            wires.Add(wire2);
                            wires[wires.Count - 1].Validate(wires, connections);
                        }
                        return;
                    }
                }
                return;
            }

            for (int i = 0; i < wires.Count; i++)
            {
                if (!MathHelper.OnLine(this, wires[i]))
                {
                    //If wirestart is contained in wires[i], split wires[i] into two wires
                    if (MathHelper.IsContainedIn(StartPoint, wires[i]))
                    {
                        WireLine wire1 = new WireLine(StartPoint, wires[i].EndPoint, wires[i].XPriority);
                        WireLine wire2 = new WireLine(wires[i].StartPoint, StartPoint, wires[i].XPriority);
                        //Both wires exist
                        if(wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            RemoveConnections(wires[i], connections);

                            //Temporarily add this wire as a validated wire, then split wires[i] and validate both splits
                            //before removing the temporary wire and revalidating this

                            AddConnections(this, connections);
                            wires.Add(this);
                            wires[i] = wire1;
                            wires[i].Validate(wires, connections);
                            wires.Add(wire2);
                            wires[wires.Count - 1].Validate(wires, connections);
                            wires.Remove(this);
                            RemoveConnections(this, connections);

                            this.Validate(wires, connections);
                            return;
                        }
                    }
                    //If wireend is contained in wires[i], split wires[i] into two wires
                    else if (MathHelper.IsContainedIn(EndPoint, wires[i]))
                    {
                        WireLine wire1 = new WireLine(EndPoint, wires[i].EndPoint, wires[i].XPriority);
                        WireLine wire2 = new WireLine(wires[i].StartPoint, EndPoint, wires[i].XPriority);
                        //Both wires exist
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            RemoveConnections(wires[i], connections);

                            //Temporarily add this wire as a validated wire, then split wires[i] and validate both splits
                            //before removing the temporary wire and revalidating this

                            AddConnections(this, connections);
                            wires.Add(this);
                            wires[i] = wire1;
                            wires[i].Validate(wires, connections);
                            wires.Add(wire2);
                            wires[wires.Count - 1].Validate(wires, connections);
                            wires.Remove(this);
                            RemoveConnections(this, connections);

                            this.Validate(wires, connections);
                            return;
                        }
                    }

                    //If wires[i].wirestart is contained in this wire, split this wire into two wires
                    if (MathHelper.IsContainedIn(wires[i].StartPoint, this))
                    {
                        WireLine wire1 = new WireLine(wires[i].StartPoint, EndPoint, XPriority);
                        WireLine wire2 = new WireLine(StartPoint, wires[i].StartPoint, XPriority);
                        //Both wires exist
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            wires.Add(wire1);
                            wire1.Validate(wires, connections);
                            wires.Add(wire2);
                            wire2.Validate(wires, connections);
                            return;
                        }
                    }
                    //If wires[i].wireend is contained in this wire, split this wire into two wires
                    if (MathHelper.IsContainedIn(wires[i].EndPoint, this))
                    {
                        WireLine wire1 = new WireLine(wires[i].EndPoint, EndPoint, XPriority);
                        WireLine wire2 = new WireLine(StartPoint, wires[i].EndPoint, XPriority);
                        //Both wires exist
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            wires.Add(wire1);
                            wire1.Validate(wires, connections);
                            wires.Add(wire2);
                            wire2.Validate(wires, connections);
                            return;
                        }
                    }

                    continue;
                }

                //Cases where an start/end point matches with the target wire
                if (checkMatchCases(connections, wires, i))
                {
                    return;
                }

                //Cases where this wire is contained in the target wire
                if (StartPoint.IsContainedIn(wires[i]))
                {
                    //If both points of this wire are contained in the target wire
                    if (EndPoint.IsContainedIn(wires[i]))
                    {
                        fullyContained = true;
                        continue;
                    }
                    //Else

                    //Create a match case
                    Vec2 temp = StartPoint;
                    StartPoint = wires[i].StartPoint;
                    if (checkMatchCases(connections, wires, i))
                    {
                        return;
                    }
                    StartPoint = temp;
                    continue;
                }
                if (EndPoint.IsContainedIn(wires[i]))
                {
                    //Create a match case
                    Vec2 temp = EndPoint;
                    EndPoint = wires[i].EndPoint;
                    if (checkMatchCases(connections, wires, i))
                    {
                        return;
                    }
                    EndPoint = temp;
                    continue;
                }

                //Cases where the target wire is contained in this wire
                if (wires[i].StartPoint.IsContainedIn(this) && wires[i].EndPoint.IsContainedIn(this))
                {
                    //Split wire in two and validate both sides
                    int startDist = MathHelper.ManhattanDistance(wires[i].StartPoint, StartPoint);
                    int endDist = MathHelper.ManhattanDistance(wires[i].EndPoint, StartPoint);

                    if(startDist < endDist)
                    {
                        WireLine toStart = new WireLine(StartPoint, wires[i].StartPoint, wires[i].XPriority);
                        WireLine toEnd = new WireLine(EndPoint, wires[i].EndPoint, wires[i].XPriority);

                        wires.Add(toStart);
                        toStart.Validate(wires, connections);
                        wires.Add(toEnd);
                        toEnd.Validate(wires, connections);
                    }
                    else
                    {
                        WireLine toStart = new WireLine(EndPoint, wires[i].StartPoint, wires[i].XPriority);
                        WireLine toEnd = new WireLine(StartPoint, wires[i].EndPoint, wires[i].XPriority);

                        wires.Add(toStart);
                        toStart.Validate(wires, connections);
                        wires.Add(toEnd);
                        toEnd.Validate(wires, connections);
                    }
                    return;
                }
            }
            if (fullyContained)
            {
                return;
            }

            wires.Add(this);
            AddConnections(wires[wires.Count - 1], connections);
        }

        private bool checkMatchCases(Dictionary<Vec2, List<CircuitConnector>> connections, List<WireLine> wires, int i)
        {
            if (wires[i].StartPoint == StartPoint)
            {
                return runCases(StartPoint, EndPoint, wires[i].StartPoint, wires[i].EndPoint, connections, wires, i);
            }
            if (wires[i].StartPoint == EndPoint)
            {
                return runCases(EndPoint, StartPoint, wires[i].StartPoint, wires[i].EndPoint, connections, wires, i);
            }

            if (wires[i].EndPoint == EndPoint)
            {
                return runCases(EndPoint, StartPoint, wires[i].EndPoint, wires[i].StartPoint, connections, wires, i);
            }
            if (wires[i].EndPoint == StartPoint)
            {
                return runCases(StartPoint, EndPoint, wires[i].EndPoint, wires[i].StartPoint, connections, wires, i);
            }
            return false;
        }

        /// <param name="eqThis">Point on this wire that is equal to eqThat</param>
        /// <param name="chkThis">Point on this wire that may or may not be equal to chkThat</param>
        /// <param name="eqThat">Point on taret wire that is equal to eqThis</param>
        /// <param name="chkThat">Point on target wire that may or may not be equal to chkThis</param>
        private bool runCases(Vec2 eqThis, Vec2 chkThis, Vec2 eqThat, Vec2 chkThat, Dictionary<Vec2, List<CircuitConnector>> connections, List<WireLine> wires, int i)
        {
            //Wires are the same
            if (chkThat == chkThis)
            {
                return true;
            }

            //This wire is completely contained in that wire
            if (chkThis.IsContainedIn(wires[i]))
            {
                return true;
            }

            //This wire extends out of that wire
            if (MathHelper.ManhattanDistance(chkThis, eqThat) < MathHelper.ManhattanDistance(chkThis, chkThat))
            {
                if(connections[eqThat].Count > 1)
                {
                    //wires.Add(this);
                    //AddConnections(wires[wires.Count - 1], connections);
                    return false;
                }
                RemoveConnections(wires[i], connections);
                wires[i] = new WireLine(chkThis, chkThat, wires[i].XPriority);
            }
            else
            {
                if (connections[chkThat].Count > 1)
                {
                    //wires.Add(this);
                    //AddConnections(wires[wires.Count - 1], connections);
                    return false;
                }
                RemoveConnections(wires[i], connections);
                wires[i] = new WireLine(chkThis, eqThat, wires[i].XPriority);
            }
            wires[i].Validate(wires, connections);
            return true;
        }

        /// <summary>
        /// Add wire to connections
        /// </summary>
        public static void AddConnections(WireLine wire, Dictionary<Vec2, List<CircuitConnector>> connections)
        {
            if (!connections.ContainsKey(wire.StartPoint))
            {
                connections[wire.StartPoint] = new List<CircuitConnector>();
            }
            if (!connections.ContainsKey(wire.EndPoint))
            {
                connections[wire.EndPoint] = new List<CircuitConnector>();
            }

            connections[wire.StartPoint].Add(wire);
            connections[wire.EndPoint  ].Add(wire);
        }

        /// <summary>
        /// Remove wire from connections
        /// </summary>
        public static void RemoveConnections(WireLine wire, Dictionary<Vec2, List<CircuitConnector>> connections)
        {
            connections[wire.StartPoint].Remove(wire);
            connections[wire.EndPoint].Remove(wire);
        }

        public static void RemovePointFromWire(Vec2 point, Dictionary<Vec2, List<CircuitConnector>> connections, List<WireLine> wires, int i)
        {
            RemoveConnections(wires[i], connections);
            Vec2 initialStart = wires[i].StartPoint;
            Vec2 initialEnd = wires[i].EndPoint;

            Vec2 startsEnd;
            Vec2 endsStart;
            var xpri = wires[i].XPriority;
            if (xpri)
            {
                if(wires[i].StartPoint.X > wires[i].EndPoint.X)
                {
                    startsEnd = point.Plus(new Vec2( 1, 0));
                    endsStart = point.Plus(new Vec2(-1, 0));
                }
                else
                {
                    startsEnd = point.Plus(new Vec2(-1, 0));
                    endsStart = point.Plus(new Vec2( 1, 0));
                }
            }
            else
            {
                if (wires[i].StartPoint.Y > wires[i].EndPoint.Y)
                {
                    startsEnd = point.Plus(new Vec2(0,  1));
                    endsStart = point.Plus(new Vec2(0, -1));
                }
                else
                {
                    startsEnd = point.Plus(new Vec2(0, -1));
                    endsStart = point.Plus(new Vec2(0,  1));
                }
            }

            if (point == wires[i].StartPoint)
            {
                wires[i] = new WireLine(endsStart, wires[i].EndPoint, wires[i].XPriority);
                wires[i].Validate(wires, connections);
            }
            else if (point == wires[i].EndPoint)
            {
                wires[i] = new WireLine(wires[i].StartPoint, startsEnd, wires[i].XPriority);
                wires[i].Validate(wires, connections);
            }
            else
            {
                var temp = wires[i].StartPoint;
                wires[i] = new WireLine(endsStart, wires[i].EndPoint, wires[i].XPriority);
                wires[i].Validate(wires, connections);
                WireLine newWire = new WireLine(temp, startsEnd, xpri);
                wires.Add(newWire);
                newWire.Validate(wires, connections);
            }
        }
    }
}
