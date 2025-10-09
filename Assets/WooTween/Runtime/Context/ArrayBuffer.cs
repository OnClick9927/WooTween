/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace WooTween
{
    class ArrayBuffer<T>
    {
        public T this[int index]
        {
            get { return values[index]; }
            set { values[index] = value; }
        }
        private T[] values;
        private int capcity => values.Length;
        private int length = 16;
        public int Length => length;
        public ArrayBuffer()
        {
            //capcity = length;
            values = new T[length];
        }
        private void ValidArray(int length)
        {
            var capcity = this.capcity;
            while (capcity < length)
                capcity *= 2;

            T[] result = new T[capcity];

            Array.Copy(values, 0, result, 0, this.capcity);
            this.values = result;
        }


        public bool IsSameArray(T[] points)
        {
            for (int i = 0; i < length; i++)
            {
                if (!this.values[i].Equals(points[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public void Read(T[] points, bool reverse = false)
        {
            //var _points_length = points.Length;
            length = points.Length;
            ValidArray(length);
            for (int i = 0; i < length; i++)
            {
                if (reverse)
                    this.values[i] = points[length - 1 - i];
                else
                    this.values[i] = points[i];
            }
        }
    }



}