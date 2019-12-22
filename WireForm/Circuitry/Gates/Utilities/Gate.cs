using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Data;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates.Utilities
{
    public abstract class Gate : CircuitObject
    {
        [JsonIgnore]
        private Vec2 position;
        /// <summary>
        /// Position of the center of the Gate
        /// </summary>
        public override Vec2 StartPoint
        {
            get
            {
                return position;
            }
            set
            {
                Vec2 offset = value - position;
                position = value;
                HitBox.X += offset.X;
                HitBox.Y += offset.Y;

                RefreshPins();
            }
        }

        public GatePin[] Outputs { get; set; }
        public GatePin[] Inputs { get; set; }

        [JsonIgnore]
        private BoxCollider hitBox;
        [JsonIgnore]
        public override BoxCollider HitBox {
            get
            {
                return hitBox;
            }
            set
            {
                hitBox = value;
                hitBox.X += StartPoint.X;
                hitBox.Y += StartPoint.Y;
            }
        }

        public Gate(Vec2 Position, BoxCollider HitBox)
        {
            this.HitBox = HitBox;
            this.StartPoint = Position;
        }

        /// <summary>
        /// Adds gate pins in Inputs and Outputs to connections
        /// </summary>
        public override void AddConnections(Dictionary<Vec2, List<BoardObject>> connections)
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

        /// <summary>
        /// Removes gate pins in Inputs and Outputs from connections
        /// </summary>
        public override void RemoveConnections(Dictionary<Vec2, List<BoardObject>> connections)
        {
            foreach (GatePin input in Inputs)
            {
                connections.RemoveConnection(input);
            }
            foreach (GatePin output in Outputs)
            {
                connections.RemoveConnection(output);
            }
        }

        protected abstract void draw(Graphics gfx);
        public void Draw(Graphics gfx)
        {

            draw(gfx);
            //gfx._DrawRectangle(Color.Red, 1, HitBox.X, HitBox.Y, HitBox.Width, HitBox.Height);

            //gfx._DrawEllipseC(Color.Black, 3, StartPoint.X, StartPoint.Y, .01f, .01f);

            foreach (var output in Outputs)
            {
                Painter.DrawPin(gfx, output.StartPoint, output.Values);
            }
            foreach (var input in Inputs)
            {
                Painter.DrawPin(gfx, input.StartPoint, input.Values);
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
        private void RefreshPins()
        {
            if (Inputs == null)
            {
                //Debug.WriteLine("Pin Inputs null when refreshing");
            }
            else
            {
                foreach (GatePin pin in Inputs)
                {
                    pin.RefreshLocation();
                }
            }
            if (Outputs == null)
            {
                //Debug.WriteLine("Pin Outputs null when refreshing");
            }
            else
            {
                foreach (GatePin pin in Outputs)
                {
                    pin.RefreshLocation();
                }
            }
        }

        public override void Delete(BoardState propogator)
        {
            propogator.gates.Remove(this);
            RemoveConnections(propogator.Connections);
        }

        //public override CircuitObject Copy()
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
