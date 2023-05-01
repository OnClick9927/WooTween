
using System;

namespace WooTween
{
    public interface IPlugin<T> where T : struct
    {
        T start { get; set; }
        T end { get; set; }
        Action<T> setter { get; set; }
        Func<T> getter { get; set; }
        bool snap { get; set; }
        float duration { get; set; }
        void Config(T start, T end, float duration, Func<T> getter, Action<T> setter, bool snap);
    }
}
