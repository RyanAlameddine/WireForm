﻿using Newtonsoft.Json;
using System.Diagnostics;
using WireForm.MathUtils;

namespace WireForm.Circuitry.Gates.Utilities
{
    public class GatePin : CircuitConnector
    {
        [JsonIgnore]
        public override Vec2 StartPoint { get; set; }
        Vec2 localPoint;
        public Vec2 LocalPoint
        {
            get
            {
                return localPoint;
            }
            set
            {
                localPoint = value;

                if(Parent == null)
                {
                    Debug.WriteLine("Null parent in gatepin initialization - If this occurs while loading, this is not an error");
                    return;
                }

                StartPoint = MathHelper.Plus(value, Parent.Position);

            }
        }
        [JsonIgnore]
        public BitValue Value { get; set; }
        public Gate Parent { get; set; }
        public GatePin(Gate Parent, Vec2 LocalStart, BitValue value)
        {
            this.Parent = Parent;

            this.LocalPoint = LocalStart;
            this.Value = Value;
        }

        public void RefreshLocation()
        {
            StartPoint = localPoint + Parent.Position;
        }
    }
}