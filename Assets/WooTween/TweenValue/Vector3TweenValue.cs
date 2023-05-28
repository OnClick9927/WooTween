
using UnityEngine;

namespace WooTween
{
    class Vector3TweenValue : TweenValue<Vector3>
    {
        protected override void MoveNext()
        {
            Vector3 dest = Vector3.Lerp(start, end, convertPercent);
            SetCurrent(Vector3.Lerp(pluginValue, dest, deltaPercent));
        }
        protected override Vector3 Snap(Vector3 value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            value.z = Mathf.RoundToInt(value.z);
            return value;
        }
    }

}
