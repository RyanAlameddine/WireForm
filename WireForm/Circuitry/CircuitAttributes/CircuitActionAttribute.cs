﻿using System;
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

        /// <summary>
        /// Reflectively loads all [CircuitActions] on a target CircuitObject.
        /// </summary>
        /// <param name="cleanup">Function to be run after the action is invoked (refresh selections)</param>
        public static List<CircuitAct> GetActions(CircuitObject target, Action<BoardState> cleanup, Action<string> registerChange)
        {
            var actions = new List<CircuitAct>();

            //Find and register all methods which are circuit actions
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                foreach (var attribute in (CircuitActionAttribute[])method.GetCustomAttributes(typeof(CircuitActionAttribute), true))
                {
                    //If method has IgnoreCircuitAttribute, continue
                    if (method.GetCustomAttributes(typeof(HideCircuitAttributesAttribute), true).Length != 0) continue;

                    void action(BoardState state)
                    {
                        //Invoke method with or without state as parameter
                        method.Invoke(target, method.GetParameters().Length == 1 ? new[] { state } : null);

                        cleanup(state);
                        registerChange($"Executed action {attribute.Name} on selection");
                    }
                    actions.Add(new CircuitAct(action, attribute));
                }
            }

            //Find and register all properties which are Circuit Actions
            var properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var circuitProps = new List<CircuitProp>();

            foreach (var property in properties)
            {
                var actionAttribute = property.GetCustomAttribute<CircuitActionAttribute>(true);
                var propertyAttribute = property.GetCustomAttribute<CircuitPropertyAttribute>(true);
                //If attribute is not found or if property has an [IgnoreCircuitAttributesAttribute]
                if (actionAttribute == null || property.GetCustomAttribute(typeof(HideCircuitAttributesAttribute), true) != null) continue;
                if (propertyAttribute == null) throw new NotImplementedException("All [CircuitAction] attributes on a property must also have a [CircuitProperty] attribute.");

                var prop = new CircuitProp(property, target, propertyAttribute.ValueRange, propertyAttribute.ValueNames, propertyAttribute.RequireReconnect, property.Name);

                //Increments the property's value and resets to valueRange.min if it goes over the valueRange.max
                void action(BoardState state)
                {
                    int value = prop.Get();
                    if (++value > prop.valueRange.max)
                        value = prop.valueRange.min;
                    prop.Set(value, state.Connections);
                    cleanup(state);
                    registerChange($"Executed action {actionAttribute.Name} on selection");
                }

                actions.Add(new CircuitAct(action, actionAttribute));
            }

            return actions;
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

        public void Invoke(BoardState state)
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