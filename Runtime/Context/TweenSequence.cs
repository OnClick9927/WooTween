/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace WooTween
{
    class TweenSequence : TweenContextBase, ITweenGroup
    {

        public override float GetPercent()
        {
            var total = list.Count;
            if (total == 0)
                return isDone ? 1f : 0f;
            if (isDone)
                return 1f;
            if (inner == null)
                return 0f;

            float result = _runed.Count - 1 + inner.GetPercent();
            return result / total;
        }


        public List<Func<ITweenContext>> list = new List<Func<ITweenContext>>();
        private List<ITweenContext> _runed = new List<ITweenContext>();
        private List<ITweenContext> rewindContexts = new List<ITweenContext>();
        private bool currentIsRewindLoop;
        protected override void OnRewind()
        {
            RewindChildren(_runed);
            if (!currentIsRewindLoop)
                RewindChildren(rewindContexts);
        }
        public ITweenGroup NewContext(Func<ITweenContext> func)
        {
            if (func == null) return this;
            list.Add(func);
            return this;
        }
        private ITweenContext inner;

        protected override void StopChildren()
        {
            if (inner != null)
            {
                inner.Stop();
                Tween.DetachContext(inner);
            }
            inner = null;
        }

        protected override void Reset()
        {
            ReleaseAllChildren();
            base.Reset();
            loops = 1;
            inner = null;
            list.Clear();
            _nextIndex = 0;
            _loopContextCount = 0;
        }

        private int _loops = 0;
        private int loops = 1;
        private int _nextIndex;
        private int _loopContextCount;

        //private float _runed_time = 0;
        public void SetLoops(int loops)
        {
            this.loops = loops;
        }


        private void RunNext(ITweenContext context)
        {
            if (context != null)
                Tween.DetachContext(context);
            if (canceled || isDone) return;

            while (_nextIndex < _loopContextCount)
            {
                inner = list[_nextIndex++].Invoke();
                if (inner == null)
                    continue;

                inner.SetAutoCycle(false);
                inner.OnTick(_OnTick);
                inner.OnCancel(RunNext);
                inner.OnComplete(RunNext);
                inner.SetTimeScale(this.timeScale);
                _runed.Add(inner);
                if (currentIsRewindLoop)
                    rewindContexts.Add(inner);
                return;
            }

            _loops++;
            if (loops == -1 || _loops < loops)
                OnceLoop();
            else
                Complete();
        }

        private void _OnTick(ITweenContext context, float time, float delta)
        {

            InvokeTick(time, delta);
        }
        private void OnceLoop()
        {
            PrepareLoop();
            _nextIndex = 0;
            _loopContextCount = list.Count;
            RunNext(null);
        }

        private void PrepareLoop()
        {
            if (_loops == 0)
            {
                currentIsRewindLoop = true;
                return;
            }

            if (!currentIsRewindLoop)
                ReleaseChildren(_runed);
            _runed.Clear();
            currentIsRewindLoop = false;
            inner = null;
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
            ReleaseChildren(_runed);
            ReleaseChildren(rewindContexts);
            _runed.Clear();
            rewindContexts.Clear();
            currentIsRewindLoop = false;
            inner = null;
        }

        public override void Run()
        {
            ReleaseAllChildren();
            base.Run();
            _loops = 0;
            OnceLoop();
        }

        public override ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            base.SetTimeScale(timeScale);
            inner?.SetTimeScale(timeScale);
            return this;
        }

        public override void Pause()
        {
            if (!valid || paused) return;
            base.Pause();
            inner?.Pause();
        }

        public override void UnPause()
        {
            if (!valid || !paused) return;
            base.UnPause();
            inner?.UnPause();
        }



    }





}
