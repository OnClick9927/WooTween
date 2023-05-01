
using UnityEngine;
namespace WooTween
{
    public class DoScaleComponent : TweenComponent<Vector3,Transform>
    {
        protected override Vector3 GetTargetValue()
        {
            return transform.localScale; 
        }

        protected override void SetTargetValue(Vector3 value)
        {
            transform.localScale = value;
        }
    }
}
