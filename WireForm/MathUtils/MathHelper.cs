﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;
using WireForm.GraphicsUtils;

namespace WireForm.MathUtils
{
    public static class MathHelper
    {

        public static Vec2 Times(this Vec2 point, float a)
        {
            return new Vec2((int) (point.X * a), (int) (point.Y * a));
        }

        public static Vec2 Plus(this Vec2 point, int a)
        {
            return new Vec2(point.X + a, point.Y + a);
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

        /// <summary>
        /// Returns true if both wires have the same X coordinate (for horizontal lines) or Y coordinate (for vertical lines)
        /// </summary>
        public static bool OnLine(WireLine line1, WireLine line2)
        {
            if((line1.IsHorizontal && line2.IsHorizontal && line1.StartPoint.Y == line2.StartPoint.Y) || (!line1.IsHorizontal && !line2.IsHorizontal && line1.StartPoint.X == line2.StartPoint.X))
            {
                return true;
            }
            return false;
        }

        public static int ManhattanDistance(Vec2 point1, Vec2 point2)
        {
            return (int) Math.Abs(point1.X - point2.X) + (int) Math.Abs(point1.Y - point2.Y);
        }

        /// <summary>
        /// Linearly interpolate from Vec2 a to Vec2 b by amount t.
        /// t=0 is a. t=1 is b. t=0.5f is a point halfway between the a and b.
        /// </summary>
        public static Vec2 Lerp(Vec2 a, Vec2 b, float t)
        {
            return new Vec2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }

        public static int Ceiling(float value)
        {
            int i = (int)value;
            if(value != i)
            {
                return i + 1;
            }
            return i;
        }

        ///// <summary>
        ///// Position of the mouse in local coordinates rounded to the nearest grid point
        ///// </summary>
        //public static Vec2 LocalGridPoint(Vec2 mousePosition)
        //{
        //    return ((mousePosition + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();
        //}

        /// <summary>
        /// Transforms a Vec2 from viewport coordinates to relative to the local grid
        /// </summary>
        public static Vec2 ViewportToLocalPoint(Vec2 point)
        {
            return point * (1 / GraphicsManager.SizeScale);
        }
    }
}
