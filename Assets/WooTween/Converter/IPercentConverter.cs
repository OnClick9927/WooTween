

namespace WooTween
{
    public interface IPercentConverter
    {
        float Convert(float percent, float time, float duration);
        void Recycle();
    }
    public interface IPercentConverter<T> : IPercentConverter
    {
        IPercentConverter Config(T value);
    }
}
