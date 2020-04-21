using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry.Utilities;
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
        private float zoom; // = 50f

        private Painter painter;

        private Vec2 offset;

        private Vec2 multiplier;

        /// <summary>
        /// Creates an empty painter
        /// </summary>
        public PainterScope(Painter painter, float zoom)
        {
            this.painter = painter;
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
        /// Offsets and flips a point to match the PainterScope.offset value and PainterScope.multiplier where the bound is topLeft
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
        /// Offsets an arc by the PainterScope.multiplier value
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
        }

        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint)
        {
            Vec2 zero = Vec2.Zero;
            OffsetPosition(ref startPoint, ref zero);
            OffsetPosition(ref endPoint, ref zero);

            ScalePoint(ref startPoint);
            ScalePoint(ref endPoint);

            painter.DrawLine(color, penWidth, startPoint, endPoint);
        }

        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            painter.DrawArc(color, penWidth, startPoint, size, startAngle, sweepAngle);
        }

        public void DrawArcC(Color color, int penWidth, Vec2 centralPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            painter.DrawArc(color, penWidth, centralPoint, size, startAngle, sweepAngle);
        }

        public void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            OffsetPositionTL(ref startPoint, ref size);

            ScalePoint(ref startPoint);
            ScalePoint(ref size);

            painter.DrawEllipse(color, penWidth, startPoint, size);
        }

        public void DrawEllipseC(Color color, int penWidth, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

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

        public void DrawString(string s, Color color, Vec2 startPoint, float scaleDivider)
        {
            var size = painter.MeasureString(s, zoom, scaleDivider);
            var V2Size = new Vec2(size.X, size.Y);

            OffsetPositionTL(ref startPoint, ref V2Size);

            painter.DrawString(s, color, startPoint * zoom, scaleDivider);
        }

        public void DrawStringC(string s, Color color, Vec2 centralPoint, float scaleDivider)
        {
            var size = painter.MeasureString(s, zoom, scaleDivider);
            var V2Size = new Vec2(size.X, size.Y);

            OffsetPosition(ref centralPoint, ref V2Size);

            painter.DrawString(s, color, centralPoint * zoom - size / 2f, scaleDivider);
        }
    }
}
