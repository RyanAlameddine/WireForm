using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Wireform.Circuitry.CircuitAttributes.Utilities;
using Wireform.Circuitry.Data;

namespace Wireform.Circuitry.CircuitAttributes.Utils
{
    /// <summary>
    /// Contains static functions required to load and process [CircuitAction], [CircuitProperty], and [HideCircuitAttributes]
    /// </summary>
    public static class CircuitAttributes
    {
        /// <summary>
        /// Loads all [CircuitProperties] on the target object
        /// </summary>
        public static CircuitPropertyCollection GetProperties(CircuitObject target, Action<string> registerChange)
        {
            var properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var circuitProps = new HashSet<CircuitProp>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<CircuitPropertyAttribute>(true);
                ///If attribute is not found or if property has an [IgnoreCircuitAttributesAttribute]
                if (attribute == null || property.GetCustomAttribute(typeof(HideCircuitAttributesAttribute), true) != null) continue;

                //create CircuitProp
                CircuitProp prop = new CircuitProp(
                    () => (int) property.GetValue(target),
                    (value, connections) => property.SetValue(target, value),
                    target, attribute.ValueRange, attribute.ValueNames, attribute.RequireReconnect, property.Name);

                circuitProps.Add(prop);
            }
            return new CircuitPropertyCollection(circuitProps, registerChange);
        }

        /// <summary>
        /// Reflectively loads all [CircuitActions] on a target CircuitObject.
        /// </summary>
        /// <param name="cleanup">Function to be run after the action is invoked (refresh selections)</param>
        public static CircuitActionCollection GetActions(CircuitObject target, Action<BoardState> cleanup, Action<string> registerChange)
        {
            var actions = new HashSet<CircuitAct>();

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

                CircuitProp prop = new CircuitProp(
                    () => (int)property.GetValue(target),
                    (value, connections) => property.SetValue(target, value),
                    target, propertyAttribute.ValueRange, propertyAttribute.ValueNames, propertyAttribute.RequireReconnect, property.Name);

                //Increments the property's value and resets to valueRange.min if it goes over the valueRange.max
                void action(BoardState state)
                {
                    int value = prop.Get();
                    if (++value > prop.valueRange.max)
                        value = prop.valueRange.min;
                    prop.Set(value, state.Connections);
                    cleanup(state);
                }

                actions.Add(new CircuitAct(action, actionAttribute));
            }

            return new CircuitActionCollection(actions, registerChange);
        }
    }
}
