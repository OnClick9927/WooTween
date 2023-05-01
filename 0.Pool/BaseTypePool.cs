using System;
using System.Collections.Generic;

namespace WooTween
{
    /// <summary>
    /// 统一类型的对象池
    /// </summary>
    /// <typeparam name="T">基础类型</typeparam>
    public abstract class BaseTypePool<T>
    {

        private Dictionary<Type, IObjectPool> _poolMap;
        private object para = new object();

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseTypePool()
        {
            _poolMap = new Dictionary<Type, IObjectPool>();
        }

        /// <summary>
        /// 获取内部对象池
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        public ObjectPool<Object> GetPool<Object>() where Object : T
        {
            Type type = typeof(Object);
            var pool = GetPool(type);
            return pool as ObjectPool<Object>;
        }

        /// <summary>
        /// 获取内部对象池
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IObjectPool GetPool(Type type)
        {
            lock (para)
            {
                IObjectPool pool;
                if (!_poolMap.TryGetValue(type, out pool))
                {
                    pool = CreatePool(type);
                    if (pool == null)
                    {
                        var pooType = typeof(ActivatorCreatePool<>).MakeGenericType(type);
                        pool = Activator.CreateInstance(pooType, null) as IObjectPool;
                    }
                    _poolMap.Add(type, pool);
                }
                return pool;
            }
        }
        /// <summary>
        /// 創建对象池
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual IObjectPool CreatePool(Type type)
        {
            return null;
        }

      
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Object Get<Object>() where Object : T
        {
            var pool = GetPool<Object>();
            Object t = pool.Get();
            return t;
        }

        /// <summary>
        /// 回收数据
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set<Object>(Object t) where Object : T
        {
            Type type = t.GetType();
            var pool = GetPool(type);
            pool.Set(t);
        }



        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            lock (para)
            {
                foreach (var item in _poolMap.Values) item.Dispose();
                _poolMap.Clear();
            }
        }

    }

}
