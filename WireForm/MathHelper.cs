using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    public static class MathHelper
    {
        public static Point Times(this Point point, float x)
        {
            return new Point((int) (point.X * x), (int) (point.Y * x));
        }

        public static Point Plus(this Point point, int x)
        {
            return new Point(point.X + x, point.Y + x);
        }

        public static bool ContainedIn(this Point point, WireLine line)
        {
            if (line.WireStart.X == line.WireEnd.X && point.X == line.WireStart.X)
            {
                if (point.Y == line.WireStart.Y || point.Y == line.WireEnd.Y)
                {
                    return true;
                }

                if (line.WireStart.Y > point.Y)
                {
                    return point.Y > line.WireEnd.Y;
                }
                else
                {
                    return point.Y < line.WireEnd.Y;
                }
            }
            else if (line.WireStart.Y == line.WireEnd.Y && point.Y == line.WireStart.Y)
            {
                if (point.X == line.WireStart.X || point.X == line.WireEnd.X)
                {
                    return true;
                }

                if (line.WireStart.X > point.X)
                {
                    return point.X > line.WireEnd.X;
                }
                else
                {
                    return point.X < line.WireEnd.X;
                }
            }

            return false;
        }

        public static bool OnLine(WireLine line1, WireLine line2)
        {
            if((line1.XPriority && line2.XPriority && line1.WireStart.Y == line2.WireStart.Y) || (!line1.XPriority && !line2.XPriority && line1.WireStart.X == line2.WireStart.X))
            {
                return true;
            }
            return false;
        }

        public static int ManhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }
    }
}
