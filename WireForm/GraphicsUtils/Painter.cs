using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry.Utilities;
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

        private Vec2 multiplier;

        /// <summary>
        /// Creates an empty painter
        /// </summary>
        public Painter(Graphics gfx, float zoom)
        {
            this.gfx = gfx;
            this.zoom = zoom;
            this.offset = Vec2.Zero;
            this.multiplier = new Vec2(1, 1);
        }

        /// <summary>
        /// Adds a new Vec2 to the target offset
        /// </summary>
        public void AppendOffset(Vec2 offset)
        {
            this.offset += offset;
        }
        
        /// <summary>
        /// Sets the multiplier for the un-offsetted position values for drawing.
        /// The sign on the x and y coordinates should determine which direction a gate is facing.
        /// </summary>
        /// <param name="multiplier">X and Y should be equal to 1, or -1 ONLY</param>
        private void setLocalMultiplier(Vec2 multiplier)
        {
            this.multiplier = multiplier;
        }

        /// <summary>
        /// Sets the multiplier for the un-offsetted position values for drawing.
        /// Converts direction enum into multipliers fo the painting.
        /// </summary>
        public void SetLocalMultiplier(Direction direction)
        {
            multiplier = direction.GetMultiplier();
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
        /// Offsets and flips a point to match the Painter.offset value and Painter.multiplier
        /// </summary>
        void OffsetPosition(ref Vec2 position, ref Vec2 size)
        {
            var mult = multiplier;
            if (multiplier.Y == -1)
            {
                size = new Vec2(size.Y, size.X);
                position = new Vec2(position.Y, position.X);
                mult = new Vec2(1, mult.X);
            }
            position.X = offset.X + position.X * mult.X;
            position.Y = offset.Y + position.Y * mult.Y;
        }
        /// <summary>
        /// Offsets and flips a point to match the Painter.offset value and Painter.multiplier where the bound is topLeft
        /// </summary>
        void OffsetPositionTL(ref Vec2 position, ref Vec2 size)
        {
            var mult = multiplier;
            if (multiplier.Y == -1)
            {
                size = new Vec2(size.Y, size.X);
                position = new Vec2(position.Y, position.X);
                mult = new Vec2(1, mult.X);
            }
            float xCenter = size.X / 2f;
            float yCenter = size.Y / 2f;
            position.X = offset.X + (position.X + xCenter) * mult.X - xCenter;
            position.Y = offset.Y + (position.Y + yCenter) * mult.Y - yCenter;
        }

        /// <summary>
        /// Centers a point to the middle of it's size Vector
        /// </summary>
        void CenterPoint(ref Vec2 position, Vec2 size)
        {
            position.X -= size.X / 2f;
            position.Y -= size.Y / 2f;
        }

        /// <summary>
        /// Offsets an arc by the Painter.multiplier value
        /// </summary>
        void MultiplyArc(ref float startAngle, ref float sweepAngle)
        {
            if(multiplier.X == -1)
            {
                if(multiplier.Y == -1)
                {
                    startAngle = startAngle-90;
                    //sweepAngle *= -1;
                    return;
                }
                startAngle = 180 - startAngle;
                sweepAngle *= -1;
            }
            else if(multiplier.Y == -1)
            {
                startAngle = 90 - startAngle;
                sweepAngle *= -1;
            }

            //int switcher = 0;
            //if (multiplier.X == -1)
            //{
            //    switcher = -180;
            //    if (multiplier.Y == -1)
            //    {
            //        switcher = -90;
            //    }
            //}
            //else if(multiplier.Y == -1)
            //{
            //    switcher = 90;
            //}



            //startAngle = startAngle + switcher; 
        }

        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint)
        {
            Vec2 zero = Vec2.Zero;
            OffsetPosition(ref startPoint, ref zero);
            OffsetPosition(ref endPoint, ref zero);

            ScalePoint(ref startPoint);
            ScalePoint(ref endPoint);

            gfx.DrawLine(getPen(color, penWidth), startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }

        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.DrawArc(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y, startAngle, sweepAngle);
        }

        public void DrawArcC(Color color, int penWidth, Vec2 centralPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            //CenterPoint(ref centralPoint, size);
            //DrawArc(color, penWidth, centralPoint, size, startAngle, sweepAngle);
            //sweepAngle = 360;
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            gfx.DrawArc(getPen(color, penWidth), centralPoint.X, centralPoint.Y, size.X, size.Y, startAngle, sweepAngle);
        }

        public void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.DrawEllipse(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void DrawEllipseC(Color color, int penWidth, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            gfx.DrawEllipse(getPen(color, penWidth), centralPoint.X, centralPoint.Y, size.X, size.Y);
        }

        public void FillEllipse(Color color, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.FillEllipse(getEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillEllipseC(Color color, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);

            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            gfx.FillEllipse(getEmptyBrush(color), centralPoint.X, centralPoint.Y, size.X, size.Y);
        }

        public void DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.DrawRectangle(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillRectangle(Color color, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            gfx.FillRectangle(getEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillRectangleC(Color color, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            gfx.FillRectangle(getEmptyBrush(color), centralPoint.X, centralPoint.Y, size.X, size.Y);
        }

        public void DrawString(string s, Color color, Vec2 startPoint, float scaleDivider)
        {
            Font font = new Font(FontFamily.GenericMonospace, zoom / scaleDivider, FontStyle.Bold);

            var size = gfx.MeasureString(s, font);
            var V2Size = new Vec2(size.Width, size.Height);

            OffsetPositionTL(ref startPoint, ref V2Size);

            gfx.DrawString(s, font, new Pen(color, 10).Brush, startPoint.X * zoom, startPoint.Y * zoom);
        }

        public void DrawStringC(string s, Color color, Vec2 centralPoint, float scaleDivider)
        {
            Font font = new Font(FontFamily.GenericMonospace, zoom / scaleDivider, FontStyle.Bold);

            var size = gfx.MeasureString(s, font);
            var V2Size = new Vec2(size.Width, size.Height);

            OffsetPosition(ref centralPoint, ref V2Size);

            gfx.DrawString(s, font, new Pen(color, 10).Brush, centralPoint.X * zoom - size.Width / 2f, centralPoint.Y * zoom - size.Height / 2f);
        }
    }
}
