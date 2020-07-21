using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wireform.MathUtils
{
    public struct Vec2 : IEquatable<Vec2>
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static Vec2 Zero => new Vec2(0, 0);

        public Vec2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        #region mathematical

        //Vector to Vector

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2 operator *(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X * b.X, a.Y * b.Y);
        }

        public static Vec2 operator /(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X / b.X, a.Y / b.Y);
        }

        //Vector to float
        public static Vec2 operator *(Vec2 a, float b)
        {
            return new Vec2(a.X * b, a.Y * b);
        }
        public static Vec2 operator /(Vec2 a, float b)
        {
            return new Vec2(a.X / b, a.Y / b);
        }
        public static Vec2 operator /(float b, Vec2 a)
        {
            return new Vec2(b / a.X, b / a.Y);
        }
        public static Vec2 operator +(Vec2 a, float b)
        {
            return new Vec2(a.X + b, a.Y + b);
        }
        public static Vec2 operator -(Vec2 a, float b)
        {
            return new Vec2(a.X - b, a.Y - b);
        }
        public static Vec2 operator -(float b, Vec2 a)
        {
            return new Vec2(b - a.X, b - a.Y);
        }
        public static Vec2 operator -(Vec2 a)
        {
            return new Vec2(-a.X, -a.Y);
        }
        #endregion

        //Casting
        public static explicit operator Point(Vec2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public static explicit operator Vec2(Point pnt)
        {
            return new Vec2(pnt.X, pnt.Y);
        }

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

        public override string ToString()
        {
            return "{" + X + ", " + Y + "}";
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
