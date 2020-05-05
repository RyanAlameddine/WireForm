using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Wireform.Utils
{
    public class WrappingArray<T> : ICollection<T>
    {
        private readonly T[] elements;
        int i = 0;

        public WrappingArray(int length)
        {
            elements = new T[length];
        }

        public int Count => elements.Length;

        bool ICollection<T>.IsReadOnly => false;

        public bool Remove(T item)
        {
            elements[i % elements.Length] = default;
            i--;
            return true;
        }

        public void Add(T item)
        {
            elements[i % elements.Length] = item;
            i++;
        }

        void ICollection<T>.Clear()
        {
            ((ICollection<T>)elements).Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return ((ICollection<T>)elements).Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            elements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((ICollection<T>)elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<T>)elements).GetEnumerator();
        }
    }
}
