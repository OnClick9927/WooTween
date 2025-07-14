/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;


namespace WooTween
{
    public abstract class TweenComponentContextActor: TweenComponentActor
    {
        public enum CurveType
        {
            Ease,
            AnimationCurve,
        }
        public AnimationCurve curve = new AnimationCurve();
        public Ease ease;
        public CurveType curveType = CurveType.Ease;
        public LoopType loopType = LoopType.Restart;
        public int loops = 1;
        public float delay = 0;
        public float sourceDelta = 0;
        public bool snap = false;

    }
    public abstract class TweenComponentActor<T, TTarget> : TweenComponentContextActor
    {

        protected override ITweenContext _Create()
        {
            if (target == null)
                target = transform.GetComponent<TTarget>();

            if (target == null)
            {
                Debug.LogError($"Can not GetComponent<{typeof(TTarget)}>() from {transform.name}");
            }

            var context = OnCreate();
            context.SetLoop(loopType, loops)
                .SetDelay(delay)
                .SetSnap(snap)
                .SetDuration(duration)
                .SetSourceDelta(sourceDelta)
                .SetId(id);
            if (curveType == CurveType.Ease)
                context.SetEase(ease);
            else
                context.SetAnimationCurve(curve);
            return context;
        }



        protected abstract ITweenContext<T, TTarget> OnCreate();

        public TTarget target;


    }

}