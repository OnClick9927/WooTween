using UnityEngine;
namespace WooTween
{
    public class DoRotationComponent : TweenComponent<Vector3,Transform>
    {
        protected override Vector3 GetTargetValue()
        {
            //返回物体的缩放值
            Debug.Log(transform.localEulerAngles);

            return transform.localEulerAngles; 
        }

        protected override void SetTargetValue(Vector3 value)
        {
            //物体的缩放值等于value
            transform.localRotation =Quaternion.Euler(value); 
        }
    }
}
