using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.MathUtils.Collision
{
    public struct HitBox : IEquatable<HitBox>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public static HitBox Zero
        {
            get
            {
                return new HitBox(0, 0, 0, 0);
            }
        }

        public HitBox(float X, float Y, float Width, float Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

        public bool Intersects(HitBox other)
        {
            HitBox intersectedRect = GetIntersection(other);
            if(intersectedRect == HitBox.Zero)
            {
                return false;
            }
            return true;
        }

        public HitBox GetIntersection(HitBox other)
        {
            float x1 = Math.Max(this.X, other.X);
            float x2 = Math.Min(this.X + this.Width, other.X + other.Width);
            float y1 = Math.Max(this.Y, other.Y);
            float y2 = Math.Min(this.Y + this.Height, other.Y + other.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new HitBox(x1, y1, x2 - x1, y2 - y1);
            }

            return HitBox.Zero;
        }

        public static bool operator ==(HitBox h1, HitBox h2)
        {
            return h1.Equals(h2);
        }

        public static bool operator !=(HitBox h1, HitBox h2)
        {
            return !h1.Equals(h2);
        }

        public bool Equals(HitBox other)
        {
            if (other.X == X && other.Y == Y && other.Width == Width && other.Height == Height)
            {
                return true;
            }
            else return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HitBox))
            {
                return false;
            }
            return ((HitBox)obj).Equals(this);
        }
    }
}
