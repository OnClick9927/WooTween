using System;
using WooTween;

namespace WooPool
{
    public static class PoolEx
    {
        private class GPool : BaseTypePool<object>
        {
            protected override IObjectPool CreatePool(Type type)
            {
                if (type.IsArray)
                {
                    var poolType = typeof(ArrayPool<>).MakeGenericType(type.GetElementType());
                    return Activator.CreateInstance(poolType) as IObjectPool;
                }
                return null;
            }

            protected override void OnDispose()
            {

            }
        }
         private static GPool gPool = new GPool();

        /// <summary>
        /// 获取全局对象池数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static int GetGlbalPoolCount<T>()
        {
            return gPool.GetPoolCount<T>();
        }
        /// <summary>
        /// 设置全局对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pool"></param>
        public static void SetGlbalPool<T>(ObjectPool<T> pool)
        {
            gPool.SetPool(pool);
        }
        /// <summary>
        /// 全局分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static T GlobalAllocate<T>(IPoolArgs arg = null) where T : class
        {
            return gPool.Get<T>(arg);
        }
        public static Object GlobalAllocate(Type type, IPoolArgs arg = null)
        {
            return gPool.Get(type, arg);
        }
        /// <summary>
        /// 全局回收
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public static void GlobalRecycle<T>(T t, IPoolArgs arg = null) where T : class
        {
            gPool.Set(t, arg);
        }
        /// <summary>
        /// 分配数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] GlobalAllocateArray<T>(int length)
        {
            var result = GlobalAllocate<T[]>(new ArrayPoolArg(length));
            return result;
        }


   
    }
}
