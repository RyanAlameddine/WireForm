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
        public Point WireStart { get; set; }
        public Point WireEnd { get; set; }

        public bool XPriority { get; set; }

        public WireLine(Point start, Point end, bool XPriority)
        {
            WireStart = start;
            WireEnd = end;
            this.XPriority = XPriority;
        }

        //TODO ADD THE ABILITY TO BREAK LINES
        public void IncludeWire(List<WireLine> wires)
        {
            bool existing = validate(wires);
            if (existing)
            {

            }
        }

        /// <returns>Returns if this wire still remains in the wires list</returns>
        private bool validate(List<WireLine> wires)
        {
            wires.Remove(this);

            for (int i = 0; i < wires.Count; i++)
            {
                if (!MathHelper.OnLine(this, wires[i]))
                {
                    //If wirestart is contained in wires[i], split wires[i] into two wires
                    if (MathHelper.ContainedIn(WireStart, wires[i]))
                    {
                        WireLine wire1 = new WireLine(WireStart, wires[i].WireEnd, wires[i].XPriority);
                        WireLine wire2 = new WireLine(wires[i].WireStart, WireStart, wires[i].XPriority);
                        //Wire1 is nonexistant
                        if (wire1.WireStart == wire1.WireEnd)
                        {
                            wires[i] = wire2;
                        }
                        //Wire2 is nonexistant
                        else if (wire2.WireStart == wire2.WireEnd)
                        {
                            wires[i] = wire1;
                        }
                        //Both wires exist
                        else
                        {
                            wires[i] = wire1;
                            wires.Add(wire2);
                        }
                    }
                    //If wireend is contained in wires[i], split wires[i] into two wires
                    else if (MathHelper.ContainedIn(WireEnd, wires[i]))
                    {
                        WireLine wire1 = new WireLine(WireEnd, wires[i].WireEnd, wires[i].XPriority);
                        WireLine wire2 = new WireLine(wires[i].WireStart, WireEnd, wires[i].XPriority);
                        //Wire1 is nonexistant
                        if (wire1.WireStart == wire1.WireEnd)
                        {
                            wires[i] = wire2;
                        }
                        //Wire2 is nonexistant
                        else if (wire2.WireStart == wire2.WireEnd)
                        {
                            wires[i] = wire1;
                        }
                        //Both wires exist
                        else
                        {
                            wires[i] = wire1;
                            wires.Add(wire2);
                        }
                    }

                    continue;
                }

                //Cases where an start/end point matches with the target wire
                if (checkMatchCases(wires, i))
                {
                    return false;
                }

                //Cases where this wire is contained in the target wire
                if (WireStart.ContainedIn(wires[i]))
                {
                    //If both points of this wire are contained in the target wire
                    if (WireEnd.ContainedIn(wires[i]))
                    {
                        return false;
                    }
                    //Else

                    //Create a match case
                    WireStart = wires[i].WireStart;
                    checkMatchCases(wires, i);

                    return false;
                }
                if (WireEnd.ContainedIn(wires[i]))
                {
                    //Create a match case
                    WireEnd = wires[i].WireEnd;
                    checkMatchCases(wires, i);

                    return false;
                }

                //Cases where the target wire is contained in this wire
                if (wires[i].WireStart.ContainedIn(this) && wires[i].WireEnd.ContainedIn(this))
                {
                    wires[i] = this;
                    wires[i].IncludeWire(wires);
                    return false;
                }
            }
            wires.Add(this);
            return true;
        }

        private bool checkMatchCases(List<WireLine> wires, int i)
        {
            if (wires[i].WireStart == WireStart)
            {
                runCases(WireStart, WireEnd, wires[i].WireStart, wires[i].WireEnd, wires, i);
                return true;
            }
            if (wires[i].WireStart == WireEnd)
            {
                runCases(WireEnd, WireStart, wires[i].WireStart, wires[i].WireEnd, wires, i);
                return true;
            }

            if (wires[i].WireEnd == WireEnd)
            {
                runCases(WireEnd, WireStart, wires[i].WireEnd, wires[i].WireStart, wires, i);
                return true;
            }
            if (wires[i].WireEnd == WireStart)
            {
                runCases(WireStart, WireEnd, wires[i].WireEnd, wires[i].WireStart, wires, i);
                return true;
            }
            return false;
        }

        /// <param name="eqThis">Point on this wire that is equal to eqThat</param>
        /// <param name="chkThis">Point on this wire that may or may not be equal to chkThat</param>
        /// <param name="eqThat">Point on taret wire that is equal to eqThis</param>
        /// <param name="chkThat">Point on target wire that may or may not be equal to chkThis</param>
        private void runCases(Point eqThis, Point chkThis, Point eqThat, Point chkThat, List<WireLine> wires, int i)
        {
            //Wires are the same
            if (chkThat == chkThis)
            {
                return;
            }

            //This wire is completely contained in that wire
            if (chkThis.ContainedIn(wires[i]))
            {
                return;
            }

            //This wire extends out of that wire
            if (MathHelper.ManhattanDistance(chkThis, eqThat) < MathHelper.ManhattanDistance(chkThis, chkThat))
            {
                wires[i] = new WireLine(chkThis, chkThat, wires[i].XPriority);
            }
            else
            {
                wires[i] = new WireLine(eqThat, chkThis, wires[i].XPriority);
            }
            wires[i].IncludeWire(wires);
            return;
        }
    }
}
