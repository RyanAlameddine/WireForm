﻿using System.Collections.Generic;
using System.Drawing;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;
using WireForm.MathUtils.Collision;

namespace WireForm.Circuitry.Gates
{
    class NandGate : Gate
    {
        public NandGate(Vec2 Position)
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
            Outputs[0].Values = Inputs[0].Values.And(Inputs[1].Values).Not();
        }

        protected override void draw(Graphics gfx)
        {
            gfx._DrawLine(Color.Black, 10, StartPoint + new Vec2(-2, 1.5f), StartPoint + new Vec2(-2, -1.5f));

            gfx._DrawLine(Color.Black, 10, StartPoint + new Vec2(-2, 1.5f), StartPoint + new Vec2(-.5f + .1f, 1.5f));
            gfx._DrawLine(Color.Black, 10, StartPoint + new Vec2(-2, -1.5f), StartPoint + new Vec2(-.5f + .1f, -1.5f));
            gfx._DrawArc(Color.Black, 10, StartPoint.X - 2f, StartPoint.Y - 1.5f, 3f, 3f, 270, 180);

            gfx._DrawEllipseC(Color.Black, 10, StartPoint.X + 1, StartPoint.Y, .4f, .4f);
        }

        public override CircuitObject Copy()
        {
            return new NandGate(StartPoint);
        }
    }
}
