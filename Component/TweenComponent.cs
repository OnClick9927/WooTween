
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WooTween
{
    public abstract class TweenComponent<T, Target> : MonoBehaviour where T : struct where Target : Object
    {
        public Target[] targets;
        public enum TweenType
        {
            Single,
            Array
        }
        public bool autoPlay = true;
        public float duration = 1;
        public bool snap = false;
        public TweenType type;
        public T start, end;
        public T[] array;
        public AnimationCurve curve;
        public int loop = -1;
        public LoopType LoopType = LoopType.PingPong;
        public bool autoRcyle;
        public Action onComplete;
        public ITween tween;
        private void OnEnable()
        {
            if (autoPlay)
            {
                Play();
            }
        }
        protected virtual void OnTweenComplete()
        {
            if (tween.autoRecycle)
            {
                tween = null;
            }
            onComplete?.Invoke();
        }
        public void Play()
        {
            if (tween != null)
            {
                tween.Complete(false);
                tween = null;
            }
            switch (type)
            {
                case TweenType.Single:
                    tween = TweenEx.DoGoto<T>(start, end, duration, GetTargetValue, SetTargetValue, snap)
                        .SetLoop(loop, LoopType)
                        .SetRecycle(autoRcyle)
                        .OnComplete(OnTweenComplete);
                    if (curve != null)
                    {
                        tween.SetAnimationCurve(curve);
                    }
                    break;
                case TweenType.Array:
                    tween = TweenEx.DoGoto<T>(array, duration, GetTargetValue, SetTargetValue, snap)
                        .SetLoop(loop, LoopType)
                        .SetRecycle(autoRcyle)
                        .OnComplete(OnTweenComplete);
                    if (curve != null)
                    {
                        tween.SetAnimationCurve(curve);
                    }
                    break;
                default:
                    break;
            }

        }
        public void Rewind(int time)
        {
            if (tween != null)
            {
                tween.Rewind(time);
            }
        }

        protected abstract void SetTargetValue(T value);

        protected abstract T GetTargetValue();

        public void Complete(bool v)
        {
            if (tween != null)
            {
                tween.Complete(v);
            }
        }
    }
}
