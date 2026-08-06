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
    class TweenScheduler
    {
        private readonly Dictionary<Type, ISimpleObjectPool> contextPools;

        public TweenScheduler()
        {
            contextPools = new Dictionary<Type, ISimpleObjectPool>(8);
        }

        public void Update()
        {
            isUpdating = true;
            try
            {
                float deltaTime = Tween.GetDeltaTime();
                var runCount = contexts_run.Count;
                for (int i = 0; i < runCount; i++)
                {
                    var context = contexts_run[i];
                    if (context == null)
                        continue;
                    (context as TweenContext).Update(deltaTime);
                }

                for (int i = 0; i < contexts_wait_to_run.Count; i++)
                {
                    var context = contexts_wait_to_run[i];
                    if (context == null)
                        continue;
                    context.Run();
                }
                contexts_wait_to_run.Clear();
            }
            finally
            {
                isUpdating = false;
                FlushDeferredRecycle();
                CompactRunList();
            }
        }

        private readonly List<ITweenContext> contexts_run = new List<ITweenContext>(16);
        private readonly List<ITweenContext> contexts_wait_to_run = new List<ITweenContext>(16);
        private readonly List<ITweenGroup> contexts_group = new List<ITweenGroup>(4);
        private readonly List<ITweenContext> contexts_to_recycle = new List<ITweenContext>(8);
        private bool runListHasHoles;
        private bool isUpdating;

        public ITweenContext<T, Target> AllocateContext<T, Target>(bool auto_run)
        {
            Type type = typeof(TweenContext<T, Target>);
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<TweenContext<T, Target>>();
                contextPools.Add(type, pool);
            }
            var simple = pool as SimpleObjectPool<TweenContext<T, Target>>;
            var context = simple.Get();
            if (auto_run)
            {
                context.waitIndex = contexts_wait_to_run.Count;
                contexts_wait_to_run.Add(context);
            }
            return context;
        }
        public ITweenGroup AllocateSequence()
        {
            Type type = typeof(TweenSequence);
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<TweenSequence>();
                contextPools.Add(type, pool);
            }
            var simple = pool as SimpleObjectPool<TweenSequence>;
            var context = simple.Get();
            //contexts_run.Add(context);
            return context;
        }
        public ITweenGroup AllocateParallel()
        {
            Type type = typeof(TweenParallel);
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<TweenParallel>();
                contextPools.Add(type, pool);
            }
            var simple = pool as SimpleObjectPool<TweenParallel>;
            var context = simple.Get();
            //contexts_run.Add(context);
            return context;
        }

        public bool CycleContext(ITweenContext context)
        {
            var type = context.GetType();
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool)) return false;

            var contextBase = context.AsContextBase();
            if (contextBase.recyclePending)
                return false;

            if (isUpdating)
            {
                DetachContext(context);
                contextBase.recyclePending = true;
                contexts_to_recycle.Add(context);
                return false;
            }

            DetachContext(context);
            pool.SetObject(context);
            return true;
        }

        internal void DetachContext(ITweenContext context)
        {
            if (context == null)
                return;

            var contextBase = context.AsContextBase();
            RemoveWaiting(context, contextBase);

            if (contextBase.inRunList)
            {
                contextBase.inRunList = false;
                RemoveRunning(context, contextBase);
            }

            if (contextBase.inGroupList)
            {
                contextBase.inGroupList = false;
                contexts_group.Remove(context as ITweenGroup);
            }
        }

        private void FlushDeferredRecycle()
        {
            for (int i = 0; i < contexts_to_recycle.Count; i++)
            {
                var context = contexts_to_recycle[i];
                var contextBase = context.AsContextBase();
                contextBase.recyclePending = false;
                if (CycleContext(context))
                    Tween.NotifyContextRecycle(context);
            }
            contexts_to_recycle.Clear();
        }

        private void RemoveRunning(ITweenContext context, TweenContextBase contextBase)
        {
            var index = contextBase.runIndex;
            if (index < 0 || index >= contexts_run.Count ||
                !ReferenceEquals(contexts_run[index], context))
            {
                index = contexts_run.IndexOf(context);
            }

            if (index >= 0)
            {
                contexts_run[index] = null;
                runListHasHoles = true;
            }
            contextBase.runIndex = -1;
        }

        private void CompactRunList()
        {
            if (!runListHasHoles)
                return;

            int writeIndex = 0;
            for (int readIndex = 0; readIndex < contexts_run.Count; readIndex++)
            {
                var context = contexts_run[readIndex];
                if (context == null)
                    continue;

                if (writeIndex != readIndex)
                    contexts_run[writeIndex] = context;
                context.AsContextBase().runIndex = writeIndex;
                writeIndex++;
            }

            if (writeIndex < contexts_run.Count)
                contexts_run.RemoveRange(writeIndex, contexts_run.Count - writeIndex);
            runListHasHoles = false;
        }

        private void RemoveWaiting(ITweenContext context, TweenContextBase contextBase)
        {
            var index = contextBase.waitIndex;
            if (index >= 0 && index < contexts_wait_to_run.Count &&
                ReferenceEquals(contexts_wait_to_run[index], context))
            {
                contexts_wait_to_run[index] = null;
            }
            contextBase.waitIndex = -1;
        }

        public void KillTweens()
        {
            for (int i = contexts_group.Count - 1; i >= 0; i--)
            {
                var context = contexts_group[i];
                context.Stop();
                context.Recycle();
            }
            for (int i = contexts_wait_to_run.Count - 1; i >= 0; i--)
            {
                var context = contexts_wait_to_run[i];
                if (context == null) continue;
                context.Stop();
                context.Recycle();
            }
            for (int i = contexts_run.Count - 1; i >= 0; i--)
            {
                var context = contexts_run[i];
                if (context == null) continue;
                context.Stop();
                context.Recycle();
            }
            if (!isUpdating)
                CompactRunList();
        }
        public void KillTweens(object obj)
        {
            for (int i = contexts_group.Count - 1; i >= 0; i--)
            {
                var context = contexts_group[i];
                if (context.AsContextBase().owner != obj) continue;
                context.Stop();
                context.Recycle();
            }
            for (int i = contexts_wait_to_run.Count - 1; i >= 0; i--)
            {
                var context = contexts_wait_to_run[i];
                if (context == null) continue;
                if (context.AsContextBase().owner != obj) continue;

                context.Stop();
                context.Recycle();
            }
            for (int i = contexts_run.Count - 1; i >= 0; i--)
            {
                var context = contexts_run[i];
                if (context == null) continue;
                if (context.AsContextBase().owner != obj) continue;
                context.Stop();
                context.Recycle();
            }
            if (!isUpdating)
                CompactRunList();
        }
        public bool IsRunning(ITweenContext context)
        {
            if (context == null) return false;
            var contextBase = context.AsContextBase();
            return contextBase != null &&
                (contextBase.waitIndex >= 0 || contextBase.inRunList || contextBase.inGroupList);
        }
        internal void AddToRun(ITweenContext context)
        {
            if (context == null) return;

            var contextBase = context.AsContextBase();
            if (context is ITweenGroup group)
            {
                if (contextBase.inGroupList) return;
                contextBase.inGroupList = true;
                contexts_group.Add(group);
            }
            else
            {
                if (contextBase.inRunList) return;
                RemoveWaiting(context, contextBase);
                contextBase.inRunList = true;
                contextBase.runIndex = contexts_run.Count;
                contexts_run.Add(context);
            }

        }




    }





}
