using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.GraphicsUtils
{
    public static class GraphicsManager
    {
        private static float scale = 50f;
        public static float SizeScale
        {
            get
            {
                return scale;
            }
            set
            {
                if(value > 5)
                {
                    scale = value;
                }
                else
                {
                    scale = 5;
                }
            }
        }

        public static void Paint(Graphics gfx, WirePainter wirePainter, Vec2 viewportSize,
            HashSet<BoxCollider> collisions, HashSet<CircuitObject> selections, BoxCollider mouseBox, HashSet<BoxCollider> resetBoxes,
            BoardState state)
        {
            Painter painter = new Painter(gfx, scale);

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
                wirePainter.DrawWireLine(painter, state, wireLine);
            }

            foreach (BoxCollider collision in collisions)
            {
                painter.FillRectangle(Color.FromArgb(128, 255, 0, 0), collision.Position, collision.Bounds);
            }

            foreach (CircuitObject selection in selections)
            {
                Gate asGate = selection as Gate;
                WireLine asWire = selection as WireLine;
                if(asGate != null) 
                { 
                    asGate.Draw(painter);
                    BoxCollider selectionBox = selection.HitBox;

                    painter.DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.Position, selectionBox.Bounds);

                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selectionBox.Position                                                              , new Vec2(.4f, .4f));
                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, new Vec2(selectionBox.X + selectionBox.Width, selectionBox.Y                      ), new Vec2(.4f, .4f));
                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, new Vec2(selectionBox.X                     , selectionBox.Y + selectionBox.Height), new Vec2(.4f, .4f));
                    painter.DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, new Vec2(selectionBox.X + selectionBox.Width, selectionBox.Y + selectionBox.Height), new Vec2(.4f, .4f));
                }
                if(asWire != null)
                {
                    BoxCollider selectionBox = selection.HitBox;

                    painter.DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.Position, selectionBox.Bounds);
                    wirePainter.DrawWireLine(painter, state, asWire, new[] { Color.FromArgb(255, 0, 128, 128) }); 
                }
            }
            
            //foreach(var key in state.Connections.Keys)
            //{
            //    foreach(var value in state.Connections[key])
            //    {
            //        if (value is GatePin)
            //        {
            //            gfx._DrawEllipseC(Color.FromArgb(255, 0, 128, 128), 5, key.X, key.Y, .5f, .5f);
            //        }
            //    }
            //}

            if (mouseBox != null)
            {
                BoxCollider newBox = mouseBox.GetNormalized();
                painter.DrawRectangle(Color.FromArgb(255, 0, 0, 255), 3, newBox.Position, newBox.Bounds);
                painter.FillRectangle(Color.FromArgb(64, 128, 128, 255), newBox.Position, newBox.Bounds);
            }

            if (collisions.Count > 0)
            {
                foreach (var resetBox in resetBoxes)
                {
                    painter.FillRectangle(Color.FromArgb(64, 128, 128, 255), resetBox.Position, resetBox.Bounds);
                }
            }
        }
    }
}
