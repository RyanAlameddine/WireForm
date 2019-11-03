using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireForm.Circuitry.CircuitObjectActions
{
    /// <summary>
    /// Circuit Actions are functions which can be performed on a certain object, often found in the right-click menu.
    /// 
    /// This attribute must be placed on a method which takes in nothing as a paramter, or the current BoardState
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class CircuitActionAttribute : Attribute
    {
        public string Name { get; private set; }
        public bool RequireRefresh { get; private set; }

        public Keys Hotkey { get; private set; }
        public CircuitActionAttribute(string Name, bool RequireRefresh)
        {
            this.Name = Name;
            this.RequireRefresh = RequireRefresh;
        }

        public CircuitActionAttribute(string Name, bool RequireRefresh, Keys Hotkey)
            : this(Name, RequireRefresh)
        {
            this.Hotkey = Hotkey;
        }

        /// <param name="target">The target object on which the actions are found and run</param>
        public static List<(CircuitActionAttribute attribute, EventHandler action)> GetActions(object target, BoardState state)
        {
            var actions = new List<(CircuitActionAttribute attribute, EventHandler action)>();

            var methods = target.GetType().GetMethods();
            foreach (var method in methods)
            {
                foreach (var attribute in (CircuitActionAttribute[])method.GetCustomAttributes(typeof(CircuitActionAttribute), true))
                {
                    EventHandler action;
                    if (method.GetParameters().Length > 0)
                    {
                        action = (object sender, EventArgs args) => method.Invoke(target, new object[] { state });
                    }
                    else
                    {
                        action = (object sender, EventArgs args) => method.Invoke(target, null);
                    }
                    actions.Add((attribute, action));
                }
            }

            return actions;
        }
    }

}
