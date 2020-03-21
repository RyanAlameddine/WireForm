using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.Circuitry.Data;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Utilities
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

                RefreshChildren();
            }
        }

        public GatePin[] Outputs { get; set; }
        public GatePin[] Inputs { get; set; }

        /// <summary>
        /// Lo
        /// </summary>
        [JsonIgnore]
        private BoxCollider localHitbox;
        public BoxCollider LocalHitbox
        {
            get
            {
                return localHitbox;
            }
            protected set
            {
                localHitbox = value;

                var mult = Direction.GetMultiplier();
                //Debug.WriteLine(Direction);
                if (mult.Y == -1)
                {
                    hitBox = new BoxCollider(value.Y, value.X, value.Height, value.Width * mult.X);
                    mult = new Vec2(1, mult.X);
                }
                else
                {
                    hitBox = new BoxCollider(value.X, value.Y, value.Width * mult.X, value.Height);
                }
                hitBox.X = StartPoint.X + hitBox.X * mult.X;
                hitBox.Y = StartPoint.Y + hitBox.Y * mult.Y;
                hitBox = hitBox.GetNormalized();
            }
        }

        [JsonIgnore]
        private BoxCollider hitBox;
        /// <summary>
        /// Hitbox with rotations and global transformations applied
        /// </summary>
        [JsonIgnore]
        public override BoxCollider HitBox
        {
            get
            {
                return hitBox;
            }
        }

        [CircuitAction("Rotate", System.Windows.Forms.Keys.R)]
        [CircuitProperty(0, 3, true, new[] { "Right", "Down", "Left", "Up" })]
        public virtual Direction Direction { get; set; } = Direction.Right;

        public Gate(Vec2 Position, Direction direction, BoxCollider localHitbox)
        {
            Direction = direction;
            this.localHitbox = localHitbox;
            StartPoint = Position;
        }

        /// <summary>
        /// Adds gate pins in Inputs and Outputs to connections
        /// </summary>
        public override void AddConnections(Dictionary<Vec2, List<BoardObject>> connections)
        {
            RefreshChildren();
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

        protected abstract void draw(PainterScope painter);
        public void Draw(PainterScope painter)
        {
            painter.AppendOffset(StartPoint);

            painter.SetLocalMultiplier(Direction);
            draw(painter);

            painter.DrawEllipseC(Color.Red, 3, Vec2.Zero, new Vec2(.1f, .1f));
            //gfx._DrawRectangle(Color.Red, 1, HitBox.X, HitBox.Y, HitBox.Width, HitBox.Height);

            //gfx._DrawEllipseC(Color.Black, 3, StartPoint.X, StartPoint.Y, .01f, .01f);

            foreach (var output in Outputs)
            {
                WirePainter.DrawPin(painter, output.LocalPoint, output.Values);
            }
            foreach (var input in Inputs)
            {
                WirePainter.DrawPin(painter, input.LocalPoint, input.Values);
            }
        }

        protected abstract void compute();
        public void Compute()
        {
            compute();
        }

        /// <summary>
        /// Refreshes the absolute position of the pins in Inputs and Outputs relative 
        /// to the changed position of the Gate and refreshes the Gate's HitBox.
        /// </summary>
        private void RefreshChildren()
        {
            LocalHitbox = localHitbox;

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
