
using UnityEngine;

namespace WooTween
{
    class ColorTweenValue : TweenValue<Color>
    {
        protected override void MoveNext()
        {
            Color dest = Color.Lerp(start, end, convertPercent);
            SetCurrent(Color.Lerp(pluginValue, dest, deltaPercent));
        }
        protected override Color Snap(Color value)
        {
            value.a = Mathf.RoundToInt(value.a);
            value.r = Mathf.RoundToInt(value.r);
            value.g = Mathf.RoundToInt(value.g);
            value.b = Mathf.RoundToInt(value.b);
            return value;
        }
    }

}
