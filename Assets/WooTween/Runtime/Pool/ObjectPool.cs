using System;
using System.Collections.Generic;

namespace WooPool
{

    /// <summary>
    /// 基础对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : PoolUnit, IDisposable, IObjectPool
    {
        /// <summary>
        /// 数据容器
        /// </summary>
        protected Queue<T> pool { get { return _lazy.Value; } }
        private Lazy<Queue<T>> _lazy = new Lazy<Queue<T>>(() => { return new Queue<T>(); }, true);
        /// <summary>
        /// 自旋锁
        /// </summary>
        protected object para = new object();
        /// <summary>
        /// 存储数据类型
        /// </summary>
        public virtual Type type { get { return typeof(T); } }

        /// <summary>
        /// 池子数量
        /// </summary>
        public int count { get { return pool.Count; } }


        /// <summary>
        /// 释放时
        /// </summary>
        protected override void OnDispose()
        {
            Clear(null);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual T Get(IPoolArgs arg = null)
        {
            lock (para)
            {
                T t;
                if (pool.Count > 0)
                {
                    t = pool.Dequeue();
                }
                else
                {
                    t = CreateNew(arg);
                    OnCreate(t, arg);
                    (t as IPoolObject)?.OnAllocate();
                }
                OnGet(t, arg);
                (t as IPoolObject)?.OnGet();
                return t;
            }
        }
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public bool Set(object obj, IPoolArgs args)
        {
            if (obj is T)
            {
                return Set((T)obj, args);
            }
            return false;
        }
        protected void RealSet(T t, IPoolArgs arg = null)
        {
            pool.Enqueue(t);
            (t as IPoolObject)?.OnSet();
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual bool Set(T t, IPoolArgs arg = null)
        {
            lock (para)
            {
                if (!pool.Contains(t))
                {
                    if (OnSet(t, arg))
                    {
                        RealSet(t, arg);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="arg"></param>
        public void Clear(IPoolArgs arg = null)
        {
            lock (para)
            {
                while (pool.Count > 0)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg);
                    (t as IDisposable)?.Dispose();
                }
            }
        }
        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="count"></param>
        /// <param name="arg"></param>
        public void Clear(int count, IPoolArgs arg = null)
        {
            lock (para)
            {
                count = count > pool.Count ? 0 : pool.Count - count;
                while (pool.Count > count)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg);
                }
            }
        }
        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected abstract T CreateNew(IPoolArgs arg);
        /// <summary>
        /// 数据被清除时
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        protected virtual void OnClear(T t, IPoolArgs arg) { }
        /// <summary>
        /// 数据被回收时，返回true可以回收
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual bool OnSet(T t, IPoolArgs arg)
        {
            return true;
        }
        /// <summary>
        /// 数据被获取时
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        protected virtual void OnGet(T t, IPoolArgs arg) { }
        /// <summary>
        /// 数据被创建时
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        protected virtual void OnCreate(T t, IPoolArgs arg) { }
    }
}
