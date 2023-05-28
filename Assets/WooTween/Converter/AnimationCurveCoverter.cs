using UnityEngine;

namespace WooTween
{
    public class AnimationCurveCoverter : TweenObject, IPercentConverter<AnimationCurve>
    {
        private AnimationCurve _curve= null;

        public float Convert(float percent, float time, float duration)
        {
             return _curve.Evaluate(percent); 
        }
        public IPercentConverter Config(AnimationCurve value)
        {
            this._curve = value;
            return this;
        }

        protected override void Reset()
        {
            _curve = null;
        }
    }
}
