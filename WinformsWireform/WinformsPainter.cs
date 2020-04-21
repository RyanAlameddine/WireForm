using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wireform.Circuitry.Utilities;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;

namespace WinformsWireform
{
    /// <summary>
    /// A tool used to draw on the circuit board.
    /// Each painter contains a reference to the current Graphics class
    /// as well as the current zoom value.
    /// Painters also contain an internal offset value which will be applied to all position elements
    /// and a multiplier which determines rotational values of drawn gates
    /// </summary>
    public class WinformsPainter : IPainter
    {
        Graphics gfx;
        float zoom;
        public WinformsPainter(Graphics gfx, float zoom)
        {
            this.gfx = gfx;
            this.zoom = zoom;
        }

        Pen getPen(Color color, int width)
        {
            return new Pen(color, width * zoom / 50f);
        }

        Brush getEmptyBrush(Color color)
        {
            return (new Pen(color, 1)).Brush;
        }

        public void DrawLine(Color color, int penWidth, Vec2 startPoint, Vec2 endPoint)
        {
            gfx.DrawLine(getPen(color, penWidth), startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }

        public void DrawArc(Color color, int penWidth, Vec2 startPoint, Vec2 size, float startAngle, float sweepAngle)
        {
            gfx.DrawArc(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y, startAngle, sweepAngle);
        }

        public void DrawEllipse(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            gfx.DrawEllipse(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillEllipse(Color color, Vec2 startPoint, Vec2 size)
        {
            gfx.FillEllipse(getEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void DrawRectangle(Color color, int penWidth, Vec2 startPoint, Vec2 size)
        {
            gfx.DrawRectangle(getPen(color, penWidth), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void FillRectangle(Color color, Vec2 startPoint, Vec2 size)
        {
            gfx.FillRectangle(getEmptyBrush(color), startPoint.X, startPoint.Y, size.X, size.Y);
        }

        public void DrawString(string s, Color color, Vec2 startPoint, float scaleDivider)
        {
            Font font = new Font(FontFamily.GenericMonospace, zoom / scaleDivider, FontStyle.Bold);
            gfx.DrawString(s, font, new Pen(color, 10).Brush, startPoint.X, startPoint.Y);
        }

        public Vec2 MeasureString(string s, float zoom, float scaleDivider)
        {
            Font font = new Font(FontFamily.GenericMonospace, zoom / scaleDivider, FontStyle.Bold);
            var size = gfx.MeasureString(s, font);
            return new Vec2(size.Width, size.Height);
        }
    }
}
