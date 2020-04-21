﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;

namespace WireForm.MathUtils.Collision
{
    public class BoxCollider : IEquatable<BoxCollider>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Vec2 Position 
        {
            get => new Vec2(X, Y);
        }

        public Vec2 Bounds
        {
            get => new Vec2(Width, Height);
        }

        public static BoxCollider Zero
        {
            get => new BoxCollider(0, 0, 0, 0);
        }

        public BoxCollider(float X, float Y, float Width, float Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }


        public bool Intersects(BoxCollider other) => Intersects(other, out _);

        public bool Intersects(BoxCollider other, out BoxCollider intersection)
        {
            intersection = GetIntersection(other);
            if (intersection == null)
            {
                return false;
            }
            return true;
        }

        private BoxCollider GetIntersection(BoxCollider other)
        {
            float x1 = Math.Max(this.X, other.X);
            float x2 = Math.Min(this.X + this.Width, other.X + other.Width);
            float y1 = Math.Max(this.Y, other.Y);
            float y2 = Math.Min(this.Y + this.Height, other.Y + other.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new BoxCollider(x1, y1, x2 - x1, y2 - y1);
            }

            return null;
        }

        /// <summary>
        /// Returns a BoxCollider that contains the same area but all values for width and height are positive
        /// </summary>
        public BoxCollider GetNormalized()
        {
            BoxCollider newBox = new BoxCollider(X, Y, Width, Height);
            if (newBox.Width < 0)
            {
                newBox.X += newBox.Width;
                newBox.Width *= -1;
            }
            if (newBox.Height < 0)
            {
                newBox.Y += newBox.Height;
                newBox.Height *= -1;
            }
            return newBox;
        }

        public static bool operator ==(BoxCollider h1, BoxCollider h2)
        {
            if (h1 is null)
            {
                return h2 is null;
            }
            if(h2 is null)
            {
                return false;
            }
            return h1.Equals(h2);
        }

        public static bool operator !=(BoxCollider h1, BoxCollider h2)
        {
            return !(h1 == h2);
        }

        public bool Equals(BoxCollider other)
        {
            if (other.X == X && other.Y == Y && other.Width == Width && other.Height == Height)
            {
                return true;
            }
            else return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoxCollider))
            {
                return false;
            }
            return ((BoxCollider)obj).Equals(this);
        }

        /// <summary>
        /// Auto-generated by Visual Studios 2019
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = 466501756;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        public BoxCollider Copy()
        {
            return new BoxCollider(X, Y, Width, Height);
        }

        /// <summary>
        /// Gets all the Gate intersections a certain BoxCollider hits. If only2D == true, ignores all intersections which are not two-dimensional
        /// </summary>
        /// <param name="intersectBoxes">The rectangles for the intersections</param>
        /// <returns>Did the BoxCollider intersect with anything</returns>
        public bool GetIntersections(BoardState propogator, bool hitWires, out HashSet<BoxCollider> intersectBoxes, out HashSet<CircuitObject> intersectedcircuitObjects, bool only2D = true)
        {
            intersectBoxes = new HashSet<BoxCollider>();
            intersectedcircuitObjects = new HashSet<CircuitObject>();

            if (hitWires)
            {
                foreach (WireLine wire in propogator.wires)
                {
                    BoxCollider collider = wire.HitBox;
                    if (Intersects(collider, out var intersection))
                    {
                        intersectedcircuitObjects.Add(wire);
                        intersectBoxes.Add(intersection);
                    }
                }
            }

            foreach (Gate gate in propogator.gates)
            {
                BoxCollider collider = gate.HitBox;
                if (Intersects(collider, out var intersection))
                {
                    if (only2D && (intersection.Width == 0 || intersection.Height == 0)) continue;
                    intersectedcircuitObjects.Add(gate);
                    intersectBoxes.Add(intersection);
                }
            }

            if (intersectBoxes.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}
