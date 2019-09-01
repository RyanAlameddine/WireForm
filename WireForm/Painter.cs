using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm
{
    class Painter
    {
        Pen pen;
        Pen thin;

        public Painter()
        {
            pen = new Pen(Color.Black, 10);
            thin = new Pen(Color.Gray, 5);
        }

        public void DrawLine(Graphics graphics, WireLine wireLine)
        {
            DrawLine(graphics, wireLine.Start, wireLine.End);
            graphics.DrawEllipse(thin, new Rectangle(wireLine.Start.X * 50 - 10, wireLine.Start.Y * 50 - 10, 20, 20));
            graphics.DrawEllipse(thin, new Rectangle(wireLine.End.X * 50 - 10, wireLine.End.Y * 50 - 10, 20, 20));
        }

        public void DrawLine(Graphics graphics, Point start, Point end)
        {
            start = start.Times(50);
            end = end.Times(50);
            graphics.DrawLine(pen, start, end);
        }
    }
}
