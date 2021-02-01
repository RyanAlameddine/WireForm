using System.Drawing;
using System.Threading.Tasks;
using Wireform.Circuitry.Utils;
using Wireform.MathUtils;

namespace Wireform.GraphicsUtils
{
    /// <summary>
    /// A tool used to draw on the circuit board.
    /// Each painter contains a reference to the current Graphics class
    /// as well as the current Zoom value.
    /// Painters also contain an internal offset value which will be applied to all position elements
    /// and a multiplier which determines rotational values of drawn gates
    /// </summary>
    public struct PainterScope
    {
        public readonly float Zoom { get; } // = 50f

        private readonly IPainter painter;

        private Vec2 offset;

        private (int xMult, int yMult, bool flipXY) multiplier;

        /// <summary>
        /// Creates an empty painter
        /// </summary>
        public PainterScope(IPainter painter, float Zoom)
        {
            this.painter = painter;
            this.Zoom = Zoom;
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
        /// Scales a penWidth to match the Zoom
        /// </summary>
        void ScaleWidth(ref int penWidth)
        {
            penWidth = (int) (penWidth * Zoom / 50f);
        }

        /// <summary>
        /// Scales a point to match the PainterScope.Zoom value
        /// </summary>
        void ScalePoint(ref Vec2 position)
        {
            position.X *= Zoom;
            position.Y *= Zoom;
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

                int tempMult = yMult;
                yMult = xMult;
                xMult = tempMult;
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

        public async Task DrawLine(Color color, int penWidth, Vec2 point, Vec2 endPoint)
        {
            Vec2 zero = Vec2.Zero;
            OffsetPosition(ref point, ref zero);
            OffsetPosition(ref endPoint, ref zero);

            ScalePoint(ref point);
            ScalePoint(ref endPoint); 
            ScaleWidth(ref penWidth);

            await painter.DrawLine(color, penWidth, point, endPoint);
        }

        public async Task DrawArc(Color color, int penWidth, Vec2 point, Vec2 size, float startAngle, float sweepAngle)
        {
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPositionTL(ref point, ref size);

            ScalePoint(ref point);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            await painter.DrawArc(color, penWidth, point, size, startAngle, sweepAngle);
        }

        public async Task DrawArcC(Color color, int penWidth, Vec2 centralPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            MultiplyArc(ref startAngle, ref sweepAngle);
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            await painter.DrawArc(color, penWidth, centralPoint, size, startAngle, sweepAngle);
        }

        public async Task DrawEllipse(Color color, int penWidth, Vec2 point, Vec2 size)
        {
            OffsetPositionTL(ref point, ref size);

            ScalePoint(ref point);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            await painter.DrawEllipse(color, penWidth, point, size);
        }

        public async Task DrawEllipseC(Color color, int penWidth, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            await painter.DrawEllipse(color, penWidth, centralPoint, size);
        }

        public async Task FillEllipse(Color color, Vec2 point, Vec2 size)
        {
            OffsetPositionTL(ref point, ref size);

            ScalePoint(ref point);
            ScalePoint(ref size);

            await painter.FillEllipse(color, point, size);
        }

        public async Task FillEllipseC(Color color, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);

            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            await painter.FillEllipse(color, centralPoint, size);
        }

        public async Task DrawRectangle(Color color, int penWidth, Vec2 point, Vec2 size)
        {
            OffsetPositionTL(ref point, ref size);

            ScalePoint(ref point);
            ScalePoint(ref size);
            ScaleWidth(ref penWidth);

            await painter.DrawRectangle(color, penWidth, point, size);
        }

        public async Task FillRectangle(Color color, Vec2 point, Vec2 size)
        {
            OffsetPositionTL(ref point, ref size);

            ScalePoint(ref point);
            ScalePoint(ref size);

            await painter.FillRectangle(color, point, size);
        }

        public async Task FillRectangleC(Color color, Vec2 centralPoint, Vec2 size)
        {
            OffsetPosition(ref centralPoint, ref size);
            CenterPoint(ref centralPoint, size);

            ScalePoint(ref centralPoint);
            ScalePoint(ref size);

            await painter.FillRectangle(color, centralPoint, size);
        }

        public async Task DrawString(string s, Color color, Vec2 point, float scale)
        {
            var size = await painter.MeasureString(s, Zoom, scale * Zoom);
            var V2Size = new Vec2(size.X, size.Y);

            OffsetPositionTL(ref point, ref V2Size);

            await painter.DrawString(s, color, point * Zoom, scale * Zoom);
        }

        public async Task DrawStringC(string s, Color color, Vec2 centralPoint, float scale)
        {
            var size = await painter.MeasureString(s, Zoom, scale * Zoom);
            var V2Size = new Vec2(size.X, size.Y);

            OffsetPosition(ref centralPoint, ref V2Size);

            await painter.DrawString(s, color, centralPoint * Zoom - size / 2f, scale * Zoom);
        }

        public Task<Vec2> MeasureString(string s, float scale)
        {
            return painter.MeasureString(s, Zoom, scale * Zoom);
        }
    }
}
