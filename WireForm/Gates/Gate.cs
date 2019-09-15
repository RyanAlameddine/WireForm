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
        [JsonIgnore]
        public GatePin[] Outputs { get; set; }
        public int InputCount { get; set; }

        public Gate(Point Position, GatePin[] Outputs, int InputCount, Dictionary<Point, List<CircuitConnector>> connections)
        {
            this.Position = Position;
            this.Outputs = Outputs;
            this.InputCount = InputCount;

            foreach (GatePin output in Outputs)
            {
                output.Parent = this;
                Point truePosition = MathHelper.Plus(output.StartPoint, Position);
                if (!connections.ContainsKey(truePosition))
                {
                    connections.Add(truePosition, new List<CircuitConnector>());
                }
                connections[truePosition].Add(output);
            }
        }

        protected abstract void draw(Graphics gfx);
        public void Draw(Graphics gfx)
        {
            draw(gfx);
            foreach(var output in Outputs)
            {
                Painter.DrawPin(gfx, MathHelper.Plus(output.StartPoint, Position), Color.DarkBlue);
            }
        }

        protected abstract void compute(BitValue[] inputs);
        public void Compute(BitValue[] inputs)
        {
            if(inputs == null && InputCount == 0 || inputs.Length == InputCount)
            {
                compute(inputs);
            }
            else
            {
                for(int i = 0; i < Outputs.Length; i++)
                {
                    Outputs[i].Value = BitValue.Error;
                }
            }
        }
    }
}
