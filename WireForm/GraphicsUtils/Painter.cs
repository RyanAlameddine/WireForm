using System.Drawing;
using Wireform.MathUtils;

namespace Wireform.GraphicsUtils
{
    public abstract class Painter
    {
        public abstract void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint);

        public abstract void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle);

        public abstract void DrawArcC(Color color, int penWidth, Vec2 centralPoint, Vec2 size, float startAngle, float sweepAngle);

        public abstract void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size);

        public abstract void DrawEllipseC(Color color, int penWidth, Vec2 centralPoint, Vec2 size);

        public abstract void FillEllipse(Color color, Vec2 startPoint, Vec2 size);

        public abstract void FillEllipseC(Color color, Vec2 centralPoint, Vec2 size);

        public abstract void DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size);

        public abstract void FillRectangle(Color color, Vec2 startPoint, Vec2 size);

        public abstract void FillRectangleC(Color color, Vec2 centralPoint, Vec2 size);

        public abstract void DrawString(string s, Color color, Vec2 startPoint, float scaleDivider);

        public abstract void DrawStringC(string s, Color color, Vec2 centralPoint, float scaleDivider);
        public abstract Vec2 MeasureString(string s, float zoom, float scaleDivider);
    }
}
