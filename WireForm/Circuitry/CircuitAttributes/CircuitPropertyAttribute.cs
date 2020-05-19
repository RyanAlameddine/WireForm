using System;
using System.Collections.Generic;
using System.Reflection;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;

namespace Wireform.Circuitry.CircuitAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CircuitPropertyAttribute : Attribute
    {
        public readonly (int min, int max) ValueRange;
        public readonly string[] ValueNames;
        public readonly bool RequireReconnect;

        /// <param name="RequireReconnect">true if the Circuit Property requires the gate to be reconnected (connections reset) on edit</param>
        public CircuitPropertyAttribute(int min, int max, bool RequireReconnect)
        {
            this.RequireReconnect = RequireReconnect;
            ValueRange = (min, max);
            int valueCount = max - min + 1;
            ValueNames = new string[valueCount];
            for (int i = 0; i < valueCount; i++)
            {
                ValueNames[i] = (i + min).ToString();
            }
        }

        /// <param name="RequireReconnect">true if the Circuit Property requires the gate to be reconnected (connections reset) on edit. Usually only true if the property affects the gatepins at runtime</param>
        public CircuitPropertyAttribute(int min, int max, bool RequireReconnect, string[] ValueNames)
        {
            this.RequireReconnect = RequireReconnect;
            ValueRange = (min, max);
            this.ValueNames = ValueNames;
            if (ValueRange.max < ValueRange.min)
            {
                throw new Exception("Max value range is lower than min value range");
            }
            if (ValueNames.Length != ValueRange.max - ValueRange.min + 1)
            {
                throw new Exception("ValueRange is not of the same length as specified ValueNames array");
            }
        }
    }

    /// <summary>
    /// A processed representation of a property with a [CircuitProperty]
    /// Includes the Get and Set methods
    /// </summary>
    public readonly struct CircuitProp
    {
        private readonly Func<int?> getter;
        private readonly Action<int, Dictionary<Vec2, List<DrawableObject>>> setter;

        public readonly CircuitObject circuitObject;
        public readonly (int min, int max) valueRange;
        public readonly string[] valueNames;
        public readonly bool RequireReconnect;
        public readonly string Name;

        internal CircuitProp(Func<int?> getter, Action<int, Dictionary<Vec2, List<DrawableObject>>> setter, CircuitObject circuitObject, (int min, int max) valueRange, string[] valueNames, bool RequireReconnect, string Name)
        {
            this.getter = getter;
            this.setter = setter;
            this.circuitObject = circuitObject;
            this.valueRange = valueRange;
            this.valueNames = valueNames;
            this.Name = Name;
            this.RequireReconnect = RequireReconnect;
        }

        internal int? Get()
        {
            return getter();
        }

        internal void Set(int value, Dictionary<Vec2, List<DrawableObject>> connections)
        {
            if (value < valueRange.min || value > valueRange.max)
            {
                throw new Exception("Selected value is not in range");
            }
            if (RequireReconnect)
            {
                circuitObject.RemoveConnections(connections);
                setter(value, connections);
                circuitObject.AddConnections(connections);
            }
            else
            {
                setter(value, connections);
            }
        }

        public string GetValueName(int value)
        {
            return valueNames[value - valueRange.min];
        }

        public override bool Equals(object obj)
        {
            return obj is CircuitProp prop &&
                   EqualityComparer<Func<int?>>.Default.Equals(getter, prop.getter) &&
                   EqualityComparer<Action<int, Dictionary<Vec2, List<DrawableObject>>>>.Default.Equals(setter, prop.setter) &&
                   valueRange.Equals(prop.valueRange) &&
                   EqualityComparer<string[]>.Default.Equals(valueNames, prop.valueNames) &&
                   Name == prop.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -224463829;
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<int?>>.Default.GetHashCode(getter);
            hashCode = hashCode * -1521134295 + EqualityComparer<Action<int, Dictionary<Vec2, List<DrawableObject>>>>.Default.GetHashCode(setter);
            hashCode = hashCode * -1521134295 + valueRange.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(valueNames);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public static bool operator ==(CircuitProp left, CircuitProp right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CircuitProp left, CircuitProp right)
        {
            return !(left == right);
        }

    }
}