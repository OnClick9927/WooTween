/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace WooTween
{
    class ArrayBuffer<T>
    {
        private const int InitialCapacity = 16;
        private static readonly bool NeedsReferenceClearing = ContainsReferences(typeof(T));

        public T this[int index]
        {
            get { return values[index]; }
            set { values[index] = value; }
        }
        private T[] values;
        private int length;
        public int Length => length;

        public ArrayBuffer()
        {
            values = new T[InitialCapacity];
        }

        private static bool ContainsReferences(Type type)
        {
            if (type.IsPointer)
                return false;
            if (!type.IsValueType)
                return true;
            if (type.IsPrimitive || type.IsEnum)
                return false;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                if (ContainsReferences(fields[i].FieldType))
                    return true;
            }
            return false;
        }

        private void EnsureCapacity(int requiredLength)
        {
            if (requiredLength <= values.Length)
                return;

            var capacity = values.Length;
            while (capacity < requiredLength)
                capacity *= 2;

            var result = new T[capacity];
            Array.Copy(values, 0, result, 0, length);
            values = result;
        }

        public void Clear()
        {
            if (NeedsReferenceClearing && length > 0)
                Array.Clear(values, 0, length);
            length = 0;
        }

        public bool IsSameArray(T[] points)
        {
            if (points == null || points.Length != length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < length; i++)
            {
                if (!comparer.Equals(values[i], points[i]))
                    return false;
            }
            return true;
        }

        public void Read(T[] points, bool reverse = false)
        {
            var previousLength = length;
            EnsureCapacity(points.Length);
            length = points.Length;

            if (reverse)
            {
                for (int i = 0; i < length; i++)
                    values[i] = points[length - 1 - i];
            }
            else
            {
                Array.Copy(points, 0, values, 0, length);
            }

            if (NeedsReferenceClearing && previousLength > length)
                Array.Clear(values, length, previousLength - length);
        }
    }



}
