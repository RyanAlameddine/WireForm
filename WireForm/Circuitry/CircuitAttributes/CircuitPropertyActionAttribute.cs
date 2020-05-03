using System;
using System.Collections.Generic;
using System.Reflection;
using Wireform.Circuitry.CircuitAttributes.Utils;
using Wireform.Circuitry.Data;
using Wireform.Input;

namespace Wireform.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Circuit Actions are functions which can be performed on a certain object, often found in the right-click menu.
    /// This attribute should be placed on a property if the property already has a [CircuitProperty].
    /// An action will be generated which cycles through the range of the [CircuitProperty].
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class CircuitPropertyActionAttribute : CircuitActionBase
    {
        public readonly bool increment;
        public readonly PropertyOverflow behavior;

        /// <param name="increment">true if the action should increment the value, false will decrement</param>
        /// <param name="behavior">Clip will clip the value at the min/max, Wrap will go from min->max and max->min if it overflows</param>
        public CircuitPropertyActionAttribute(string Name, bool increment, PropertyOverflow behavior = PropertyOverflow.Wrap) 
            : base(Name) 
        { 
            this.increment = increment;
            this.behavior = behavior;
        }

        /// <param name="increment">true if the action should increment the value, false will decrement</param>
        /// <param name="behavior">Clip will clip the value at the min/max, Wrap will go from min->max and max->min if it overflows</param>
        public CircuitPropertyActionAttribute(string Name, char hotkey, bool increment, PropertyOverflow behavior = PropertyOverflow.Wrap) 
            : base(Name, hotkey)
        {
            this.increment = increment;
            this.behavior = behavior;
        }

        /// <param name="increment">true if the action should increment the value, false will decrement</param>
        /// <param name="behavior">Clip will clip the value at the min/max, Wrap will go from min->max and max->min if it overflows</param>
        public CircuitPropertyActionAttribute(string Name, char hotkey, Modifier modifiers, bool increment, PropertyOverflow behavior = PropertyOverflow.Wrap) 
            : base(Name, hotkey, modifiers)
        {
            this.increment = increment;
            this.behavior = behavior;
        }
    }

    public enum PropertyOverflow
    {
        Wrap,
        Clip
    }
}