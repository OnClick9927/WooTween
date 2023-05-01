
using System;

namespace WooTween
{
    public interface IArrayTween<T> : ITween<T> where T : struct
    {
        void Config(T[] array, float duration, Func<T> getter, Action<T> setter,bool snap);
    }
}
