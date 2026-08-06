/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace WooTween
{
    abstract class ValueCalculator<T>
    {

        public T Calculate(TweenType mode, T start, T end, float percent, T srcValue,
            float srcPercent, bool snap, T strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping,
            ArrayBuffer<T> points)
        {
            T dest = default(T);
            if (mode == TweenType.Bezier || mode == TweenType.Array)
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    if (mode == TweenType.Bezier)
                    {
                        dest = EvaluateBezier(percent, points);
                    }
                    else
                    {
                        T _start, _end; float _percent;
                        EvaluateArray(percent, points, points.Length, out _start, out _end, out _percent);
                        dest = Lerp(_start, _end, _percent);
                    }
                }
            }
            else
                dest = Lerp(start, end, percent);


            dest = Lerp(srcValue, dest, srcPercent);


            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                //strength = MultiStrength(strength, end);
                var s = Multi(strength, EvaluateStrength(frequency, dampingRatio, percent));
                if (mode == TweenType.Punch)
                    dest = Add(dest, s);
                else
                {
                    s = Multi(s, percent);
                    dest = Add(dest, RangeValue(s));
                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                for (int i = 0; i < devCount; i++)
                    strength = Dev(strength, jumpDamping);
                dest = Add(dest, Multi(strength, jumpAdd));
            }
            if (snap)
                dest = Snap(dest);
            return dest;


        }

        public abstract T Snap(T value);
        public abstract T Lerp(T start, T end, float percent);
        public abstract T Minus(T value, T value2);
        public abstract T Add(T value, T value2);
        public abstract T Dev(T value, float dev);
        public abstract T Multi(T value, float dev);
        public abstract T MultiStrength(T strength, T value);
        public abstract T RangeValue(T value);









        protected static float Range() => UnityEngine.Random.Range(-1, 1);

        const float E = 2.71828175F;
        const float PI = 3.14159274F;

        private T EvaluateBezier(float percent, ArrayBuffer<T> points)
        {
            float u = 1f - percent;
            int lastIndex = points.Length - 1;
            float coefficient = Mathf.Pow(u, lastIndex);
            float coefficientRatio = percent / u;
            T result = Multi(points[0], coefficient);

            for (int i = 1; i <= lastIndex; i++)
            {
                coefficient *= coefficientRatio * (points.Length - i) / i;
                result = Add(result, Multi(points[i], coefficient));
            }

            return result;
        }
        private static void EvaluateArray(float percent, ArrayBuffer<T> array, int length, out T start, out T end, out float _percent)
        {
            var temp = percent * (length - 1);
            var floor = Mathf.FloorToInt(temp);
            _percent = temp - floor;
            start = array[floor];

            end = array[floor + 1];
        }
        private static void EvaluateJump(int jumpCount, float percent, out int devCount, out float jumpAdd)
        {
            if (percent == 0 || percent == 1)
            {
                jumpAdd = 0;
                devCount = 0;
                return;
            }

            var gap = 1f / jumpCount;
            var _percent = (percent % gap) * jumpCount;
            devCount = Mathf.FloorToInt(percent / gap);
            jumpAdd = (1 - (4 * Mathf.Pow(_percent - 0.5f, 2)));
        }
        private static float EvaluateStrength(int frequency, float dampingRatio, float t)
        {
            if (t == 1f || t == 0f)
            {
                return 0;
            }
            float angularFrequency = (frequency - 0.5f) * PI;
            float dampingFactor = dampingRatio * frequency / (2f * PI);
            return Mathf.Cos(angularFrequency * t) * Mathf.Pow(E, -dampingFactor * t);
        }
    }


}
