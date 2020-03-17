using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Utilities;

namespace WireForm.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Circuit Actions are functions which can be performed on a certain object, often found in the right-click menu.
    /// This attribute usually must be placed on a method which takes in nothing as a paramter, or the current BoardState.
    /// 
    /// It may also be placed on a property if the property already has a [CircuitProperty].
    /// In this case, an action will be generated which cycles through the range of the [CircuitProperty].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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
        public static List<(CircuitActionAttribute attribute, EventHandler action)> GetActions(CircuitObject target, StateStack stateStack, Panel panel)
        {
            var actions = new List<(CircuitActionAttribute attribute, EventHandler action)>();

            //Find and register all methods which are circuit actions
            var methods = target.GetType().GetMethods();
            foreach (var method in methods)
            {
                foreach (var attribute in (CircuitActionAttribute[])method.GetCustomAttributes(typeof(CircuitActionAttribute), true))
                {
                    //If method has IgnoreCircuitAttribute, continue
                    if (method.GetCustomAttributes(typeof(HideCircuitAttributesAttribute), true).Length != 0) continue;
                    
                    string message = GetMessage(attribute, target);

                    EventHandler action;

                    //add the methods to the event handler, including a possible parameter (the current state), and registering the change to the stateStack
                    if (method.GetParameters().Length > 0)
                    {
                        action = (sender, args) =>
                        {
                            method.Invoke(target, new object[] { stateStack.CurrentState });
                            stateStack.RegisterChange(message);
                        };
                    }
                    else
                    {
                        action = (sender, args) =>
                        {
                            method.Invoke(target, null);
                            stateStack.RegisterChange(message);
                        };
                    }
                    action += (sender, args) =>
                    {
                        panel.Refresh();
                    };
                    actions.Add((attribute, action));
                }
            }

            //Find and register all properties which are Circuit Actions
            var properties = target.GetType().GetProperties();
            var circuitProps = new List<CircuitProp>();

            foreach (var property in properties)
            {
                var actionAttribute   = property.GetCustomAttribute<CircuitActionAttribute>(true);
                var propertyAttribute = property.GetCustomAttribute<CircuitPropertyAttribute>(true);
                ///If attribute is not found or if property has an [IgnoreCircuitAttributesAttribute]
                if (actionAttribute == null || property.GetCustomAttribute(typeof(HideCircuitAttributesAttribute), true) != null) continue;
                if (propertyAttribute == null) throw new NotImplementedException("All [CircuitAction] attributes on a property must also have a [CircuitProperty] attribute.");

                string message = GetMessage(actionAttribute, target);

                EventHandler action;

                var prop = new CircuitProp(property, target, propertyAttribute.ValueRange, propertyAttribute.ValueNames, propertyAttribute.RequireRefresh, property.Name);

                action = (sender, args) =>
                {
                    int value = prop.Get();
                    if(++value > prop.valueRange.max)
                        value = prop.valueRange.min;
                    prop.Set(value, stateStack.CurrentState.Connections);
                    stateStack.RegisterChange(message);
                    panel.Refresh();
                };

                actions.Add((actionAttribute, action));
            }

            return actions;
        }
        private static string GetMessage(CircuitActionAttribute attribute, CircuitObject target)
        {
            //Append necessary info to the message
            string message = attribute.Name;
            if (target is WireLine)
            {
                message += $" wire at {target.StartPoint}";
            }
            else if (target is Gate)
            {
                message += $" {target.GetType().Name} at {target.StartPoint}";
            }

            return message;
        }
    }
}
