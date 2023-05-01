
namespace WooTween
{
    /// <summary>
    /// 框架入口
    /// </summary>
    public static class PoolEx
    {

        private class GlobalPool : BaseTypePool<object> { }
        static private GlobalPool _globalPool = new GlobalPool();

        /// <summary>
        /// 全局分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static T GlobalAllocate<T>()where T: class
        {
            return _globalPool.Get<T>();
        }
        /// <summary>
        /// 全局回收
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public static void GlobalRecyle<T>(T t)where T :class
        {
            _globalPool.Set(t);
        }

    }
}
