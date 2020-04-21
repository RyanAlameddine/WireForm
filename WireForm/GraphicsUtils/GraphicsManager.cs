using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.Input;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.GraphicsUtils
{
    internal static class GraphicsManager
    {
        private static float scale = 50f;
        public static float SizeScale
        {
            get => scale;
            set { scale = value > 5 ? value : 5; } 
        }

        public static void Paint(Graphics gfx, Vec2 viewportSize, BoardState state, InputStateManager inputManager)
        {
            PainterScope painter = new PainterScope(gfx, scale);

            int xSize = (int)(viewportSize.X / SizeScale);
            int step = MathHelper.Ceiling(xSize / 50f);


            for (int x = 0; x * SizeScale < viewportSize.X; x += step)
            {
                for (int y = 0; y * SizeScale < viewportSize.Y; y += step)
                {
                    painter.FillRectangleC(Color.DarkBlue, new Vec2(x, y), new Vec2(.05f * step, .05f * step));
                }
            }

            foreach (Gate gate in state.gates)
            {
                gate.Draw(painter);
            }

            foreach (WireLine wireLine in state.wires)
            {
                WirePainter.DrawWireLine(painter, state, wireLine);
            }

            inputManager.Draw(state, painter);
        }
    }
}
