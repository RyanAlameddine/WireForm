using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WireForm.Circuitry.Data;
using WireForm.Circuitry.Gates.Utilities;
using WireForm.MathUtils;

namespace WireForm.Circuitry.CircuitAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CircuitPropertyAttribute : Attribute
    {
        public (int min, int max) ValueRange;
        public string[] ValueNames;

        public CircuitPropertyAttribute(int min, int max)
        {
            ValueRange = (min, max);
            int valueCount = max - min + 1;
            ValueNames = new string[valueCount];
            for (int i = 0; i < valueCount; i++)
            {
                ValueNames[i] = (i + min).ToString();
            }
        }

        public CircuitPropertyAttribute(int min, int max, string[] ValueNames)
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

        public static List<CircuitProp> GetProperties(CircuitObject target, StateStack stateStack, Form form)
        {
            var properties = target.GetType().GetProperties();
            var circuitProps = new List<CircuitProp>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute(typeof(CircuitPropertyAttribute), true) as CircuitPropertyAttribute;
                ///If attribute is not found or if property has an [IgnoreCircuitAttributesAttribute]
                if (attribute == null || property.GetCustomAttribute(typeof(IgnoreCircuitAttributesAttribute), true) != null) continue;
                circuitProps.Add(new CircuitProp(property, target, attribute.ValueRange, attribute.ValueNames, property.Name));
            }
            return circuitProps;
        }
    }

    public readonly struct CircuitProp
    {
        private readonly PropertyInfo info;
        private readonly CircuitObject circuitObject;
        public readonly (int min, int max) valueRange;
        public readonly string[] valueNames;

        public readonly string Name;

        public CircuitProp(PropertyInfo info, CircuitObject circuitObject, (int min, int max) valueRange, string[] valueNames, string Name)
        {
            this.info = info;
            this.circuitObject = circuitObject;
            this.valueRange = valueRange;
            this.valueNames = valueNames;
            this.Name = Name;
        }

        public int Get()
        {
            return (int)info.GetValue(circuitObject);
        }

        public void Set(int value)
        {
            if (value < valueRange.min || value > valueRange.max)
            {
                throw new Exception("Selected value is not in range");
            }
            info.SetValue(circuitObject, value);
        }
    }
}