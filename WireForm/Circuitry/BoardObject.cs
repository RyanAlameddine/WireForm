using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireForm.MathUtils;

namespace WireForm.Circuitry
{
    /// <summary>
    /// An object which sits on the circuit board
    /// </summary>
    public abstract class BoardObject
    {
        public abstract Vec2 StartPoint { get; set; }
    }
}
