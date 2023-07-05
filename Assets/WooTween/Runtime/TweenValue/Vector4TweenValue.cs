
using UnityEngine;

namespace WooTween
{
    class Vector4TweenValue : TweenValue<Vector4>
    {
        protected override void MoveNext()
        {
            Vector4 dest = Vector4.Lerp(start, end, convertPercent);
            SetCurrent(Vector4.Lerp(pluginValue, dest, deltaPercent));
        }
        protected override Vector4 Snap(Vector4 value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            value.z = Mathf.RoundToInt(value.z);
            value.w = Mathf.RoundToInt(value.w);
            return value;
        }
    }

}
