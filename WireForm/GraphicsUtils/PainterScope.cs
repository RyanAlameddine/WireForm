﻿using System.Drawing;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;

namespace Wireform.GraphicsUtils
{
    /// <summary>
    /// A tool used to draw on the circuit board.
    /// Each painter contains a reference to the current Graphics class
    /// as well as the current zoom value.
    /// Painters also contain an internal offset value which will be applied to all position elements
    /// and a multiplier which determines rotational values of drawn gates
    /// </summary>
    public struct PainterScope
    {
        private readonly float zoom; // = 50f

        private readonly IPainter painter;

        private Vec2 offset;

        private (int xMult, int yMult, bool flipXY) multiplier;

        /// <summary>
        /// Creates an empty painter
        /// </summary>
        public PainterScope(IPainter painter, float zoom)
        {
            this.painter = painter;
            this.zoom = zoom;
            this.offset = Vec2.Zero;
            this.multiplier = (1, 1, false);
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
        /// Converts direction enum into multipliers fo the painting.
        /// </summary>
        public void SetLocalMultiplier(Direction direction) => multiplier = direction.GetMultiplier();

        /// <summary>
        /// Scales a penWidth to match the zoom
        /// </summary>
        void ScaleWidth(ref int penWidth)
        {
            penWidth = (int) (penWidth * zoom / 50f);
        }

        /// <summary>
        /// Scales a point to match the PainterScope.zoom value
        /// </summary>
        void ScalePoint(ref Vec2 position)
        {
            position.X *= zoom;
            position.Y *= zoom;
        }

        /// <summary>
        /// Offsets and flips a point to match the PainterScope.offset value and PainterScope.multiplier
        /// </summary>
        void OffsetPosition(ref Vec2 position, ref Vec2 size)
        {
            var (xMult, yMult, flipXY) = multiplier;
            if (flipXY)
            {
                size = new Vec2(size.Y, size.X);
                position = offset + new Vec2(position.Y * yMult, position.X * xMult);
            }
            else
            {
                position = offset + new Vec2(position.X * xMult, position.Y * yMult);
            }
        }
        /// <summary>
        /// Offsets and flips a point to match the PainterScope.offset value and PainterScope.multiplier where the bound is topLeft
        /// </summary>
        void OffsetPositionTL(ref Vec2 position, ref Vec2 size)
        {
            var (xMult, yMult, flipXY) = multiplier;
            float x = position.X;
            float y = position.Y;
            if (flipXY)
            {
                size = new Vec2(size.Y, size.X);
                float temp = y;
                y = x;
                x = temp;
            }
            float xCenter = size.X / 2f;
            float yCenter = size.Y / 2f;
            position.X = offset.X + (x + xCenter) * xMult - xCenter;
            position.Y = offset.Y + (y + yCenter) * yMult - yCenter;
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
        /// Offsets an arc by the PainterScope.multiplier value
        /// </summary>
        void MultiplyArc(ref float startAngle, ref float sweepAngle)
        {
            int xMult = multiplier.xMult;
            int yMult = multiplier.yMult;
            if (multiplier.flipXY)
            {
                startAngle = 90 - startAngle;
                sweepAngle *= -1;

                yMult = multiplier.xMult;
                xMult = multiplier.yMult;
            }
            if(xMult == -1)
            {
                startAngle = 180 - startAngle;
                sweepAngle *= -1;
            }
            if(yMult == -1)
            {
                startAngle = -startAngle;
                sweepAngle *= -1;
            }
        }

        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint)
        {
            Vec2 zero = Vec2.Zero;
            OffsetPosition(ref startPoint, ref zero);
            OffsetPosition(ref endPoint, ref zero);

            ScalePoint(ref startPoint);
            ScalePoint(ref endPoint); 
            ScaleWidth(ref penWidth);

            painter.DrawLine(color, penWidth, startPoint, endPoint);
        }

        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            painter.DrawArc(color, penWidth, startPoint, size, startAngle, sweepAngle);
        }

        public void DrawArcC(Color color, int penWidth, Vec2 centralPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            painter.DrawArc(color, penWidth, centralPoint, size, startAngle, sweepAngle);
        }

        public void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            painter.DrawEllipse(color, penWidth, startPoint, size);
        }

        public void DrawEllipseC(Color color, int penWidth, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            painter.DrawEllipse(color, penWidth, centralPoint, size);
        }

        public void FillEllipse(Color color, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            painter.FillEllipse(color, startPoint, size);
        }

        public void FillEllipseC(Color color, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);

            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            painter.FillEllipse(color, centralPoint, size);
        }

        public void DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            painter.DrawRectangle(color, penWidth, startPoint, size);
        }

        public void FillRectangle(Color color, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            painter.FillRectangle(color, startPoint, size);
        }

        public void FillRectangleC(Color color, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            painter.FillRectangle(color, centralPoint, size);
        }

        public void DrawString(string s, Color color, Vec2 startPoint, float scale)
        {
            var size = painter.MeasureString(s, zoom, scale * zoom);
            var V2Size = new Vec2(size.X, size.Y);

            OffsetPositionTL(ref startPoint, ref V2Size);

            painter.DrawString(s, color, startPoint * zoom, scale * zoom);
        }

        public void DrawStringC(string s, Color color, Vec2 centralPoint, float scale)
        {
            var size = painter.MeasureString(s, zoom, scale * zoom);
            var V2Size = new Vec2(size.X, size.Y);

            OffsetPosition(ref centralPoint, ref V2Size);

            painter.DrawString(s, color, centralPoint * zoom - size / 2f, scale * zoom);
        }
    }
}
