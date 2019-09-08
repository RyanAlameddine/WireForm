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
            thin = new Pen(Color.DarkGray, 5);
        }

        public void DrawWireLine(Graphics graphics, WireLine wireLine)
        {
            DrawWire(graphics, wireLine.Start, wireLine.End, wireLine.Data.bitValues[0]);
            graphics.DrawEllipse(thin, new Rectangle(wireLine.Start.X * 50 - 10, wireLine.Start.Y * 50 - 10, 20, 20));
            graphics.DrawEllipse(thin, new Rectangle(wireLine.End.X * 50 - 10, wireLine.End.Y * 50 - 10, 20, 20));
        }

        public void DrawWire(Graphics graphics, Point start, Point end, BitValue value)
        {
            start = start.Times(50);
            end = end.Times(50);
            graphics.DrawLine(pen, start, end);
            switch (value)
            {
                case BitValue.Error:
                    graphics.DrawLine(new Pen(Color.Red, 4), start, end);
                    break;
                case BitValue.Nothing:
                    graphics.DrawLine(new Pen(Color.DimGray, 4), start, end);
                    break;
                case BitValue.One:
                    graphics.DrawLine(new Pen(Color.Blue, 4), start, end);
                    break;
                case BitValue.Zero:
                    graphics.DrawLine(new Pen(Color.DarkBlue, 4), start, end);
                    break;
            }
        }
    }
}
