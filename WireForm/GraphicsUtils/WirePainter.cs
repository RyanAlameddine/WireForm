using System;
using System.Drawing;
using System.Threading.Tasks;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Data.Bits;
using Wireform.MathUtils;

namespace Wireform.GraphicsUtils
{
    internal static class WirePainter
    {
        private const float wireSize = 1.4f;
        public static async Task DrawWireLine(PainterScope painter, BoardState state, WireLine wireLine)
        {
            Color[] bitColors = wireLine.Values.BitColors();

            //ensures start->end vs end<-start wires will display colors in the same order:
            if (wireLine.StartPoint.X > wireLine.EndPoint.X || wireLine.StartPoint.Y > wireLine.EndPoint.Y) Array.Reverse(bitColors);
            
            await DrawWireLine(painter, state, wireLine, bitColors);
        }
        public static async Task DrawWireLine(PainterScope painter, BoardState state, WireLine wireLine, Color[] colors)
        {
            Vec2 squareFixerSize;
            if (colors.Length != 1)
            {
                await painter.FillRectangleC(Color.Black, wireLine.StartPoint, new Vec2(.18f * wireSize, .18f * wireSize));
                await painter.FillRectangleC(Color.Black, wireLine.EndPoint  , new Vec2(.18f * wireSize, .18f * wireSize));


                await painter.DrawLine(Color.Black, (int) (10 * wireSize), wireLine.StartPoint, wireLine.EndPoint);
                Vec2 previousStart = wireLine.StartPoint;
                for (int i = 0; i < colors.Length; i++)
                {
                    Vec2 endPoint = MathHelper.Lerp(wireLine.StartPoint, wireLine.EndPoint, (i + 1f) / colors.Length);
                    await painter.DrawLine(colors[i], (int) (6 * wireSize), previousStart, endPoint);
                    previousStart = endPoint;
                }

                squareFixerSize = new Vec2(.14f * wireSize, .14f * wireSize);
            }
            else
            {
                await painter.DrawLine(colors[0], (int) (10 * wireSize), wireLine.StartPoint, wireLine.EndPoint);

                squareFixerSize = new Vec2(.16f * wireSize, .16f * wireSize);
            }

            await painter.FillRectangleC(colors[0]                , wireLine.StartPoint, squareFixerSize);
            await painter.FillRectangleC(colors[colors.Length - 1], wireLine.EndPoint  , squareFixerSize);
            await DrawPoint(painter, state, wireLine.StartPoint, colors[0]);
            await DrawPoint(painter, state, wireLine.EndPoint, colors[colors.Length - 1]);
        }

        private static async Task DrawPoint(PainterScope painter, BoardState state, Vec2 point, Color bitColor)
        {
            ///Draws point in the following cases:
            ///    The point has an amount of connections greater than or less than 2
            ///    The point is attached to a gatePin
            bool draw = true;
            if (state.Connections.ContainsKey(point))
            {
                var connections = state.Connections[point];
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
                await painter.FillEllipseC(bitColor, point, new Vec2(.5f, .5f));
            }
        }
    }
}
