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
            DrawLine(graphics, wireLine.WireStart, wireLine.WireEnd);
            graphics.DrawEllipse(thin, new Rectangle(wireLine.WireStart.X * 50 - 10, wireLine.WireStart.Y * 50 - 10, 20, 20));
            graphics.DrawEllipse(thin, new Rectangle(wireLine.WireEnd.X * 50 - 10, wireLine.WireEnd.Y * 50 - 10, 20, 20));
        }

        public void DrawLine(Graphics graphics, Point start, Point end)
        {
            start = start.Times(50);
            end = end.Times(50);
            graphics.DrawLine(pen, start, end);
        }

    }
}
