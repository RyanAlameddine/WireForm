using System.Drawing;
using Wireform.MathUtils;

namespace Wireform.GraphicsUtils
{
    public interface IPainter
    {
        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint);

        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle);

        //public void DrawArcC(Color color, int penWidth, Vec2 centralPoint, Vec2 size, float startAngle, float sweepAngle);

        public void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size);

        //public void DrawEllipseC(Color color, int penWidth, Vec2 centralPoint, Vec2 size);

        public void FillEllipse(Color color, Vec2 startPoint, Vec2 size);

        //public void FillEllipseC(Color color, Vec2 centralPoint, Vec2 size);

        public void DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size);

        public void FillRectangle(Color color, Vec2 startPoint, Vec2 size);

        //public void FillRectangleC(Color color, Vec2 centralPoint, Vec2 size);

        public void DrawString(string s, Color color, Vec2 startPoint, float scaleDivider);

        //public void DrawStringC(string s, Color color, Vec2 centralPoint, float scaleDivider);
        public Vec2 MeasureString(string s, float zoom, float scaleDivider);
    }
}
