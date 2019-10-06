using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry;
using WireForm.MathUtils;

namespace WireForm.GraphicsUtils
{
    public class Painter
    {

        public void DrawWireLine(Graphics gfx, WireLine wireLine)
        {
            Color bitColor = wireLine.Data.bitValue.BitColor();
            gfx._DrawLine(bitColor, 10, wireLine.StartPoint, wireLine.EndPoint);
            gfx._FillEllipse(bitColor, wireLine.StartPoint.X - .25f, wireLine.StartPoint.Y - .25f, .5f, .5f);
            gfx._FillEllipse(bitColor, wireLine.EndPoint.X - .25f, wireLine.EndPoint.Y - .25f, .5f, .5f);
        }

        public static void DrawPin(Graphics gfx, Vec2 position, BitValue value)
        {
            gfx._DrawEllipse(value.BitColor(), 10, position.X - .2f, position.Y - .2f, .4f, .4f);
        }
    }
}
