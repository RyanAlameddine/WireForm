using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry;
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

        public static void PropogateAndPaint(Graphics gfx, Painter painter,
            HashSet<BoxCollider> collisions, HashSet<Gate> selections, BoxCollider mouseBox,
            FlowPropogator propogator)
        {
            Queue<Gate> sources = new Queue<Gate>();
            foreach (Gate gate in propogator.gates)
            {
                if (gate.Inputs.Length == 0)
                {
                    sources.Enqueue(gate);
                }
            }
            propogator.Propogate(sources);

            foreach (Gate gate in propogator.gates)
            {
                gate.Draw(gfx);
            }

            foreach (WireLine wireLine in propogator.wires)
            {
                painter.DrawWireLine(gfx, wireLine);
            }

            foreach (BoxCollider collision in collisions)
            {
                gfx._FillRectangle(Color.FromArgb(128, 255, 0, 0), collision.X, collision.Y, collision.Width, collision.Height);
            }

            foreach (Gate selectedGate in selections)
            {
                selectedGate.Draw(gfx);

                BoxCollider selection = selectedGate.HitBox;
                gfx._DrawRectangle(Color.FromArgb(128, 0, 0, 255), 10, selection.X, selection.Y, selection.Width, selection.Height);
                gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selection.X, selection.Y, .5f, .5f);
                gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selection.X + selection.Width, selection.Y, .5f, .5f);
                gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selection.X, selection.Y + selection.Height, .5f, .5f);
                gfx._DrawEllipseC(Color.FromArgb(255, 0, 0, 255), 5, selection.X + selection.Width, selection.Y + selection.Height, .5f, .5f);
            }
            
            foreach(var key in propogator.Connections.Keys)
            {
                foreach(var value in propogator.Connections[key])
                {
                    gfx._DrawEllipseC(Color.FromArgb(255, 0, 128, 128), 5, key.X, key.Y, .5f, .5f);
                }
            }

            if (mouseBox != null)
            {
                BoxCollider newBox = mouseBox.GetNormalized();
                gfx._DrawRectangle(Color.FromArgb(255, 0, 0, 255), 3, newBox.X, newBox.Y, newBox.Width, newBox.Height);
                gfx._FillRectangle(Color.FromArgb(64, 128, 128, 255), newBox.X, newBox.Y, newBox.Width, newBox.Height);

            }

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    gfx._DrawRectangle(Color.Gray, 1, x, y, .02f, .02f);
                }
            }
        }

        public static void _DrawLine(this Graphics gfx, Color color, int penWidth, float x1, float y1, float x2, float y2)
        {
            gfx.DrawLine(new Pen(color, penWidth * scale / 50f), x1 * scale, y1 * scale, x2 * scale, y2 * scale);
        }

        public static void _DrawLine(this Graphics gfx, Color color, int penWidth, Vec2 pt1, Vec2 pt2)
        {
            gfx.DrawLine(new Pen(color, penWidth * scale / 50f), (Point)(pt1 * scale), (Point)(pt2 * scale));
        }

        public static void _DrawArc(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            gfx.DrawArc(new Pen(color, penWidth * scale / 50f), x * scale, y * scale, width * scale, height * scale, startAngle, sweepAngle);
        }

        public static void _DrawArcC(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            gfx.DrawArc(new Pen(color, penWidth * scale / 50f), (x - width / 2f) * scale, (y - height / 2f) * scale, width * scale, height * scale, startAngle, sweepAngle);
        }

        public static void _DrawEllipse(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawEllipse(new Pen(color, penWidth * scale / 50f), x * scale, y * scale, width * scale, height * scale);
        }

        public static void _DrawEllipseC(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawEllipse(new Pen(color, penWidth * scale / 50f), (x - width / 2f) * scale, (y - height / 2f) * scale, width * scale, height * scale);
        }

        public static void _FillEllipse(this Graphics gfx, Color color, float x, float y, float width, float height)
        {
            gfx.FillEllipse(new Pen(color, 1).Brush, x * scale, y * scale, width * scale, height * scale);
        }

        public static void _FillEllipseC(this Graphics gfx, Color color, float x, float y, float width, float height)
        {
            gfx.FillEllipse(new Pen(color, 1).Brush, (x - width / 2f) * scale, (y - height / 2f) * scale, width * scale, height * scale);
        }

        public static void _DrawRectangle(this Graphics gfx, Color color, int penWidth, float x, float y, float width, float height)
        {
            gfx.DrawRectangle(new Pen(color, penWidth * scale / 50f), x * scale, y * scale, width * scale, height * scale);
        }

        public static void _FillRectangle(this Graphics gfx, Color color, float x, float y, float width, float height)
        {
            gfx.FillRectangle(new Pen(color, 1).Brush, x * scale, y * scale, width * scale, height * scale);
        }
    }
}
