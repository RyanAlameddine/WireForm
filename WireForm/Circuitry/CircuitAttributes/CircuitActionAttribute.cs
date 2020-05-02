using System;
using System.Collections.Generic;
using System.Reflection;
using Wireform.Circuitry.Data;
using Wireform.Input;

namespace Wireform.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Circuit Actions are functions which can be performed on a certain object, often found in the right-click menu.
    /// This attribute will usually be placed on a method with no parameters, or one parameter: the current BoardState.
    /// 
    /// It may also be placed on a property if the property already has a [CircuitProperty].
    /// In this case, an action will be generated which cycles through the range of the [CircuitProperty].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CircuitActionAttribute : Attribute
    {
        public string Name { get; }

        public char Hotkey { get; }
        public Modifier Modifiers { get; }
        public CircuitActionAttribute(string Name)
        {
            this.Name = Name;
        }

        public CircuitActionAttribute(string Name, char hotkey)
            : this(Name)
        {
            this.Hotkey = hotkey;
        }

        public CircuitActionAttribute(string Name, char hotkey, Modifier modifiers)
            : this(Name)
        {
            this.Hotkey = hotkey;
            this.Modifiers = modifiers;
        }
    }

    public readonly struct CircuitAct
    {
        private readonly Action<BoardState> action;

        public readonly string Name;
        public readonly char Hotkey;
        public readonly Modifier Modifiers;

        internal CircuitAct(Action<BoardState> action, CircuitActionAttribute attribute)
        {
            this.action = action;
            this.Hotkey = attribute.Hotkey;
            this.Modifiers = attribute.Modifiers;
            this.Name = attribute.Name;
        }

        internal void Invoke(BoardState state)
        {
            action(state);
        }

        //auto-generated code:
        public override bool Equals(object obj)
        {
            return obj is CircuitAct act &&
                   EqualityComparer<Action<BoardState>>.Default.Equals(action, act.action) &&
                   Name == act.Name &&
                   Hotkey == act.Hotkey &&
                   Modifiers == act.Modifiers;
        }

        public override int GetHashCode()
        {
            var hashCode = 1150516337;
            hashCode = hashCode * -1521134295 + EqualityComparer<Action<BoardState>>.Default.GetHashCode(action);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Hotkey.GetHashCode();
            hashCode = hashCode * -1521134295 + Modifiers.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CircuitAct left, CircuitAct right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CircuitAct left, CircuitAct right)
        {
            return !(left == right);
        }

    }
}