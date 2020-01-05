using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;
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

        public static void Paint(Graphics gfx, WirePainter painter, Vec2 viewportSize,
            HashSet<BoxCollider> collisions, HashSet<CircuitObject> selections, BoxCollider mouseBox, HashSet<BoxCollider> resetBoxes,
            BoardState state)
        {
            

            foreach (Gate gate in state.gates)
            {
                gate.Draw(gfx);
            }

            foreach (WireLine wireLine in state.wires)
            {
                painter.DrawWireLine(gfx, state, wireLine);
            }

            foreach (BoxCollider collision in collisions)
            {
                gfx._FillRectangle(Color.FromArgb(128, 255, 0, 0), collision.X, collision.Y, collision.Width, collision.Height);
            }

            foreach (CircuitObject selection in selections)
            {
                Gate asGate = selection as Gate;
                WireLine asWire = selection as WireLine;
                if(asGate != null) 
                { 
                    asGate.Draw(gfx);
                    BoxCollider selectionBox = selection.HitBox;

                    gfx._DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.X, selectionBox.Y, selectionBox.Width, selectionBox.Height);
                    gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selectionBox.X, selectionBox.Y, .4f, .4f);
                    gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selectionBox.X + selectionBox.Width, selectionBox.Y, .4f, .4f);
                    gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selectionBox.X, selectionBox.Y + selectionBox.Height, .4f, .4f);
                    gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selectionBox.X + selectionBox.Width, selectionBox.Y + selectionBox.Height, .4f, .4f);
                }
                if(asWire != null)
                {
                    BoxCollider selectionBox = selection.HitBox;

                    gfx._DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selectionBox.X, selectionBox.Y, selectionBox.Width, selectionBox.Height);
                    painter.DrawWireLine(gfx, state, asWire, Color.FromArgb(255, 0, 128, 128)); 
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
                gfx._DrawRectangle(Color.FromArgb(255, 0, 0, 255), 3, newBox.X, newBox.Y, newBox.Width, newBox.Height);
                gfx._FillRectangle(Color.FromArgb(64, 128, 128, 255), newBox.X, newBox.Y, newBox.Width, newBox.Height);

            }

            if (collisions.Count > 0)
            {
                foreach (var resetBox in resetBoxes)
                {
                    gfx._FillRectangle(Color.FromArgb(64, 128, 128, 255), resetBox.X, resetBox.Y, resetBox.Width, resetBox.Height);
                }
            }

            for (int x = 0; x * SizeScale < viewportSize.X; x++)
            {
                for (int y = 0; y * SizeScale < viewportSize.Y; y++)
                {
                    gfx._FillRectangleC(Color.Gray, x, y, .02f, .02f);
                }
            }
        }
    }
}
