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
    interface IPoolObject
    {
        bool valid { get; set; }
        void OnGet();
        void OnSet();
    }
    interface ISimpleObjectPool
    {
        void SetObject(object context);
    }
    class SimpleObjectPool<T> : ISimpleObjectPool where T : class, new()
    {
        private readonly Stack<T> pool = new Stack<T>();

        public void SetObject(object context)
        {
            var value = context as T;
            if (value == null)
                return;

            Set(value);
        }

        private T CreateNew()
        {
            return new T();
        }


        public  Type type { get { return typeof(T); } }


        public int count { get { return pool.Count; } }




        public  T Get()
        {
            T t;
            if (pool.Count > 0)
            {
                t = pool.Pop();
            }
            else
            {
                t = CreateNew();
                OnCreate(t);
            }
            if (t is IPoolObject)
            {
                IPoolObject obj = t as IPoolObject;
                obj.valid = true;
                obj.OnGet();
            }
            OnGet(t);

            return t;
        }

        public bool Set(T t)
        {
            if (t == null)
                return false;

            var poolObject = t as IPoolObject;
            if (poolObject != null && !poolObject.valid)
                return false;

            if (OnSet(t))
            {
                if (poolObject != null)
                {
                    poolObject.valid = false;
                    poolObject.OnSet();
                }
                pool.Push(t);
                return true;
            }

            return false;
        }


        public void Clear()
        {
            while (pool.Count > 0)
            {
                var t = pool.Pop();
                OnClear(t);
                IDisposable dispose = t as IDisposable;
                if (dispose != null)
                    dispose.Dispose();
            }
        }


        private void OnClear(T t) { }

        private bool OnSet(T t)
        {
            return true;
        }

        private void OnGet(T t) { }

        private void OnCreate(T t) { }

    }


    class StaticPool<T> where T : class, new()
    {
        internal static readonly SimpleObjectPool<T> s_Pool = new SimpleObjectPool<T>();

        public static T Get()
        {
            return s_Pool.Get();
        }
        public static void Set(T toRelease)
        {
            s_Pool.Set(toRelease);
        }
    }


}
