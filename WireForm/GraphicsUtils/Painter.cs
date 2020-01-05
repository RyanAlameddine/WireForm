using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.MathUtils;

namespace WireForm.GraphicsUtils
{
    public class Painter
    {
        private static float scale = 50f;
        public static float SizeScale
        {
            get
            {
                return scale;
            }
            set
            {
                if (value > 5)
                {
                    scale = value;
                }
                else
                {
                    scale = 5;
                }
            }
        }

        private Graphics gfx;


        Pen getPen(Color color, int width)
        {
            return new Pen(color, width * scale / 50f);
        }

        Brush getEmptyBrush(Color color)
        {
            return (new Pen(color, 1)).Brush;
        }

        void scalePoint(ref Vec2 position)
        {
            position.X *= scale;
            position.Y *= scale;
        }

        void centerPoint(ref Vec2 position, Vec2 size)
        {
            position.X -= size.X / 2f;
            position.Y -= size.Y / 2f;
        }

        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint)
        {
            scalePoint(ref startPoint);
            scalePoint(ref endPoint);

            gfx.DrawLine(getPen(color, penWidth), startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }

        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            scalePoint(ref startPoint);
            scalePoint(ref size);

            gfx.DrawArc(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y, startAngle, sweepAngle);
        }

        public void DrawArcC(Color color, int penWidth, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            gfx.DrawArc(getPen(color, penWidth), (x - width / 2f) * scale, (y - height / 2f) * scale, width * scale, height * scale, startAngle, sweepAngle);
        }

        public void DrawEllipse(Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawEllipse(getPen(color, penWidth), x * scale, y * scale, width * scale, height * scale);
        }

        public void DrawEllipseC(Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawEllipse(getPen(color, penWidth), (x - width / 2f) * scale, (y - height / 2f) * scale, width * scale, height * scale);
        }

        public void FillEllipse(Color color, float x, float y, float width, float height)
        {
            gfx.FillEllipse(getEmptyBrush(color), x * scale, y * scale, width * scale, height * scale);
        }

        public void FillEllipseC(Color color, float x, float y, float width, float height)
        {
            gfx.FillEllipse(getEmptyBrush(color), (x - width / 2f) * scale, (y - height / 2f) * scale, width * scale, height * scale);
        }

        public void DrawRectangle(Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawRectangle(getPen(color, penWidth), x * scale, y * scale, width * scale, height * scale);
        }

        public void FillRectangle(Color color, float x, float y, float width, float height)
        {
            gfx.FillRectangle(getEmptyBrush(color), x * scale, y * scale, width * scale, height * scale);
        }

        public void FillRectangleC(Color color, float x, float y, float width, float height)
        {
            gfx.FillRectangle(getEmptyBrush(color), (x - width / 2f) * scale, (y - height / 2f) * scale, width * scale, height * scale);
        }

        public void DrawString(string s, Color color, float x, float y, float scaleDivider)
        {
            Font font = new Font(FontFamily.GenericMonospace, scale / scaleDivider, FontStyle.Bold);
            gfx.DrawString(s, font, new Pen(color, 10).Brush, x * scale, y * scale);
        }

        public void DrawStringC(string s, Color color, float x, float y, float scaleDivider)
        {
            Font font = new Font(FontFamily.GenericMonospace, scale / scaleDivider, FontStyle.Bold);

            var size = gfx.MeasureString(s, font);

            gfx.DrawString(s, font, new Pen(color, 10).Brush, x * scale - size.Width / 2f, y * scale - size.Height / 2f);
        }
    }
}
