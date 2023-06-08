
using UnityEngine;
namespace WooTween
{
    public class DoPositionComponent : TweenComponent<Vector3,Transform>
    {
        protected override Vector3 GetTargetValue()
        {
                //返回物体的缩放值
            return transform.localPosition; 
        }

        protected override void SetTargetValue(Vector3 value)
        {
                //物体的缩放值等于value
            transform.localPosition = value;
        }
    }
}
