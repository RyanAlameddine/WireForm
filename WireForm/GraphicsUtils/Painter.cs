using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.MathUtils;

namespace WireForm.GraphicsUtils
{
    public class Painter
    {

        public void DrawWireLine(Graphics gfx, BoardState propogator, WireLine wireLine)
        {
            Color bitColor = wireLine.Data.BitColor();
            DrawWireLine(gfx, propogator, wireLine, bitColor);
        }
        public void DrawWireLine(Graphics gfx, BoardState propogator, WireLine wireLine, Color color)
        {
            gfx._DrawLine(color, 10, wireLine.StartPoint, wireLine.EndPoint);

            DrawPoint(gfx, propogator, wireLine.StartPoint, color);
            DrawPoint(gfx, propogator, wireLine.EndPoint, color);
        }

        public void DrawPoint(Graphics gfx, BoardState propogator, Vec2 point, Color bitColor)
        {

            gfx._FillRectangleC(bitColor, point.X, point.Y, 1/5f, 1/5f);

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
                gfx._FillEllipseC(bitColor, point.X, point.Y, .5f, .5f);
            }
        }

        public static void DrawPin(Graphics gfx, Vec2 position, BitArray values)
        {
            gfx._FillEllipseC(values.BitColor(), position.X, position.Y, .4f, .4f);
        }
    }
}
