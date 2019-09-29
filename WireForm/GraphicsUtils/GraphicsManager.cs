using System.Drawing;
using WireForm.MathUtils;

namespace WireForm.GraphicsUtils
{
    public static class GraphicsManager
    {
        public static float SizeScale = 50f;
        public static void _DrawLine(this Graphics gfx, Color color, int penWidth, float x1, float y1, float x2, float y2)
        {
            gfx.DrawLine(new Pen(color, penWidth * SizeScale / 50f), x1 * SizeScale, y1 * SizeScale, x2 * SizeScale, y2 * SizeScale);
        }

        public static void _DrawLine(this Graphics gfx, Color color, int penWidth, Vec2 pt1, Vec2 pt2)
        {
            gfx.DrawLine(new Pen(color, penWidth * SizeScale / 50f), (Point)(pt1 * SizeScale), (Point)(pt2 * SizeScale));
        }

        public static void _DrawArc(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            gfx.DrawArc(new Pen(color, penWidth * SizeScale / 50f), x * SizeScale, y * SizeScale, width * SizeScale, height * SizeScale, startAngle, sweepAngle);
        }

        public static void _DrawEllipse(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawEllipse(new Pen(color, penWidth * SizeScale / 50f), x * SizeScale, y * SizeScale, width * SizeScale, height * SizeScale);
        }

        public static void _FillEllipse(this Graphics gfx, Color color, float x, float y, float width, float height)
        {
            gfx.FillEllipse(new Pen(color, 1).Brush, x * SizeScale, y * SizeScale, width * SizeScale, height * SizeScale);
        }

        public static void _DrawRectangle(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawRectangle(new Pen(color, penWidth * SizeScale / 50f), x * SizeScale, y * SizeScale, width * SizeScale, height * SizeScale);
        }
    }
}
