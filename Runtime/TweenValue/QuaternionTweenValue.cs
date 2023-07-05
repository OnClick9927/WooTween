
using UnityEngine;

namespace WooTween
{
    class QuaternionTweenValue : TweenValue<Quaternion>
    {
        protected override void MoveNext()
        {
            Quaternion dest = Quaternion.Lerp(start, end, convertPercent);
            SetCurrent(Quaternion.Lerp(pluginValue, dest, deltaPercent));
        }
        protected override Quaternion Snap(Quaternion value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            value.z = Mathf.RoundToInt(value.z);
            value.z = Mathf.RoundToInt(value.z);
            return value;
        }
    }

}
