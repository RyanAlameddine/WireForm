using System.Drawing;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.MathUtils;

namespace WireForm.GraphicsUtils
{
    public class WirePainter
    {

        public void DrawWireLine(Painter painter, BoardState propogator, WireLine wireLine)
        {
            Color bitColor = wireLine.Data.BitColor();
            DrawWireLine(painter, propogator, wireLine, bitColor);
        }
        public void DrawWireLine(Painter painter, BoardState propogator, WireLine wireLine, Color color)
        {
            painter.DrawLine(color, 10, wireLine.StartPoint, wireLine.EndPoint);


            DrawPoint(painter, propogator, wireLine.StartPoint, color);
            DrawPoint(painter, propogator, wireLine.EndPoint, color);
        }

        public void DrawPoint(Painter painter, BoardState propogator, Vec2 point, Color bitColor)
        {

            painter.FillRectangleC(bitColor, point, new Vec2(1/5f, 1/5f));

            ///Draws point in the following cases:
            ///    The point has an amount of connections greater than or less than 2
            ///    The point is attached to a gatePin
            bool draw = true;
            if (propogator.Connections.ContainsKey(point))
            {
                var connections = propogator.Connections[point];
                draw = connections.Count != 2;
                if (!draw)
                {
                    foreach (var connection in connections)
                    {
                        if (connection is GatePin)
                        {
                            draw = true;
                            break;
                        }
                    }
                }
            }

            if (draw)
            {
                painter.FillEllipseC(bitColor, point, new Vec2(.5f, .5f));
            }
        }

        public static void DrawPin(Painter painter, Vec2 position, BitArray values)
        {
            painter.FillEllipseC(values.BitColor(), position, new Vec2(.4f, .4f));
        }
    }
}
