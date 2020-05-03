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
    /// This attribute should be placed on a method with no parameters, or one parameter: the current BoardState.
    /// 
    /// It may also be placed on a property if the property already has a [CircuitProperty].
    /// In this case, an action will be generated which cycles through the range of the [CircuitProperty].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class CircuitActionAttribute : CircuitActionBase
    {
        public CircuitActionAttribute(string Name) : base(Name) { }

        public CircuitActionAttribute(string Name, char hotkey) : base(Name, hotkey) { }

        public CircuitActionAttribute(string Name, char hotkey, Modifier modifiers) : base(Name, hotkey, modifiers) { }
    }
}