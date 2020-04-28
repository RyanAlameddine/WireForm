﻿using System.Drawing;
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
        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint);
        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle);
        public void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size);
        public void FillEllipse(Color color, Vec2 startPoint, Vec2 size);
        public void DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size);
        public void FillRectangle(Color color, Vec2 startPoint, Vec2 size);
        public void DrawString(string s, Color color, Vec2 startPoint, float scaleDivider);
        public Vec2 MeasureString(string s, float zoom, float scaleDivider);
    }
}
