using System;
using System.Collections.Generic;
using System.Reflection;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;

namespace Wireform.Circuitry.CircuitAttributes
{
    /// <summary>
    /// Circuit Properties are properties which can be altered to change information
    /// about a certain object, often found in a properties menu on the side of the screen.
    /// This attribute should be placed on a property which has a getter and a setter and is an int.
    /// If you would like an attribute which works with text instead of a dropdown, see <see cref="CircuitPropertyTextAttribute"/>
    /// 
    /// If you would like to add CircuitActions to this property (increments or decrements), see <see cref="CircuitDropdownActionAttribute"/>.
    /// </summary>
    public sealed class CircuitPropertyDropdownAttribute : CircuitPropertyBase
    {
        public readonly (int min, int max) ValueRange;
        public readonly string[] ValueNames;

        /// <param name="RequireReconnect">true if the Circuit Property requires the gate to be reconnected (connections reset) on edit</param>
        public CircuitPropertyDropdownAttribute(int min, int max, bool RequireReconnect) : base(RequireReconnect)
        {
            ValueRange = (min, max);
            int valueCount = max - min + 1;
            ValueNames = new string[valueCount];
            for (int i = 0; i < valueCount; i++)
            {
                ValueNames[i] = (i + min).ToString();
            }
        }

        /// <param name="RequireReconnect">true if the Circuit Property requires the gate to be reconnected (connections reset) on edit. Usually only true if the property affects the gatepins at runtime</param>
        public CircuitPropertyDropdownAttribute(int min, int max, bool RequireReconnect, string[] ValueNames) : base(RequireReconnect)
        {
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

        public override CircuitProp ToProp(PropertyInfo property, BoardObject target)
        {
            return new CircuitProp(
                () => ValueNames[(int)property.GetValue(target) - ValueRange.min],
                (value, connections) => property.SetValue(target, Array.IndexOf(ValueNames, value) + ValueRange.min),
                true, target, ValueRange, ValueNames, RequireReconnect, property.Name);

        }
    }
}