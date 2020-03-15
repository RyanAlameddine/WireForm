using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.MathUtils;

namespace WireForm.GraphicsUtils
{
    /// <summary>
    /// A tool used to draw on the circuit board.
    /// Each painter contains a reference to the current Graphics class
    /// as well as the current zoom value.
    /// Painters also contain an internal offset value which will be applied to all position elements
    /// </summary>
    public struct Painter
    {
        private float zoom; // = 50f

        private Graphics gfx;

        private Vec2 offset;

        /// <summary>
        /// Creates an empty painter
        /// </summary>
        public Painter(Graphics gfx, float zoom)
        {
            this.gfx = gfx;
            this.zoom = zoom;
            this.offset = Vec2.Zero;
        }

        /// <summary>
        /// Adds a new Vec2 to the target offset
        /// </summary>
        public void AppendOffset(Vec2 offset)
        {
            this.offset += offset;
        }


        Pen getPen(Color color, int width)
        {
            return new Pen(color, width * zoom / 50f);
        }

        Brush getEmptyBrush(Color color)
        {
            return (new Pen(color, 1)).Brush;
        }

        /// <summary>
        /// Scales a point to match the Painter.zoom value
        /// </summary>
        void ScalePoint(ref Vec2 position)
        {
            position.X *= zoom;
            position.Y *= zoom;
        }

        /// <summary>
        /// Offsets a point to match the Painter.offset value
        /// </summary>
        void OffsetPoint(ref Vec2 position)
        {
            position.X += offset.X;
            position.Y += offset.Y;
        }

        /// <summary>
        /// Centers a point to the middle of it's size Vector
        /// </summary>
        /// <param name="position"></param>
        void CenterPoint(ref Vec2 position, Vec2 size)
        {
            position.X -= size.X / 2f;
            position.Y -= size.Y / 2f;
        }

        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint)
        {
            OffsetPoint(ref startPoint);
            OffsetPoint(ref endPoint);

            ScalePoint(ref startPoint);
            ScalePoint(ref endPoint);

            gfx.DrawLine(getPen(color, penWidth), startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }

        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            OffsetPoint(ref startPoint);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.DrawArc(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y, startAngle, sweepAngle);
        }

        public void DrawArcC(Color color, int penWidth, Vec2 centerPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            OffsetPoint(ref centerPoint);
            CenterPoint(ref centerPoint, size);

            ScalePoint(ref centerPoint);
            ScalePoint(ref size);

            gfx.DrawArc(getPen(color, penWidth), centerPoint.X, centerPoint.Y, size.X, size.Y, startAngle, sweepAngle);
        }

        public void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            OffsetPoint(ref startPoint);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.DrawEllipse(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void DrawEllipseC(Color color, int penWidth, Vec2 centerPoint, Vec2 size)
        {
            OffsetPoint(ref centerPoint);
            CenterPoint(ref centerPoint, size);

            ScalePoint(ref centerPoint);
            ScalePoint(ref size);

            gfx.DrawEllipse(getPen(color, penWidth), centerPoint.X, centerPoint.Y, size.X, size.Y);
        }

        public void FillEllipse(Color color, Vec2 startPoint, Vec2 size)
        {
            OffsetPoint(ref startPoint);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.FillEllipse(getEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillEllipseC(Color color, Vec2 centerPoint, Vec2 size)
        {
            OffsetPoint(ref centerPoint);

            CenterPoint(ref centerPoint, size);

            ScalePoint(ref centerPoint);
            ScalePoint(ref size);

            gfx.FillEllipse(getEmptyBrush(color), centerPoint.X, centerPoint.Y, size.X, size.Y);
        }

        public void DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            OffsetPoint(ref startPoint);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.DrawRectangle(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillRectangle(Color color, Vec2 startPoint, Vec2 size)
        {
            OffsetPoint(ref startPoint);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.FillRectangle(getEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillRectangleC(Color color, Vec2 centerPoint, Vec2 size)
        {
            OffsetPoint(ref centerPoint);
            CenterPoint(ref centerPoint, size);

            ScalePoint(ref centerPoint);
            ScalePoint(ref size);

            gfx.FillRectangle(getEmptyBrush(color), centerPoint.X, centerPoint.Y, size.X, size.Y);
        }

        public void DrawString(string s, Color color, Vec2 startPoint, float scaleDivider)
        {
            OffsetPoint(ref startPoint);

            Font font = new Font(FontFamily.GenericMonospace, zoom / scaleDivider, FontStyle.Bold);
            gfx.DrawString(s, font, new Pen(color, 10).Brush, startPoint.X * zoom, startPoint.Y * zoom);
        }

        public void DrawStringC(string s, Color color, Vec2 centerPoint, float scaleDivider)
        {
            OffsetPoint(ref centerPoint);

            Font font = new Font(FontFamily.GenericMonospace, zoom / scaleDivider, FontStyle.Bold);

            var size = gfx.MeasureString(s, font);

            gfx.DrawString(s, font, new Pen(color, 10).Brush, centerPoint.X * zoom - size.Width / 2f, centerPoint.Y * zoom - size.Height / 2f);
        }
    }
}
