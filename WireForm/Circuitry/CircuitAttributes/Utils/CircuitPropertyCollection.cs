using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wireform.Circuitry.Data;
using Wireform.MathUtils;

namespace Wireform.Circuitry.CircuitAttributes.Utils
{
    /// <summary>
    /// A collection which is able to process lists of [CircuitProperties]
    /// </summary>
    public class CircuitPropertyCollection : IReadOnlyDictionary<string, CircuitProp>
    {
        private readonly SortedDictionary<string, CircuitProp> propertyMap;
        private Action<string> registerChange;

        public static CircuitPropertyCollection Empty { get => new CircuitPropertyCollection(null, null); }

        internal CircuitPropertyCollection(HashSet<CircuitProp> properties, Action<string> registerChange)
        {
            propertyMap = new SortedDictionary<string, CircuitProp>();
            this.registerChange = registerChange;
            if (properties == null) return;

            foreach(var prop in properties)
            {
                propertyMap.Add(prop.Name, prop);
            }
        }

        /// <summary>
        /// Updates the values of this collection to match the intersection of circuitProperties in the propertyMap. 
        /// The idea is to merge all CircuitProps with the same name if possible, trimming down the valueRanges to match
        /// acceptable values for all merged properties.
        /// </summary>
        public void Intersect(CircuitPropertyCollection other)
        {
            //Deal with empty or null collections
            if (other == null) return;
            registerChange ??= other.registerChange;
            if (other.propertyMap.Count == 0) return;
            if (this.propertyMap.Count == 0)
            {
                //Copy other into this
                foreach(var pair in other.propertyMap)
                    propertyMap.Add(pair.Key, pair.Value);
                
                return;
            }
            //Remove any non-duplicates in propertyMap
            var and = ((ICollection<KeyValuePair<string, CircuitProp>>)propertyMap)
                .Where((x) => !other.ContainsKey(x.Key))
                .Select((x) => x.Key);

            LinkedList<string> keysToRemove = new LinkedList<string>(and);

            foreach (var pair in other)
            {
                var property = pair.Value;

                //Skip if not in both sets
                if (!propertyMap.ContainsKey(property.Name))
                {
                    continue;
                }
                //If it is there, chain this property with the new one

                //upadate current value of map to match new property
                var existingProp = propertyMap[property.Name];

                //determine the valueRange
                int min = Math.Max(existingProp.valueRange.min, property.valueRange.min);
                int max = Math.Min(existingProp.valueRange.max, property.valueRange.max);
                if (min > max)
                {
                    keysToRemove.AddLast(property.Name);
                    continue;
                }

                //determine the valueNames
                string[] valueNames = new string[max + 1 - min];
                for (int value = min; value <= max; value++)
                {
                    //The current index in valueNames
                    int i      = value - min;
                    //The current index in valueNames of property
                    int propI  = value - property    .valueRange.min;
                    //The current index in valueNames of existingProp
                    int existI = value - existingProp.valueRange.min;

                    if (property.valueNames[propI] == existingProp.valueNames[existI]) valueNames[i] = existingProp.valueNames[existI];
                    else valueNames[i] = value.ToString();
                }

                //determine the value
                string propValue = property.Get();
                if (propValue != existingProp.Get()) propValue = null;

                //create and insert the new property
                var newProp = new CircuitProp(
                    ()  => propValue, //getter with shared value
                    (value, connections) => //setter which chains Set calls
                    {
                        property.Set(value, connections);
                        existingProp.Set(value, connections);
                    }, 
                    true, property.boardObject, (min, max), valueNames, property.RequireReconnect, property.Name);

                propertyMap[property.Name] = newProp;
            }

            foreach (var key in keysToRemove) propertyMap.Remove(key);
        }

        /// <summary>
        /// Invokes the Get function on the property and returns the result
        /// </summary>
        /// <returns>null only if the values are different for different selections</returns>
        public string InvokeGet(string propertyName)
        {
            return propertyMap[propertyName].Get();
        }

        /// <summary>
        /// Invokes the Set function on the property with the given input
        /// </summary>
        public void InvokeSet(string propertyName, string value, Dictionary<Vec2, List<DrawableObject>> connections)
        {
            var property = propertyMap[propertyName];
            //string valueName = property.GetValueName(int.Parse(value));
            string oldValue = property.Get();
            property.Set(value, connections);
            registerChange($"Changed {property.Name} from {oldValue} to {value} on selection(s)");
        }

        public CircuitProp this[string key] => propertyMap[key];

        public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, CircuitProp>)propertyMap).Keys;

        public IEnumerable<CircuitProp> Values => ((IReadOnlyDictionary<string, CircuitProp>)propertyMap).Values;

        public int Count => propertyMap.Count;

        public bool ContainsKey(string key)
        {
            return propertyMap.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, CircuitProp>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<string, CircuitProp>)propertyMap).GetEnumerator();
        }

        public bool TryGetValue(string key, out CircuitProp value)
        {
            return propertyMap.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyDictionary<string, CircuitProp>)propertyMap).GetEnumerator();
        }
    }
}
