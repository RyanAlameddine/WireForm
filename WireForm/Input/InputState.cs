using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.GraphicsUtils;
using WireForm.MathUtils;

namespace WireForm.Input
{
    internal abstract class InputState
    {
        /// <summary>
        /// Position of the mouse in local coordinates rounded to the nearest grid point
        /// </summary>
        protected Vec2 LocalGridPoint(Vec2 mousePosition)
        {
            return ((mousePosition + (GraphicsManager.SizeScale / 2f)) * (1 / GraphicsManager.SizeScale)).ToInts();
        }

        /// <summary>
        /// Position of the mouse in local coordinates
        /// </summary>
        protected Vec2 LocalPoint(Vec2 mousePosition)
        {
            return mousePosition * (1 / GraphicsManager.SizeScale);
        }

        public virtual void Draw(BoardState currentState, PainterScope painter) { }

        public virtual InputReturns MouseDown(Form1 form, Vec2 mousePosition, MouseButtons mouseButton) => (false, this);
        public virtual InputReturns MouseMove(Form1 form, Vec2 mousePosition) => (false, this);
        public virtual InputReturns MouseUp  (Form1 form, Vec2 mousePosition) => (false, this);
    }

    internal struct InputReturns
    {
        public readonly bool toRefresh;
        public readonly InputState state;

        public InputReturns(bool toRefresh, InputState state)
        {
            this.toRefresh = toRefresh;
            this.state = state;
        }

        public static implicit operator InputReturns((bool toRefresh, InputState state) returnValue)
        {
            return new InputReturns(returnValue.toRefresh, returnValue.state);
        }
    }
}
