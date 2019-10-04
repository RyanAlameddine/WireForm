using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    public abstract class Gate
    {
        public Vec2 Position { get; set; }
        public GatePin[] Outputs { get; set; }
        public GatePin[] Inputs { get; set; }

        public HitBox GateCollider { get; set; }

        public Gate(Vec2 Position)
        {
            this.Position = Position;
        }

        /// <summary>
        /// Adds gate pins in Inputs and Outputs to connections
        /// </summary>
        public void AddConnections(Dictionary<Vec2, List<CircuitConnector>> connections)
        {
            foreach (GatePin input in Inputs)
            {
                connections.AddConnection(input);
            }
            foreach (GatePin output in Outputs)
            {
                connections.AddConnection(output);
            }
        }

        protected abstract void draw(Graphics gfx);
        public void Draw(Graphics gfx)
        {
            draw(gfx);
            foreach(var output in Outputs)
            {
                Painter.DrawPin(gfx, output.StartPoint, output.Value);
            }
            foreach (var input in Inputs)
            {
                Painter.DrawPin(gfx, input.StartPoint, input.Value);
            }
        }

        protected abstract void compute();
        public void Compute()
        {
            compute();
        }

        /// <summary>
        /// Refreshes the absolute position of the pins in Inputs and Outputs relative to the changed position of the Gate
        /// </summary>
        public void RefreshLocation()
        {
            foreach (GatePin pin in Inputs)
            {
                pin.RefreshLocation();
            }

            foreach (GatePin pin in Outputs)
            {
                pin.RefreshLocation();
            }
        }
    }
}
