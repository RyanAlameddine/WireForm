using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;

namespace WireForm.MathUtils
{
    public static class MathHelper
    {
        public static Vec2 Times(this Vec2 point, float x)
        {
            return new Vec2((int) (point.X * x), (int) (point.Y * x));
        }

        public static Vec2 Plus(this Vec2 point, int x)
        {
            return new Vec2(point.X + x, point.Y + x);
        }

        public static Vec2 Plus(this Vec2 point1, Vec2 point2)
        {
            return new Vec2(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Vec2 ToInts(this Vec2 point)
        {
            return new Vec2((int)point.X, (int)point.Y);
        }

        public static Vec2 Round(this Vec2 point)
        {
            return new Vec2((float) Math.Round(point.X), (float) Math.Round(point.Y));
        }


        public static bool IsContainedIn(this Vec2 point, WireLine line)
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

        public static int ManhattanDistance(Vec2 point1, Vec2 point2)
        {
            return (int) Math.Abs(point1.X - point2.X) + (int) Math.Abs(point1.Y - point2.Y);
        }
    }
}
