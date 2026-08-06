/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WooTween
{
    class TweenParallel : TweenContextBase, ITweenGroup
    {
        public List<Func<ITweenContext>> list = new List<Func<ITweenContext>>();
        private List<ITweenContext> contexts = new List<ITweenContext>();
        private List<ITweenContext> rewindContexts = new List<ITweenContext>();
        private bool currentIsRewindLoop;
        public override float GetPercent()
        {
            if (contexts.Count == 0)
                return isDone ? 1f : 0f;

            float result = 1f;

            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                var contextBase = context.AsContextBase();
                if (context.isDone || contextBase.canceled)
                    continue;
                result = Mathf.Min(result, context.GetPercent());
            }

            return result;
        }
        protected override void OnRewind()
        {
            RewindChildren(contexts);
            if (!currentIsRewindLoop)
                RewindChildren(rewindContexts);
        }
        public ITweenGroup NewContext(Func<ITweenContext> func)
        {
            if (func == null) return this;
            list.Add(func);
            return this;
        }
        protected override void StopChildren()
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Stop();
                Tween.DetachContext(context);
            }
        }

        protected override void Reset()
        {
            ReleaseAllChildren();
            base.Reset();
            loops = 1;
            this._time = this._delta = -1;
            list.Clear();
            completedContexts = 0;
        }
        private int _loops = 0;
        private int loops = 1;
        private int completedContexts;
        public void SetLoops(int loops)
        {
            this.loops = loops;
        }
        private void OnContextEnd(ITweenContext context)
        {
            Tween.DetachContext(context);
            if (canceled || isDone) return;
            completedContexts++;
            if (completedContexts >= contexts.Count)
            {
                _loops++;
                if (loops == -1 || _loops < loops)
                    OnceLoop();
                else
                    Complete();
            }
        }
        private float _time, _delta;
        private void _OnTick(ITweenContext context, float time, float delta)
        {
            if (_time != time || _delta != delta)
            {
                this._time = time;
                this._delta = delta;
                InvokeTick(time, delta);
            }
        }

        private void OnceLoop()
        {
            PrepareLoop();
            completedContexts = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var func = list[i];
                var context = func.Invoke();
                if (context == null)
                    continue;
                context.SetAutoCycle(false);
                context.OnCancel(OnContextEnd);
                context.OnComplete(OnContextEnd);
                context.OnTick(_OnTick);
                context.SetTimeScale(timeScale);
                contexts.Add(context);
                if (currentIsRewindLoop)
                    rewindContexts.Add(context);
            }
            if (contexts.Count == 0)
                Complete();
        }

        private void PrepareLoop()
        {
            if (_loops == 0)
            {
                currentIsRewindLoop = true;
                return;
            }

            if (!currentIsRewindLoop)
                ReleaseChildren(contexts);
            contexts.Clear();
            currentIsRewindLoop = false;
        }

        private static void RewindChildren(List<ITweenContext> children)
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                var context = children[i];
                var contextBase = context.AsContextBase();
                if (contextBase.valid && !contextBase.recyclePending)
                    context.Rewind();
            }
        }

        private static void ReleaseChildren(List<ITweenContext> children)
        {
            for (int i = 0; i < children.Count; i++)
            {
                var context = children[i];
                var contextBase = context.AsContextBase();
                if (!contextBase.valid || contextBase.recyclePending)
                    continue;

                Tween.DetachContext(context);
                context.Recycle();
            }
        }

        private void ReleaseAllChildren()
        {
            ReleaseChildren(contexts);
            ReleaseChildren(rewindContexts);
            contexts.Clear();
            rewindContexts.Clear();
            currentIsRewindLoop = false;
        }
        public override void Run()
        {
            ReleaseAllChildren();
            base.Run();
            _loops = 0;
            this._time = this._delta = -1;
            if (list.Count <= 0)
                Complete();
            else
                OnceLoop();
            //contexts.Clear();
            //if (list.Count > 0)
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        var func = list[i];
            //        var context = func.Invoke();
            //        context.OnCancel(OnContextEnd);
            //        context.OnComplete(OnContextEnd);
            //        context.OnTick(_OnTick);
            //        context.SetTimeScale(timeScale);
            //        contexts.Add(context);
            //    }
            //else
            //    Complete();
        }

        public override ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            base.SetTimeScale(timeScale);
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.SetTimeScale(timeScale);
            }
            return this;
        }

        public override void Pause()
        {
            if (!valid || paused) return;
            base.Pause();
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Pause();
            }
        }

        public override void UnPause()
        {
            if (!valid || !paused) return;
            base.UnPause();
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.UnPause();
            }
        }
    }





}
