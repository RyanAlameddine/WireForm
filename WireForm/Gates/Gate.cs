using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireForm.Gates
{
    public abstract class Gate
    {
        public Point Position { get; set; }
        public GatePin[] Outputs { get; set; }
        public GatePin[] Inputs { get; set; }
        public int InputCount { get; set; }

        public Gate(Point Position, int InputCount, Dictionary<Point, List<CircuitConnector>> connections)
        {
            this.Position = Position;
            this.InputCount = InputCount;
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
    }
}
