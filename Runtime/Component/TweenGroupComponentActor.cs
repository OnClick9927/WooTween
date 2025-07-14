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
    public abstract class TweenGroupComponentActor: TweenComponentActor
    {
        public int loops = 1;
        public bool snap = false;

    }
    public abstract class TweenGroupComponentActor<TTarget> : TweenGroupComponentActor
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
            context.SetId(id).SetLoops(loops);

            return context;
        }
        protected abstract ITweenGroup OnCreate();

        public TTarget target;


    }

}