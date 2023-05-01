
using UnityEngine;

namespace WooTween
{
    class Vector2TweenValue : TweenValue<Vector2>
    {
        protected override void MoveNext()
        {
            Vector2 dest = Vector2.Lerp(start, end, convertPercent);
            SetCurrent(Vector2.Lerp(pluginValue, dest, deltaPercent));
        }
        protected override Vector2 Snap(Vector2 value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            return value;
        }
    }

}
