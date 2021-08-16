using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Wireform.Circuitry.CircuitAttributes;
using Wireform.Circuitry.Data;
using Wireform.Circuitry.Data.Bits;
using Wireform.GraphicsUtils;
using Wireform.MathUtils;
using Wireform.MathUtils.Collision;
using Wireform.Utils;

namespace Wireform.Circuitry.Utils
{
    public abstract class Gate : CircuitObject
    {
        //TODO: MAKE COLOR ABLE TO BE A CONSTANT
        //public const Color LineColor = Color.Black;
        /// <summary>
        /// The standard pen width for gate lines
        /// </summary>
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

                var (xMult, yMult, flipXY) = Direction.GetMultiplier();
                float x = value.X * xMult;
                float y = value.Y * yMult;
                //Debug.WriteLine(Direction);
                if (flipXY)
                {
                    hitBox = new BoxCollider(y, x, value.Height * yMult, value.Width * xMult);
                }
                else
                {
                    hitBox = new BoxCollider(x, y, value.Width * xMult, value.Height * yMult);
                }
                hitBox.X = StartPoint.X + hitBox.X;
                hitBox.Y = StartPoint.Y + hitBox.Y;
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
        /// The direction which the gate is facing (including flipping).
        /// This is NOT the circuit property Direction. See <see cref="GateDirection"/>
        /// </summary>
        public Direction Direction { get; set; } = Direction.Right;
        /// <summary>
        /// The direction which the gate is facing (ignoring flipping) derived from <see cref="Direction"/>.
        /// If you would like to disable rotation, simply override Direction
        /// and tag it with [HideCircuitAttributes]
        /// </summary>
        [CircuitDropdownAction("Rotate", 'r', true)]
        [CircuitDropdownAction("Rotate (reverse)", 'r', Modifier.Shift, false)]
        [CircuitPropertyDropdown(0, 3, true, new[] { "Right", "Down", "Left", "Up" })]
        protected Direction Facing 
        { 
            get
            {
                return (Direction) ((int)Direction % 4);
            }
            set
            {
                Direction = (Direction) ((int) value - 0 + (((int)Direction) /4)*4);
            }
        }

        /// <summary>
        /// Whether or not the gate is flipped (horizontally or vertically).
        /// If you would like to disable flipping, simply override Flipped and call the base functions.
        /// Then, attach a [HideCircuitAttributes] tag.
        /// </summary>
        [CircuitDropdownAction("Flip", 'f', true)]
        [CircuitPropertyDropdown(0, 1, true, new[] { "false", "true" })]
        //[HideCircuitAttributes]
        public virtual int Flipped 
        {
            get => Direction > Direction.Down ? 0 : 1;
            set
            {
                Direction = (Direction)(((int) Direction + 4) % 8); 
            }
        }

        protected Gate(Vec2 Position, Direction direction, BoxCollider localHitbox)
        {
            Direction = direction;
            this.localHitbox = localHitbox;
            StartPoint = Position;
        }

        /// <summary>
        /// Adds gate pins in Inputs and Outputs to connections
        /// </summary>
        public override void AddConnections(Dictionary<Vec2, List<DrawableObject>> connections)
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
        public override void RemoveConnections(Dictionary<Vec2, List<DrawableObject>> connections)
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

        protected abstract Task DrawGate(PainterScope painter);
        public override async Task Draw(PainterScope painter, BoardState state)
        {
            painter.AppendOffset(StartPoint);

            painter.SetLocalMultiplier(Direction);
            await DrawGate(painter);

            await painter.DrawEllipseC(Color.Red, 3, Vec2.Zero, new Vec2(.1f, .1f));
            //gfx._DrawRectangle(Color.Red, 1, HitBox.X, HitBox.Y, HitBox.Width, HitBox.Height);

            //gfx._DrawEllipseC(Color.Black, 3, StartPoint.X, StartPoint.Y, .01f, .01f);

            foreach (var pin in Outputs.Union(Inputs))
            {
                await pin.Draw(painter, state);
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
            BitArray accumulator = Inputs[0].Values.ToArray();
            for(int i = 1; i < Inputs.Length; i++)
            {
                accumulator = func(accumulator, Inputs[i].Values);
            }
            return accumulator;
        }

        public override void Delete(BoardState state)
        {
            state.Gates.Remove(this);
            RemoveConnections(state.Connections);
        }
    }
}
