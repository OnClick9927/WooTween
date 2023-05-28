

namespace WooTween
{
    class BoolTweenValue : TweenValue<bool>
    {
        protected override void MoveNext()
        {
            if (percent == 1)
            {
                SetCurrent(end);
            }
        }
    }
}
