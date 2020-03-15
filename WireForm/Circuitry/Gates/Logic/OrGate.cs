﻿using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates.Logic
{
    class OrGate : Gate
    {
        public OrGate(Vec2 Position)
            : base(Position, new BoxCollider(-2, -1.5f, 3, 3))
        {
            Inputs = new GatePin[] {
                new GatePin(this, new Vec2(-2, -1)),
                new GatePin(this, new Vec2(-2, 1))
            };

            Outputs = new GatePin[] {
                new GatePin(this, new Vec2(1, 0))
            };
        }

        protected override void compute()
        {
            Outputs[0].Values = Inputs[0].Values | Inputs[1].Values;
        }

        protected override void draw(Painter painter)
        {
            painter.DrawArcC(Color.Black, 10, new Vec2(-3.5f - .75f, 0), new Vec2(5, 5), 321,  78);
            painter.DrawArcC(Color.Black, 10, new Vec2(-2.3f       , 2), new Vec2(8, 7), 270,  60);
            painter.DrawArcC(Color.Black, 10, new Vec2(-2.3f       , -2), new Vec2(8, 7), 90 , -60);
        }

        public override CircuitObject Copy()
        {
            return new OrGate(StartPoint);
        }
    }
}
