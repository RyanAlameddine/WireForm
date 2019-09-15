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

        public static Point Plus(this Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static bool IsContainedIn(this Point point, WireLine line)
        {
            if (line.StartPoint.X == line.EndPoint.X && point.X == line.StartPoint.X)
            {
                if (point.Y == line.StartPoint.Y || point.Y == line.EndPoint.Y)
                {
                    return true;
                }

                if (line.StartPoint.Y > point.Y)
                {
                    return point.Y > line.EndPoint.Y;
                }
                else
                {
                    return point.Y < line.EndPoint.Y;
                }
            }
            else if (line.StartPoint.Y == line.EndPoint.Y && point.Y == line.StartPoint.Y)
            {
                if (point.X == line.StartPoint.X || point.X == line.EndPoint.X)
                {
                    return true;
                }

                if (line.StartPoint.X > point.X)
                {
                    return point.X > line.EndPoint.X;
                }
                else
                {
                    return point.X < line.EndPoint.X;
                }
            }

            return false;
        }

        public static bool OnLine(WireLine line1, WireLine line2)
        {
            if((line1.XPriority && line2.XPriority && line1.StartPoint.Y == line2.StartPoint.Y) || (!line1.XPriority && !line2.XPriority && line1.StartPoint.X == line2.StartPoint.X))
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
