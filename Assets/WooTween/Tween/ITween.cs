

namespace WooTween
{
    public interface ITween
    {
        bool recyled { get; }
        IPercentConverter converter { get; set; }
        int loop { get; set; }
        bool autoRecycle { get; set; }
        LoopType loopType { get; set; }
        bool snap { get; set; }
        void Complete(bool invoke);
        void ReStart();
        void Rewind(float duration, bool snap = false);
        ITween SetConverter(IPercentConverter converter);
    }
    public interface ITween<T> : ITween where T : struct
    {
        T end { get; set; }
        T start { get; set; }
    }
}
