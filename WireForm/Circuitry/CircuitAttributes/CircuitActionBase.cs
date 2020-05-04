using System;
using System.Collections.Generic;
using Wireform.Circuitry.Data;
using Wireform.Utils;

namespace Wireform.Circuitry.CircuitAttributes
{
    public abstract class CircuitActionBase : Attribute
    {
        public string Name { get; }

        public char Hotkey { get; }
        public Modifier Modifiers { get; }
        internal CircuitActionBase(string Name)
        {
            this.Name = Name;
        }

        internal CircuitActionBase(string Name, char hotkey)
            : this(Name)
        {
            this.Hotkey = hotkey.ToString().ToLower()[0];
        }

        internal CircuitActionBase(string Name, char hotkey, Modifier modifiers)
            : this(Name, hotkey)
        {
            this.Modifiers = modifiers;
        }
    }

    public readonly struct CircuitAct
    {
        private readonly Action<BoardState> action;

        public readonly string Name;
        public readonly char Hotkey;
        public readonly Modifier Modifiers;

        internal CircuitAct(Action<BoardState> action, CircuitActionBase attribute)
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
