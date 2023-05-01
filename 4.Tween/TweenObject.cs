
namespace WooTween
{
    public abstract class TweenObject
    {
        protected EnvironmentType envType;
        private bool _recyled = true;
        public bool recyled { get { return _recyled; } }
        public static T Allocate<T>(EnvironmentType envType) where T : TweenObject
        {
            T t = PoolEx.GlobalAllocate<T>();
            t.envType = envType;
            t._recyled = false;
            return t;
        }
        protected abstract void Reset();
        public void Recyle()
        {
            if (recyled) return;
            Reset();
            _recyled = true;
            PoolEx.GlobalRecyle(this);
        }
    }
}