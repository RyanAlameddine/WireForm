using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    public struct WireLine
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        [JsonIgnore]
        public bool XPriority { get; set; }

        public WireLine(Point start, Point end, bool XPriority)
        {
            Start = start;
            End = end;
            this.XPriority = XPriority;
        }

        //TODO ADD THE ABILITY TO BREAK LINES
        public void Validate(List<WireLine> wires, Dictionary<Point, List<WireLine>> connections)
        {
            wires.Remove(this);

            for (int i = 0; i < wires.Count; i++)
            {
                if (!MathHelper.OnLine(this, wires[i]))
                {
                    //If wirestart is contained in wires[i], split wires[i] into two wires
                    if (MathHelper.ContainedIn(Start, wires[i]))
                    {
                        WireLine wire1 = new WireLine(Start, wires[i].End, wires[i].XPriority);
                        WireLine wire2 = new WireLine(wires[i].Start, Start, wires[i].XPriority);
                        //Both wires exist
                        if(wire1.Start != wire1.End && wire2.Start != wire2.End)
                        {
                            RemoveConnections(wires[i], connections);

                            wires[i] = wire1;
                            wires.Add(wire2);

                            AddConnections(wires[i], connections);
                            AddConnections(wires[wires.Count - 1], connections);
                        }
                    }
                    //If wireend is contained in wires[i], split wires[i] into two wires
                    else if (MathHelper.ContainedIn(End, wires[i]))
                    {
                        WireLine wire1 = new WireLine(End, wires[i].End, wires[i].XPriority);
                        WireLine wire2 = new WireLine(wires[i].Start, End, wires[i].XPriority);
                        //Both wires exist
                        if (wire1.Start != wire1.End && wire2.Start != wire2.End)
                        {
                            RemoveConnections(wires[i], connections);

                            wires[i] = wire1;
                            wires.Add(wire2);

                            AddConnections(wires[i], connections);
                            AddConnections(wires[wires.Count - 1], connections);
                        }
                    }

                    //If wires[i].wirestart is contained in this wire, split this wire into two wires
                    if (MathHelper.ContainedIn(wires[i].Start, this))
                    {
                        WireLine wire1 = new WireLine(wires[i].Start, End, XPriority);
                        WireLine wire2 = new WireLine(Start, wires[i].Start, XPriority);
                        //Both wires exist
                        if (wire1.Start != wire1.End && wire2.Start != wire2.End)
                        {
                            wires.Add(wire1);
                            wire1.Validate(wires, connections);
                            wires.Add(wire2);
                            wire2.Validate(wires, connections);
                            return;
                        }
                    }
                    //If wires[i].wireend is contained in this wire, split this wire into two wires
                    if (MathHelper.ContainedIn(wires[i].End, this))
                    {
                        WireLine wire1 = new WireLine(wires[i].End, End, XPriority);
                        WireLine wire2 = new WireLine(Start, wires[i].End, XPriority);
                        //Both wires exist
                        if (wire1.Start != wire1.End && wire2.Start != wire2.End)
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
                if (Start.ContainedIn(wires[i]))
                {
                    //If both points of this wire are contained in the target wire
                    if (End.ContainedIn(wires[i]))
                    {
                        return;
                    }
                    //Else

                    //Create a match case
                    Point temp = Start;
                    Start = wires[i].Start;
                    if (checkMatchCases(connections, wires, i))
                    {
                        return;
                    }
                    Start = temp;
                    continue;
                }
                if (End.ContainedIn(wires[i]))
                {
                    //Create a match case
                    Point temp = End;
                    End = wires[i].End;
                    if (checkMatchCases(connections, wires, i))
                    {
                        return;
                    }
                    End = temp;
                    continue;
                }

                //Cases where the target wire is contained in this wire
                if (wires[i].Start.ContainedIn(this) && wires[i].End.ContainedIn(this))
                {
                    /*
                    RemoveConnections(wires[i], connections);
                    wires[i] = this;
                    wires[i].Validate(wires, connections);
                    return;
                    */
                    //Split wire in two and validate both sides
                    int startDist = MathHelper.ManhattanDistance(wires[i].Start, Start);
                    int endDist = MathHelper.ManhattanDistance(wires[i].End, Start);

                    if(startDist < endDist)
                    {
                        WireLine toStart = new WireLine(Start, wires[i].Start, wires[i].XPriority);
                        WireLine toEnd = new WireLine(End, wires[i].End, wires[i].XPriority);

                        wires.Add(toStart);
                        toStart.Validate(wires, connections);
                        wires.Add(toEnd);
                        toEnd.Validate(wires, connections);
                    }
                    else
                    {
                        WireLine toStart = new WireLine(End, wires[i].Start, wires[i].XPriority);
                        WireLine toEnd = new WireLine(Start, wires[i].End, wires[i].XPriority);

                        wires.Add(toStart);
                        toStart.Validate(wires, connections);
                        wires.Add(toEnd);
                        toEnd.Validate(wires, connections);
                    }
                    return;
                }
            }
            wires.Add(this);
            AddConnections(wires[wires.Count - 1], connections);
        }

        private bool checkMatchCases(Dictionary<Point, List<WireLine>> connections, List<WireLine> wires, int i)
        {
            if (wires[i].Start == Start)
            {
                return runCases(Start, End, wires[i].Start, wires[i].End, connections, wires, i);
            }
            if (wires[i].Start == End)
            {
                return runCases(End, Start, wires[i].Start, wires[i].End, connections, wires, i);
            }

            if (wires[i].End == End)
            {
                return runCases(End, Start, wires[i].End, wires[i].Start, connections, wires, i);
            }
            if (wires[i].End == Start)
            {
                return runCases(Start, End, wires[i].End, wires[i].Start, connections, wires, i);
            }
            return false;
        }

        /// <param name="eqThis">Point on this wire that is equal to eqThat</param>
        /// <param name="chkThis">Point on this wire that may or may not be equal to chkThat</param>
        /// <param name="eqThat">Point on taret wire that is equal to eqThis</param>
        /// <param name="chkThat">Point on target wire that may or may not be equal to chkThis</param>
        private bool runCases(Point eqThis, Point chkThis, Point eqThat, Point chkThat, Dictionary<Point, List<WireLine>> connections, List<WireLine> wires, int i)
        {
            //Wires are the same
            if (chkThat == chkThis)
            {
                return true;
            }

            //This wire is completely contained in that wire
            if (chkThis.ContainedIn(wires[i]))
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
        public static void AddConnections(WireLine wire, Dictionary<Point, List<WireLine>> connections)
        {
            if (!connections.ContainsKey(wire.Start))
            {
                connections[wire.Start] = new List<WireLine>();
            }
            if (!connections.ContainsKey(wire.End))
            {
                connections[wire.End] = new List<WireLine>();
            }

            connections[wire.Start].Add(wire);
            connections[wire.End  ].Add(wire);
        }

        /// <summary>
        /// Remove wire from connections
        /// </summary>
        public static void RemoveConnections(WireLine wire, Dictionary<Point, List<WireLine>> connections)
        {
            connections[wire.Start].Remove(wire);
            connections[wire.End].Remove(wire);
        }
    }
}
