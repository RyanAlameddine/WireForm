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
            if (line.Start.X == line.End.X && point.X == line.Start.X)
            {
                if (point.Y == line.Start.Y || point.Y == line.End.Y)
                {
                    return true;
                }

                if (line.Start.Y > point.Y)
                {
                    return point.Y > line.End.Y;
                }
                else
                {
                    return point.Y < line.End.Y;
                }
            }
            else if (line.Start.Y == line.End.Y && point.Y == line.Start.Y)
            {
                if (point.X == line.Start.X || point.X == line.End.X)
                {
                    return true;
                }

                if (line.Start.X > point.X)
                {
                    return point.X > line.End.X;
                }
                else
                {
                    return point.X < line.End.X;
                }
            }

            return false;
        }

        public static bool OnLine(WireLine line1, WireLine line2)
        {
            if((line1.XPriority && line2.XPriority && line1.Start.Y == line2.Start.Y) || (!line1.XPriority && !line2.XPriority && line1.Start.X == line2.Start.X))
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
