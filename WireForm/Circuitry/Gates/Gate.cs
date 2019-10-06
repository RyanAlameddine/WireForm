﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    public abstract class Gate
    {
        [JsonIgnore]
        private Vec2 position;
        public Vec2 Position 
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
        public BoxCollider HitBox { get; set; }

        public Gate(Vec2 Position, BoxCollider HitBox)
        {
            this.HitBox = HitBox;
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
        private void RefreshPins()
        {
            if (Inputs == null)
            {
                Debug.WriteLine("Pin Inputs null when refreshing");
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
                Debug.WriteLine("Pin Outputs null when refreshing");
            }
            else
            {
                foreach (GatePin pin in Outputs)
                {
                    pin.RefreshLocation();
                }
            }
        }
    }
}
