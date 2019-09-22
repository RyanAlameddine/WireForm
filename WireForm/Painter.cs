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
            DrawWire(graphics, wireLine.StartPoint, wireLine.EndPoint, wireLine.Data.bitValue);
            graphics.DrawEllipse(thin, new Rectangle(wireLine.StartPoint.X * 50 - 10, wireLine.StartPoint.Y * 50 - 10, 20, 20));
            graphics.DrawEllipse(thin, new Rectangle(wireLine.EndPoint.X * 50 - 10, wireLine.EndPoint.Y * 50 - 10, 20, 20));
        }

        public void DrawWire(Graphics graphics, Point start, Point end, BitValue value)
        {
            start = start.Times(50);
            end = end.Times(50);
            graphics.DrawLine(pen, start, end);
            switch (value)
            {
                case BitValue.Error:
                    graphics.DrawLine(new Pen(Color.DarkRed, 4), start, end);
                    break;
                case BitValue.Nothing:
                    graphics.DrawLine(new Pen(Color.DimGray, 4), start, end);
                    break;
                case BitValue.One:
                    graphics.DrawLine(new Pen(Color.MediumBlue, 4), start, end);
                    break;
                case BitValue.Zero:
                    graphics.DrawLine(new Pen(Color.Navy, 4), start, end);
                    break;
            }
        }

        public static void DrawGate(Graphics graphics, Point position, Color color)
        {
            position = position.Times(50);
            graphics.DrawRectangle(new Pen(color, 5), position.X - 10, position.Y - 10, 20, 20);
        }
        public static void DrawPin(Graphics graphics, Point position, BitValue value)
        {
            position = position.Times(50);
            switch (value)
            {
                case BitValue.Error:
                    graphics.DrawEllipse(new Pen(Color.DarkRed, 5), position.X - 5, position.Y - 5, 10, 10);
                    break;
                case BitValue.Nothing:
                    graphics.DrawEllipse(new Pen(Color.DimGray, 5), position.X - 5, position.Y - 5, 10, 10);
                    break;
                case BitValue.One:
                    graphics.DrawEllipse(new Pen(Color.MediumBlue, 5), position.X - 5, position.Y - 5, 10, 10);
                    break;
                case BitValue.Zero:
                    graphics.DrawEllipse(new Pen(Color.Navy, 5), position.X - 5, position.Y - 5, 10, 10);
                    break;
            }
        }
    }
}
