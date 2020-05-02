using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;

namespace Wireform.Circuitry.Utils
{
    public abstract class Gate : CircuitObject
    {
        //TODO: MAKE COLOR ABLE TO BE A CONSTANT
        //public const Color LineColor = Color.Black;
        public const int PenWidth = 10;

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
                position = value;

                RefreshChildren();
            }
        }

        public GatePin[] Outputs { get; set; }
        public GatePin[] Inputs { get; set; }

        /// <summary>
        /// Hitbox for Gate
        /// This does NOT take into account the multiplayer (gate rotation) but will update 
        /// Gate.HitBox which is relative to the rotation
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

        /// <summary>
        /// The direction which the gate is facing
        /// If you would like to disable rotation, simply override Direction and tag it
        /// with [HideCircuitAttributes]
        /// </summary>
        [CircuitAction("Rotate", 'r')]
        [CircuitProperty(0, 3, true, new[] { "Right", "Down", "Left", "Up" })]
        public virtual Direction Direction { get; set; } = Direction.Right;

        protected Gate(Vec2 Position, Direction direction, BoxCollider localHitbox)
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
                connections.Attach(input);
            }
            foreach (GatePin output in Outputs)
            {
                connections.Attach(output);
            }
        }

        /// <summary>
        /// Removes gate pins in Inputs and Outputs from connections
        /// </summary>
        public override void RemoveConnections(Dictionary<Vec2, List<BoardObject>> connections)
        {
            foreach (GatePin input in Inputs)
            {
                connections.Detatch(input);
            }
            foreach (GatePin output in Outputs)
            {
                connections.Detatch(output);
            }
        }

        protected abstract void Draw(PainterScope painter);
        public void DrawGate(PainterScope painter)
        {
            painter.AppendOffset(StartPoint);

            painter.SetLocalMultiplier(Direction);
            Draw(painter);

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

        /// <summary>
        /// Implementation for gate which should read from Inputs to set Outputs accordingly
        /// </summary>
        protected abstract void Compute();
        public void ComputeGate()
        {
            Compute();
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

        /// <summary>
        /// Helper function that applies func to all Inputs in order, passing along the accumulator (Similar to Aggregate in LINQ).
        /// E.g. to 'and' all inputs, run FoldInputs((accumulator, current) => accumulator & current);
        /// </summary>
        protected BitArray FoldInputs(Func<BitArray, BitArray, BitArray> func)
        {
            if (Inputs.Length == 0) return new BitArray(0);
            BitArray accumulator = Inputs[0].Values;
            for(int i = 1; i < Inputs.Length; i++)
            {
                accumulator = func(accumulator, Inputs[i].Values);
            }
            return accumulator;
        }

        public override void Delete(BoardState propogator)
        {
            propogator.gates.Remove(this);
            RemoveConnections(propogator.Connections);
        }
    }
}
