using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Gates.Utilities;

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

        public Keys Hotkey { get; private set; }
        public CircuitActionAttribute(string Name)
        {
            this.Name = Name;
        }

        public CircuitActionAttribute(string Name, Keys Hotkey)
            : this(Name)
        {
            this.Hotkey = Hotkey;
        }

        /// <param name="target">The target object on which the actions are found and run</param>
        public static List<(CircuitActionAttribute attribute, EventHandler action)> GetActions(CircuitObject target, StateStack stateStack, Form form)
        {
            var actions = new List<(CircuitActionAttribute attribute, EventHandler action)>();

            var methods = target.GetType().GetMethods();
            foreach (var method in methods)
            {
                foreach (var attribute in (CircuitActionAttribute[])method.GetCustomAttributes(typeof(CircuitActionAttribute), true))
                {
                    EventHandler action;

                    string message = attribute.Name;
                    if (target is WireLine)
                    {
                        message += $" wire at {target.StartPoint}";
                    }
                    else if (target is Gate)
                    {
                        message += $" {target.GetType().Name} at {target.StartPoint}";
                    }

                    ///add the methods to the event handler, including a possible parameter (the current state), and registering the change to the stateStack
                    if (method.GetParameters().Length > 0)
                    {
                        action = (object sender, EventArgs args) =>
                        {
                            method.Invoke(target, new object[] { stateStack.CurrentState });
                            stateStack.RegisterChange(stateStack.CurrentState, message);
                        };
                    }
                    else
                    {
                        action = (object sender, EventArgs args) =>
                        {
                            method.Invoke(target, null);
                            stateStack.RegisterChange(stateStack.CurrentState, message);
                        };
                    }
                    action += (object sender, EventArgs e) =>
                    {
                        form.Refresh();
                    };
                    actions.Add((attribute, action));
                }
            }

            return actions;
        }
    }

}
