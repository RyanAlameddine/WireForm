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
            Color[] bitColors = wireLine.Data.BitColors();
            DrawWireLine(painter, propogator, wireLine, bitColors);
        }
        public void DrawWireLine(Painter painter, BoardState propogator, WireLine wireLine, Color[] colors)
        {
            Vec2 squareFixerSize;
            if (colors.Length != 1)
            {
                painter.FillRectangleC(Color.Black, wireLine.StartPoint, new Vec2(.18f, .18f));
                painter.FillRectangleC(Color.Black, wireLine.EndPoint  , new Vec2(.18f, .18f));


                painter.DrawLine(Color.Black, 10, wireLine.StartPoint, wireLine.EndPoint);
                Vec2 previousStart = wireLine.StartPoint;
                for (int i = 0; i < colors.Length; i++)
                {
                    Vec2 endPoint = MathHelper.Lerp(wireLine.StartPoint, wireLine.EndPoint, (i + 1f) / colors.Length);
                    painter.DrawLine(colors[i], 6, previousStart, endPoint);
                    previousStart = endPoint;
                }

                squareFixerSize = new Vec2(.14f, .14f);
            }
            else
            {
                painter.DrawLine(colors[0], 10, wireLine.StartPoint, wireLine.EndPoint);

                squareFixerSize = new Vec2(.16f, .16f);
            }

            painter.FillRectangleC(colors[0]                , wireLine.StartPoint, squareFixerSize);
            painter.FillRectangleC(colors[colors.Length - 1], wireLine.EndPoint  , squareFixerSize);
            drawPoint(painter, propogator, wireLine.StartPoint, colors[0]);
            drawPoint(painter, propogator, wireLine.EndPoint, colors[colors.Length - 1]);
        }

        private void drawPoint(Painter painter, BoardState propogator, Vec2 point, Color bitColor)
        {

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

        //TODO MAKE THIS WORK MAGICALLY
        public static void DrawPin(Painter painter, Vec2 position, BitArray values)
        {
            painter.FillEllipseC(values.BitColors()[0], position, new Vec2(.4f, .4f));
        }
    }
}
