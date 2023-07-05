

using UnityEngine;

namespace WooTween
{
    class FloatTweenValue : TweenValue<float>
    {
        protected override void MoveNext()
        {
            float dest = Mathf.Lerp(start, end, convertPercent);
            SetCurrent(Mathf.Lerp(pluginValue, dest, deltaPercent));

        }

        protected override float Snap(float value)
        {
            return Mathf.RoundToInt(value);
        }
    }

}
