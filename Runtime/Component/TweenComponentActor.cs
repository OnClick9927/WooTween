/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;


namespace WooTween
{
    [System.Serializable]
    public abstract class TweenComponentActor
    {
        internal Transform transform { get; set; }

        public string id;
        public float duration = 1;

        internal ITweenContext Create()
        {
            ResetPercent();
            var result = _Create();
#if UNITY_EDITOR
            result.OnTick(OnTick);
            result.OnComplete(OnEnd);
            //result.OnRewind(OnRewind);

#endif
            return result;
        }


        [NonSerialized] private float _percent;
        internal float percent => _percent;
        internal void ResetPercent() => _percent = 0;
        //private void OnRewind(ITweenContext context)
        //{
        //    ResetPercent();
        //}

        private void OnEnd(ITweenContext context) => _percent = 1;

        private void OnTick(ITweenContext context, float time, float delta) => _percent = context.GetPercent();

        protected abstract ITweenContext _Create();

    }

}