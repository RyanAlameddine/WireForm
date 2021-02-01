using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Data.Bits;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry
{
    public sealed class WireLine : CircuitObject
    {
        public override Vec2 StartPoint { get; set; }
        public Vec2 EndPoint { get; set; }

        [JsonIgnore]
        public override BoxCollider HitBox
        {
            get
            {
                BoxCollider collider = new BoxCollider(StartPoint.X, StartPoint.Y, EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y).GetNormalized();
                if (IsHorizontal)
                {
                    collider.Height = .4f;
                    collider.Y -= .2f;
                }
                else
                {
                    collider.Width = .4f;
                    collider.X -= .2f;
                }

                return collider;
            }
        }

        [JsonIgnore]
        public bool IsHorizontal { get; set; }

        [JsonIgnore]
        public BitArray Values { get; set; }

        public WireLine(Vec2 start, Vec2 end, bool IsHorizontal)
        {
            StartPoint = start;
            EndPoint = end;
            this.IsHorizontal = IsHorizontal;
            Values = new BitArray(1);
        }

        /// <summary>
        /// Inserts wire into given list of wires, accounting for many cases such as a wire contained inside another wire,
        /// a wire which intersects other wires, a wire which is an extension of another wire, etc.
        /// At the end, it returns a list of references to the newly created wires.
        /// Note: The original wire this function was run on might not end up being added into the wires list depending on the case,
        /// so please only refer to the list of WireLines which are returned.
        /// </summary>
        public List<WireLine> InsertAndAttach(List<WireLine> wires, Dictionary<Vec2, List<DrawableObject>> connections)
        {
            List<WireLine> createdWires = new List<WireLine>();
            wires.Remove(this);
            bool fullyContained = false;

            if (StartPoint == EndPoint)
            {
                //Check to see if this is a request to dot two wires together
                for(int i = 0; i < wires.Count; i++)
                {
                    if (StartPoint.IsContainedIn(wires[i]))
                    {
                        WireLine wire1 = new WireLine(StartPoint, wires[i].EndPoint, wires[i].IsHorizontal);
                        WireLine wire2 = new WireLine(wires[i].StartPoint, StartPoint, wires[i].IsHorizontal);
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            wires[i].RemoveConnections(connections);
                            wires[i] = wire1;
                            var new1 = wires[i].InsertAndAttach(wires, connections);
                            wires.Add(wire2);
                            var new2 = wires[wires.Count - 1].InsertAndAttach(wires, connections);

                            createdWires.AddRange(new1);
                            createdWires.AddRange(new2);
                        }
                        else
                        {
                            //Refresh wire by tapping on it
                            wires[i].RemoveConnections(connections);
                            createdWires.AddRange(wires[i].InsertAndAttach(wires, connections));
                        }
                        return createdWires;
                    }
                }
                return createdWires;
            }

            for (int i = 0; i < wires.Count; i++)
            {
                if (!MathHelper.OnLine(this, wires[i]))
                {
                    //If wirestart is contained in wires[i], split wires[i] into two wires
                    if (MathHelper.IsContainedIn(StartPoint, wires[i]))
                    {
                        WireLine wire1 = new WireLine(StartPoint, wires[i].EndPoint, wires[i].IsHorizontal);
                        WireLine wire2 = new WireLine(wires[i].StartPoint, StartPoint, wires[i].IsHorizontal);
                        //Both wires exist
                        if(wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            wires[i].RemoveConnections(connections);

                            //Temporarily add this wire as a validated wire, then split wires[i] and validate both splits
                            //before removing the temporary wire and revalidating this

                            AddConnections(connections);
                            wires.Add(this);
                            wires[i] = wire1;
                            createdWires.AddRange(wires[i].InsertAndAttach(wires, connections));
                            wires.Add(wire2);
                            createdWires.AddRange(wires[wires.Count - 1].InsertAndAttach(wires, connections));
                            wires.Remove(this);
                            RemoveConnections(connections);

                            createdWires.AddRange(this.InsertAndAttach(wires, connections));
                            return createdWires;
                        }
                    }
                    //If wireend is contained in wires[i], split wires[i] into two wires
                    else if (MathHelper.IsContainedIn(EndPoint, wires[i]))
                    {
                        WireLine wire1 = new WireLine(EndPoint, wires[i].EndPoint, wires[i].IsHorizontal);
                        WireLine wire2 = new WireLine(wires[i].StartPoint, EndPoint, wires[i].IsHorizontal);
                        //Both wires exist
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            wires[i].RemoveConnections(connections);

                            //Temporarily add this wire as a validated wire, then split wires[i] and validate both splits
                            //before removing the temporary wire and revalidating this

                            AddConnections(connections);
                            wires.Add(this);
                            wires[i] = wire1;
                            createdWires.AddRange(wires[i].InsertAndAttach(wires, connections));
                            wires.Add(wire2);
                            createdWires.AddRange(wires[wires.Count - 1].InsertAndAttach(wires, connections));
                            wires.Remove(this);
                            RemoveConnections(connections);

                            createdWires.AddRange(this.InsertAndAttach(wires, connections));
                            return createdWires;
                        }
                    }

                    //If wires[i].wirestart is contained in this wire, split this wire into two wires
                    if (MathHelper.IsContainedIn(wires[i].StartPoint, this))
                    {
                        WireLine wire1 = new WireLine(wires[i].StartPoint, EndPoint, IsHorizontal);
                        WireLine wire2 = new WireLine(StartPoint, wires[i].StartPoint, IsHorizontal);
                        //Both wires exist
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            wires.Add(wire1);
                            createdWires.AddRange(wire1.InsertAndAttach(wires, connections));
                            wires.Add(wire2);
                            createdWires.AddRange(wire2.InsertAndAttach(wires, connections));
                            return createdWires;
                        }
                    }
                    //If wires[i].wireend is contained in this wire, split this wire into two wires
                    if (MathHelper.IsContainedIn(wires[i].EndPoint, this))
                    {
                        WireLine wire1 = new WireLine(wires[i].EndPoint, EndPoint, IsHorizontal);
                        WireLine wire2 = new WireLine(StartPoint, wires[i].EndPoint, IsHorizontal);
                        //Both wires exist
                        if (wire1.StartPoint != wire1.EndPoint && wire2.StartPoint != wire2.EndPoint)
                        {
                            wires.Add(wire1);
                            createdWires.AddRange(wire1.InsertAndAttach(wires, connections));
                            wires.Add(wire2);
                            createdWires.AddRange(wire2.InsertAndAttach(wires, connections));
                            return createdWires;
                        }
                    }

                    continue;
                }

                //Cases where an start/end point matches with the target wire
                if (checkMatchCases(connections, wires, i, createdWires))
                {
                    return createdWires;
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
                    if (checkMatchCases(connections, wires, i, createdWires))
                    {
                        return createdWires;
                    }
                    StartPoint = temp;
                    continue;
                }
                if (EndPoint.IsContainedIn(wires[i]))
                {
                    //Create a match case
                    Vec2 temp = EndPoint;
                    EndPoint = wires[i].EndPoint;
                    if (checkMatchCases(connections, wires, i, createdWires))
                    {
                        return createdWires;
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
                        WireLine toStart = new WireLine(StartPoint, wires[i].StartPoint, wires[i].IsHorizontal);
                        WireLine toEnd = new WireLine(EndPoint, wires[i].EndPoint, wires[i].IsHorizontal);

                        wires.Add(toStart);
                        createdWires.AddRange(toStart.InsertAndAttach(wires, connections));
                        wires.Add(toEnd);
                        createdWires.AddRange(toEnd.InsertAndAttach(wires, connections));
                    }
                    else
                    {
                        WireLine toStart = new WireLine(EndPoint, wires[i].StartPoint, wires[i].IsHorizontal);
                        WireLine toEnd = new WireLine(StartPoint, wires[i].EndPoint, wires[i].IsHorizontal);

                        wires.Add(toStart);
                        createdWires.AddRange(toStart.InsertAndAttach(wires, connections));
                        wires.Add(toEnd);
                        createdWires.AddRange(toEnd.InsertAndAttach(wires, connections));
                    }
                    return createdWires;
                }
            }
            if (fullyContained)
            {
                return createdWires;
            }

            wires.Add(this);
            wires[wires.Count - 1].AddConnections(connections);
            createdWires.Add(this);
            return createdWires;
        }

        private bool checkMatchCases(Dictionary<Vec2, List<DrawableObject>> connections, List<WireLine> wires, int i, List<WireLine> createdWires)
        {
            if (wires[i].StartPoint == StartPoint)
            {
                return runCases(StartPoint, EndPoint, wires[i].StartPoint, wires[i].EndPoint, connections, wires, i, createdWires);
            }
            if (wires[i].StartPoint == EndPoint)
            {
                return runCases(EndPoint, StartPoint, wires[i].StartPoint, wires[i].EndPoint, connections, wires, i, createdWires);
            }

            if (wires[i].EndPoint == EndPoint)
            {
                return runCases(EndPoint, StartPoint, wires[i].EndPoint, wires[i].StartPoint, connections, wires, i, createdWires);
            }
            if (wires[i].EndPoint == StartPoint)
            {
                return runCases(StartPoint, EndPoint, wires[i].EndPoint, wires[i].StartPoint, connections, wires, i, createdWires);
            }
            return false;
        }

        /// <param name="eqThis">Point on this wire that is equal to eqThat</param>
        /// <param name="chkThis">Point on this wire that may or may not be equal to chkThat</param>
        /// <param name="eqThat">Point on taret wire that is equal to eqThis</param>
        /// <param name="chkThat">Point on target wire that may or may not be equal to chkThis</param>
        private bool runCases(Vec2 eqThis, Vec2 chkThis, Vec2 eqThat, Vec2 chkThat, Dictionary<Vec2, List<DrawableObject>> connections, List<WireLine> wires, int i, List<WireLine> createdWires)
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
                wires[i].RemoveConnections(connections);
                wires[i] = new WireLine(chkThis, chkThat, wires[i].IsHorizontal);
            }
            else
            {
                if (connections[chkThat].Count > 1)
                {
                    //wires.Add(this);
                    //AddConnections(wires[wires.Count - 1], connections);
                    return false;
                }
                wires[i].RemoveConnections(connections);
                wires[i] = new WireLine(chkThis, eqThat, wires[i].IsHorizontal);
            }
            createdWires.AddRange(wires[i].InsertAndAttach(wires, connections));
            return true;
        }

        /// <summary>
        /// Add wire to connections
        /// </summary>
        public override void AddConnections(Dictionary<Vec2, List<DrawableObject>> connections)
        {
            if (!connections.ContainsKey(StartPoint))
            {
                connections[StartPoint] = new List<DrawableObject>();
            }
            if (!connections.ContainsKey(EndPoint))
            {
                connections[EndPoint] = new List<DrawableObject>();
            }

            connections[StartPoint].Add(this);
            connections[EndPoint  ].Add(this);
        }

        /// <summary>
        /// Remove wire from connections
        /// </summary>
        public override void RemoveConnections(Dictionary<Vec2, List<DrawableObject>> connections)
        {
            connections[StartPoint].Remove(this);
            //if(connections[StartPoint].)
            connections[EndPoint  ].Remove(this);
        }

        public static void RemovePointFromWire(Vec2 point, Dictionary<Vec2, List<DrawableObject>> connections, List<WireLine> wires, int i)
        {
            wires[i].RemoveConnections(connections);
            Vec2 initialStart = wires[i].StartPoint;
            Vec2 initialEnd = wires[i].EndPoint;

            Vec2 startsEnd;
            Vec2 endsStart;
            var xpri = wires[i].IsHorizontal;
            if (xpri)
            {
                if(wires[i].StartPoint.X > wires[i].EndPoint.X)
                {
                    startsEnd = point + new Vec2( 1, 0);
                    endsStart = point + new Vec2(-1, 0);
                }
                else
                {
                    startsEnd = point + new Vec2(-1, 0);
                    endsStart = point + new Vec2( 1, 0);
                }
            }
            else
            {
                if (wires[i].StartPoint.Y > wires[i].EndPoint.Y)
                {
                    startsEnd = point + new Vec2(0,  1);
                    endsStart = point + new Vec2(0, -1);
                }
                else
                {
                    startsEnd = point + new Vec2(0, -1);
                    endsStart = point + new Vec2(0,  1);
                }
            }

            if (point == wires[i].StartPoint)
            {
                wires[i] = new WireLine(endsStart, wires[i].EndPoint, wires[i].IsHorizontal);
                wires[i].InsertAndAttach(wires, connections);
            }
            else if (point == wires[i].EndPoint)
            {
                wires[i] = new WireLine(wires[i].StartPoint, startsEnd, wires[i].IsHorizontal);
                wires[i].InsertAndAttach(wires, connections);
            }
            else
            {
                var temp = wires[i].StartPoint;
                wires[i] = new WireLine(endsStart, wires[i].EndPoint, wires[i].IsHorizontal);
                wires[i].InsertAndAttach(wires, connections);
                WireLine newWire = new WireLine(temp, startsEnd, xpri);
                wires.Add(newWire);
                newWire.InsertAndAttach(wires, connections);
            }
        }

        public override void SetPosition(Vec2 position)
        {
            Vec2 offset = position - StartPoint;
            EndPoint += offset;
            base.SetPosition(position);
        }

        public override void Delete(BoardState state)
        {
            state.Wires.Remove(this);
            RemoveConnections(state.Connections);
        }

        public override async Task Draw(PainterScope scope, BoardState state)
        {
            await WirePainter.DrawWireLine(scope, state, this);
        }

        public override BoardObject Copy()
        {
            return new WireLine(StartPoint, EndPoint, IsHorizontal);
        }

        public override string ToString()
        {
            return "{" + StartPoint + ", " + EndPoint + "}";
        }

    }
}
