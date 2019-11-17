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

namespace WireForm.Circuitry
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CircuitPropertyAttribute : Attribute
    {
        public Range ValueRange;

        public CircuitPropertyAttribute(Range ValueRange)
        {
            this.ValueRange = ValueRange;
        }

        public static List<CircuitProp> GetProperties(CircuitObject target, StateStack stateStack, Form form)
        {
            var properties = target.GetType().GetProperties();
            var circuitProps = new List<CircuitProp>();

            foreach (var property in properties)
            {
                foreach (var attribute in (CircuitPropertyAttribute[])property.GetCustomAttributes(typeof(CircuitPropertyAttribute), true))
                {
                    circuitProps.Add(new CircuitProp(property, target, property.Name));
                }
            }
            return circuitProps;
        }
    }

    public readonly struct CircuitProp
    {
        private readonly PropertyInfo info;
        private readonly CircuitObject circuitObject;

        public readonly string Name;

        public CircuitProp(PropertyInfo info, CircuitObject circuitObject, string Name)
        {
            this.info = info;
            this.circuitObject = circuitObject;
            this.Name = Name;
        }

        public object Get()
        {
            return info.GetValue(circuitObject);
        }

        public void Set(object value)
        {
            //Type targetType = info.PropertyType;

            //value = Convert.ChangeType(value, targetType);

            info.SetValue(circuitObject, value);
        }
    }
}