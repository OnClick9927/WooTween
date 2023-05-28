using System;

namespace WooPool
{
    public abstract class DelegatePool<T> : ObjectPool<T>
    {
        private Func<T> _create;

        public DelegatePool(Func<T> create)
        {
            _create = create;
        }

        protected override T CreateNew(IPoolArgs arg)
        {
            if (_create == null)
                return default;
            return _create.Invoke();
        }
    }
}
