
using System;

namespace WooTween
{
    public interface ISingleTween<T>:ITween<T> where T : struct
    {
        void Config(T start, T end, float duration, Func<T> getter, Action<T> setter,bool snap);
    }
}
