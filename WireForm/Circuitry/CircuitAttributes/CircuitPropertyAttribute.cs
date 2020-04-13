using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.MathUtils;

namespace WireForm.Circuitry.CircuitAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CircuitPropertyAttribute : Attribute
    {
        public readonly (int min, int max) ValueRange;
        public readonly string[] ValueNames;
        public readonly bool RequireRefresh;

        /// <param name="RequireRefresh">true if the Circuit Property requires the gate to be refreshed (connections reset) on edit</param>
        public CircuitPropertyAttribute(int min, int max, bool RequireRefresh)
        {
            this.RequireRefresh = RequireRefresh;
            ValueRange = (min, max);
            int valueCount = max - min + 1;
            ValueNames = new string[valueCount];
            for (int i = 0; i < valueCount; i++)
            {
                ValueNames[i] = (i + min).ToString();
            }
        }

        /// <param name="RequireRefresh">true if the Circuit Property requires the gate to be refreshed (connections reset) on edit. Usually only true if the property affects the gatepins at runtime</param>
        public CircuitPropertyAttribute(int min, int max, bool RequireRefresh, string[] ValueNames)
        {
            this.RequireRefresh = RequireRefresh;
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

        public static List<CircuitProp> GetProperties(CircuitObject target)
        {
            var properties = target.GetType().GetProperties();
            var circuitProps = new List<CircuitProp>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<CircuitPropertyAttribute>(true);
                ///If attribute is not found or if property has an [IgnoreCircuitAttributesAttribute]
                if (attribute == null || property.GetCustomAttribute(typeof(HideCircuitAttributesAttribute), true) != null) continue;
                circuitProps.Add(new CircuitProp(property, target, attribute.ValueRange, attribute.ValueNames, attribute.RequireRefresh, property.Name));
            }
            return circuitProps;
        }
    }

    /// <summary>
    /// A processed representation of a property with a [CircuitProperty]
    /// Includes the Get and Set methods
    /// </summary>
    public readonly struct CircuitProp
    {
        private readonly PropertyInfo info;
        private readonly CircuitObject circuitObject;
        public readonly (int min, int max) valueRange;
        public readonly string[] valueNames;

        public readonly string Name;
        public readonly bool RequireRefresh;

        public CircuitProp(PropertyInfo info, CircuitObject circuitObject, (int min, int max) valueRange, string[] valueNames, bool RequireRefresh, string Name)
        {
            this.info = info;
            this.circuitObject = circuitObject;
            this.valueRange = valueRange;
            this.valueNames = valueNames;
            this.RequireRefresh = RequireRefresh;
            this.Name = Name;
        }

        public int Get()
        {
            return (int)info.GetValue(circuitObject);
        }

        public void Set(int value, Dictionary<Vec2, List<BoardObject>> connections)
        {
            if (value < valueRange.min || value > valueRange.max)
            {
                throw new Exception("Selected value is not in range");
            }
            if (RequireRefresh)
            {
                circuitObject.RemoveConnections(connections);
                info.SetValue(circuitObject, value);
                circuitObject.AddConnections(connections);
            }
            else
            {
                info.SetValue(circuitObject, value);
            }
        }
    }
}