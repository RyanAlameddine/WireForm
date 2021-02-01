using System.Drawing;
using System.Threading.Tasks;
using Wireform.MathUtils;

namespace Wireform.GraphicsUtils
{
    /// <summary>
    /// Interface which should be implemented for each graphical implementation of Wireform.
    /// When not otherwise specified, each function will pass in a Vec2 which represents the coordinate
    /// at the TOP LEFT of the bounding box of the object.
    /// </summary>
    public interface IPainter
    {                     
        Task DrawLine     (Color color, int penWidth, Vec2 point, Vec2 endPoint);
        Task DrawArc      (Color color, int penWidth, Vec2 point, Vec2 size, float startAngle, float sweepAngle);
        Task DrawEllipse  (Color color, int penWidth, Vec2 point, Vec2 size);
        Task FillEllipse  (Color color, Vec2 point, Vec2 size);
        Task DrawRectangle(Color color, int penWidth, Vec2 point, Vec2 size);
        Task FillRectangle(Color color, Vec2 point, Vec2 size);
        Task DrawString   (string s, Color color, Vec2 point, float scaleDivider);
        Task<Vec2> MeasureString(string s, float zoom, float scaleDivider);
    }
}
