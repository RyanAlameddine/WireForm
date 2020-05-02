using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Wireform.Circuitry;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Utils;
using Wireform.Input;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.GraphicsUtils
{
    public static class GraphicsManager
    {
        private static float scale = 50f;
        public static float SizeScale
        {
            get => scale;
            set { scale = value > 5 ? value : 5; } 
        }

        public static void Paint(PainterScope painter, Vec2 viewportSize, BoardState state, InputStateManager inputManager)
        {
            //PainterScope painter = new PainterScope(gfx, scale);

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
                gate.DrawGate(painter);
            }

            foreach (WireLine wireLine in state.wires)
            {
                WirePainter.DrawWireLine(painter, state, wireLine);
            }

            inputManager.Draw(state, painter);
        }
    }
}
