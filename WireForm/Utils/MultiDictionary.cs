using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Wireform.Utils
{
    [Serializable]
    public class MultiDictionary<TKey, TValue> : Dictionary<TKey, IReadOnlyList<TValue>>
    {
        public MultiDictionary() { }
        public MultiDictionary(IDictionary<TKey, IReadOnlyList<TValue>> dictionary) : base(dictionary) { }
        public MultiDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public MultiDictionary(int capacity) : base(capacity) { }
        public MultiDictionary(IDictionary<TKey, IReadOnlyList<TValue>> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        public MultiDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        protected MultiDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                Add(key, new List<TValue>());
            }
            var newList = (List<TValue>)this[key];
            newList.Add(value);
            this[key] = newList;
        }

        public bool ContainsValue(TValue value)
        {
            foreach (var key in Keys)
            {
                if (this[key].Contains(value)) return true;
            }

            return false;
        }

        public void Remove(TValue value)
        {
            foreach (var key in Keys)
            {
                if (this[key].Contains(value))
                {
                    var newList = (List<TValue>)this[key];
                    newList.Remove(value);
                    if (newList.Count > 0)
                        this[key] = newList;
                    else
                        Remove(key);
                }
            }
        }

        public TKey GetKey(TValue value)
        {
            foreach (var key in Keys)
            {
                if (this[key].Contains(value)) return key;
            }

            return default;
        }
    }
}
