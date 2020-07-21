using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;

namespace Wireform.Circuitry.CircuitAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class CircuitPropertyBase : Attribute
    {
        public readonly bool RequireReconnect;

        internal CircuitPropertyBase(bool RequireReconnect)
        {
            this.RequireReconnect = RequireReconnect;
        }

        public abstract CircuitProp ToProp(PropertyInfo property, BoardObject target);
    }


    /// <summary>
    /// A processed representation of a property with a [CircuitProperty]
    /// Includes the Get and Set methods
    /// </summary>
    public readonly struct CircuitProp
    {
        private readonly Func<string> getter;
        private readonly Action<string, Dictionary<Vec2, List<DrawableObject>>> setter;
        public readonly bool RepresentsInt;

        public readonly BoardObject boardObject;
        public readonly (int min, int max) valueRange;
        public readonly string[] valueNames;
        public readonly bool RequireReconnect;
        public readonly string Name;

        internal CircuitProp(Func<string> getter, Action<string, Dictionary<Vec2, List<DrawableObject>>> setter, bool RepresentsInt, BoardObject boardObject, (int min, int max) valueRange, string[] valueNames, bool RequireReconnect, string Name)
        {
            this.getter = getter;
            this.setter = setter;
            this.RepresentsInt = RepresentsInt;
            this.boardObject = boardObject;
            this.valueRange = valueRange;
            this.valueNames = valueNames;
            this.Name = Name;
            this.RequireReconnect = RequireReconnect;
        }

        internal string Get()
        {
            return getter();
        }

        internal void Set(string value, Dictionary<Vec2, List<DrawableObject>> connections)
        {
            if (boardObject is CircuitObject circuitObject && RequireReconnect)
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

        public override bool Equals(object obj)
        {
            return obj is CircuitProp prop &&
                   EqualityComparer<Func<string>>.Default.Equals(getter, prop.getter) &&
                   EqualityComparer<Action<string, Dictionary<Vec2, List<DrawableObject>>>>.Default.Equals(setter, prop.setter) &&
                   valueRange.Equals(prop.valueRange) &&
                   EqualityComparer<string[]>.Default.Equals(valueNames, prop.valueNames) &&
                   Name == prop.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -224463829;
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<string>>.Default.GetHashCode(getter);
            hashCode = hashCode * -1521134295 + EqualityComparer<Action<string, Dictionary<Vec2, List<DrawableObject>>>>.Default.GetHashCode(setter);
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
