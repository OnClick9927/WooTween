﻿/*********************************************************************************
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
        public List<Func<ITweenContext>> list = new List<Func<ITweenContext>>();
        private Queue<Func<ITweenContext>> _queue = new Queue<Func<ITweenContext>>();
        private List<ITweenContext> _runed = new List<ITweenContext>();
        protected override void OnRewind()
        {
            for (int i = 0; i < _runed.Count; i++)
            {
                _runed[i].Rewind();
            }
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
            inner?.Stop();
            inner = null;
        }

        protected override void Reset()
        {
            base.Reset();
            inner = null;
            list.Clear();
            _queue.Clear();
            _runed.Clear();
        }




        private void RunNext(ITweenContext context)
        {
            if (canceled || isDone) return;

            if (_queue.Count > 0)
            {
                inner = _queue.Dequeue().Invoke();
                if (inner != null)
                {
                    inner.OnTick(_OnTick);
                    inner.OnCancel(RunNext);
                    inner.OnComplete(RunNext);
                    inner?.SetTimeScale(this.timeScale);
                    _runed.Add(inner);
                }
                else
                {
                    RunNext(context);
                }
            }
            else
            {
                Complete();
            }
        }

        private void _OnTick(ITweenContext context, float time, float delta)
        {
            InvokeTick(time, delta);
        }

        public override void Run()
        {
            base.Run();
            _queue.Clear();
            _runed.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                var func = list[i];
                _queue.Enqueue(func);
            }
            RunNext(null);
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