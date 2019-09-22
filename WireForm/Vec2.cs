using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    public struct Vec2 : IEquatable<Vec2>
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vec2 Zero {
            get
            {
                return new Vec2(0, 0);
            }
        }

        public Vec2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        #region mathematical
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }

        public static explicit operator Point(Vec2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public static explicit operator Vec2(Point pnt)
        {
            return new Vec2(pnt.X, pnt.Y);
        }
        #endregion

        #region logical
        public bool Equals(Vec2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Vec2))
            {
                return false;
            }
            return ((Vec2)obj).Equals(this);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                return hash;
            }
        }

        public static bool operator  ==(Vec2 a, Vec2 b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Vec2 a, Vec2 b)
        {
            return !a.Equals(b);
        }

        #endregion


    }
}
