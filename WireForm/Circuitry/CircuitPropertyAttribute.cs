using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;

namespace WireForm.Circuitry
{
    /// <summary>
    /// Circuit Actions are functions which can be performed on a certain object, often found in the right-click menu.
    /// 
    /// This attribute must be placed on a method which takes in nothing as a paramter, or the current BoardState
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CircuitPropertyAttribute : Attribute
    {

        public CircuitPropertyAttribute()
        {

        }

    }

}
