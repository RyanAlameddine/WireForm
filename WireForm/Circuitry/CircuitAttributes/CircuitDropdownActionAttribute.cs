using System;
using Wireform.Utils;

namespace Wireform.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Circuit Actions are functions which can be performed on a certain object, often found in the right-click menu.
    /// This attribute should be placed on a property if the property already has a [CircuitPropertyDropdown].
    /// An action will be generated which cycles through the range of the [CircuitPropertyDropdown].
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class CircuitDropdownActionAttribute : CircuitActionBase
    {
        public readonly bool increment;
        public readonly PropertyOverflow behavior;

        /// <param name="increment">true if the action should increment the value, false will decrement</param>
        /// <param name="behavior">Clip will clip the value at the min/max, Wrap will go from min->max and max->min if it overflows</param>
        public CircuitDropdownActionAttribute(string Name, bool increment, PropertyOverflow behavior = PropertyOverflow.Wrap) 
            : base(Name) 
        { 
            this.increment = increment;
            this.behavior = behavior;
        }

        /// <param name="increment">true if the action should increment the value, false will decrement</param>
        /// <param name="behavior">Clip will clip the value at the min/max, Wrap will go from min->max and max->min if it overflows</param>
        public CircuitDropdownActionAttribute(string Name, char hotkey, bool increment, PropertyOverflow behavior = PropertyOverflow.Wrap) 
            : base(Name, hotkey)
        {
            this.increment = increment;
            this.behavior = behavior;
        }

        /// <param name="increment">true if the action should increment the value, false will decrement</param>
        /// <param name="behavior">Clip will clip the value at the min/max, Wrap will go from min->max and max->min if it overflows</param>
        public CircuitDropdownActionAttribute(string Name, char hotkey, Modifier modifiers, bool increment, PropertyOverflow behavior = PropertyOverflow.Wrap) 
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