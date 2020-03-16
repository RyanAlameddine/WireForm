using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.Circuitry.CircuitAttributes;
using WireForm.MathUtils;

namespace WireForm.Circuitry.Gates.Utilities
{
    public interface IRotatable
    {
        [CircuitProperty(0, 3, true, new[] { "Up", "Right", "Down", "Left" })]
        public Direction Direction { get; set; }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
