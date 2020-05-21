using System;
using Wireform.Utils;

namespace Wireform.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Circuit Actions are functions which can be performed on a certain object, often found in the right-click menu.
    /// This attribute should be placed on a method with no parameters, or one parameter: the current BoardState.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class CircuitActionAttribute : CircuitActionBase
    {
        public CircuitActionAttribute(string Name) : base(Name) { }

        public CircuitActionAttribute(string Name, char hotkey) : base(Name, hotkey) { }

        public CircuitActionAttribute(string Name, char hotkey, Modifier modifiers) : base(Name, hotkey, modifiers) { }
    }
}