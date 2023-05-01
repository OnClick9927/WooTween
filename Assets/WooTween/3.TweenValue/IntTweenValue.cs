

using UnityEngine;

namespace WooTween
{
    class IntTweenValue : TweenValue<int>
    {
        protected override void MoveNext()
        {
            float dest = Mathf.Lerp(start, end, convertPercent);
            SetCurrent((int)Mathf.Lerp(pluginValue, dest, deltaPercent));
        }
    }
}
